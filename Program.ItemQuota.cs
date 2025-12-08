using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Extensions;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        List<IMyProductionBlock> Producers => Memo.Of("Producers", TimeSpan.FromSeconds(10), () => Util.GetBlocks<IMyProductionBlock>(b => {
            if (!b.IsFunctional)
                return false;
            return
                b is IMyAssembler && b.BlockDefinition.TypeIdString != "MyObjectBuilder_SurvivalKit"
                || b.BlockDefinition.SubtypeId == "FoodProcessor";
        }));

        MyIni ItemsQuota => Memo.Of("ItemsConfig", Me.CustomData, () => {
            var ini = new MyIni();
            ini.TryParse(Me.CustomData);
            return ini;
        });

        void InitQuota() {

            var result = Producers.SelectMany(a => {
                var items = new List<MyItemType>();
                a.OutputInventory.GetAcceptedItems(items);
                return items;
            }).Join(Items, o => o, i => i.Value.TypeId, (_, r) => r);

            foreach (var item in result) {
                var subtypeId = item.Value.TypeId.SubtypeId;
                if (!ItemsQuota.ContainsKey(subtypeId, item.Key))
                    ItemsQuota.Set(subtypeId, item.Key, "0");
            }
            Me.CustomData = ItemsQuota.ToString();

            var keys = new List<MyIniKey>();
            var assemblers = Producers.Where(p => {
                if (!p.Enabled)
                    return false;
                if (p is IMyAssembler)
                    return !((IMyAssembler)p).CooperativeMode;
                return true;
            });
            Task.SetInterval(() => {
                ItemsQuota.GetKeys(keys);

                foreach (var item in keys) {
                    var quota = ItemsQuota.Get(item.Section, item.Name).ToInt32();
                    if (quota == 0)
                        continue;
                    var itemMeta = Items[item.Name];
                    var neededAmount = quota - GetInventoryItemsCount(itemMeta, Producers);

                    if (neededAmount > 0)
                        QueueItems(itemMeta, neededAmount, assemblers.Where(a => a.CanUseBlueprint(itemMeta.BlueprintId)).ToList());
                }
            }, 1.5f);
        }

        int GetInventoryItemsCount(Meta item, List<IMyProductionBlock> assemblers) {
            var inventoryItemAmount = InventoryBlocks.SelectMany(block => {
                var inv = new List<IMyInventory>();
                for (int i = 0; i < block.InventoryCount; i++) {
                    inv.Add(block.GetInventory(i));
                }
                return inv;
            }).Sum(inv => inv.GetItemAmount(item.TypeId).ToIntSafe());

            Func<MyProductionItem, int> selector = qItem => qItem.Amount.ToIntSafe();
            Func<MyProductionItem, bool> predicate = qItem => qItem.BlueprintId == item.BlueprintId;
            var queuedItemAmount = assemblers.Sum(a => {
                var qItems = new List<MyProductionItem>();
                a.GetQueue(qItems);
                return qItems.Where(predicate).Sum(selector);
            });

            return inventoryItemAmount + queuedItemAmount;
        }

        void QueueItems(Meta item, int neededAmount, List<IMyProductionBlock> assemblers) {
            if (assemblers.Count == 0)
                return;

            var amount = neededAmount > assemblers.Count ? neededAmount / assemblers.Count : neededAmount;
            foreach (var assembler in assemblers)
                assembler.AddQueueItem(item.BlueprintId, (MyFixedPoint)amount);
        }
    }
}