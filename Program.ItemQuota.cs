using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        List<IMyAssembler> Assemblers;
        List<IMyProductionBlock> FoodProcessors;
        List<IMyInventory> Inventories;
        MyIni ItemsQuota => Memo.Of("ItemsConfig", Me.CustomData, () => {
            var ini = new MyIni();
            ini.TryParse(Me.CustomData);
            return ini;
        });

        void InitQuota() {
            var producers = Util.GetBlocks<IMyProductionBlock>();
            Assemblers = producers.Where(p => p is IMyAssembler && p.BlockDefinition.TypeIdString != "MyObjectBuilder_SurvivalKit").Cast<IMyAssembler>().ToList();
            FoodProcessors = producers.Where(p => p.BlockDefinition.SubtypeId == "FoodProcessor").ToList();
            Inventories = Util.GetBlocks<IMyTerminalBlock>(b => b.HasInventory).SelectMany(b => {
                var inv = new List<IMyInventory>();
                for (var i = 0; i < b.InventoryCount; i++) {
                    inv.Add(b.GetInventory(i));
                }
                return inv;
            }).ToList();

            var assemblerItems = Assemblers.SelectMany(a => {
                var items = new List<MyItemType>();
                a.InputInventory.GetAcceptedItems(items);
                return items;
            }).Distinct().ToList();

            var foodItem = new List<MyItemType>();
            FoodProcessors.FirstOrDefault()?.OutputInventory.GetAcceptedItems(foodItem);

            var result = Enumerable.Concat(assemblerItems, foodItem).Join(Items, o => o, i => i.Value.ItemType, (_, r) => r).ToDictionary(k => k.Key, v => v.Value.ItemType.TypeId.Substring(16));

            foreach (var item in result)
                if (!ItemsQuota.ContainsKey(item.Value, item.Key))
                    ItemsQuota.Set(item.Value, item.Key, "0");

            Me.CustomData = ItemsQuota.ToString();

            Task.SetInterval(() => {
                var keys = new List<MyIniKey>();
                ItemsQuota.GetKeys(keys);

                foreach (var item in keys) {
                    var itemMeta = Items[item.Name];
                    var itemAmount = GetInventoryItemsCount(itemMeta);
                    var quota = ItemsQuota.Get(item.Section, item.Name).ToInt32();
                    var neededAmount = quota - itemAmount;

                    if (neededAmount > 0)
                        QueueItems(itemMeta, neededAmount);
                }
            }, 1.5f);
        }

        int GetInventoryItemsCount(Meta item) {
            var inventoryItemAmount = Inventories.Sum(inv => inv.GetItemAmount(item.ItemType).ToIntSafe());

            Func<MyProductionItem, int> selector = qItem => qItem.Amount.ToIntSafe();
            Func<MyProductionItem, bool> predicate = qItem => qItem.BlueprintId == item.BlueprintId;
            var queuedItemAmount = Assemblers.Sum(a => {
                var qItems = new List<MyProductionItem>();
                a.GetQueue(qItems);
                return qItems.Where(predicate).Sum(selector);
            });

            return inventoryItemAmount + queuedItemAmount;
        }

        void QueueItems(Meta item, int neededAmount) {
            var assemblers = Enumerable.Concat(Assemblers, FoodProcessors).Where(a => {
                if (!a.CanUseBlueprint(item.BlueprintId))
                    return false;
                
                if (a is IMyAssembler && ((IMyAssembler)a).CooperativeMode)
                    return false;

                return true;
            }
            ).ToList();
            if (assemblers.Count == 0) return;

            var amount = neededAmount / Math.Max(assemblers.Count, 1);
            foreach (var assembler in assemblers) {
                if (neededAmount < 0)
                    break;
                assembler.AddQueueItem(item.BlueprintId, (MyFixedPoint)amount);
                neededAmount -= amount;
            }
        }
    }
}