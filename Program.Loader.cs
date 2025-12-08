using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        List<IMyTerminalBlock> LoadoutContainers => Memo.Of("LoaderContainers", 1, () => {
            var list = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(list, b => Util.IsTagged(b, loadoutTag));
            return list;
        });

        MyIni LoadoutConfig = new MyIni();

        void InitLoader() {
            Task.RunTask(LoadoutTask()).Every(1);
        }

        IEnumerable LoadoutTask() {
            var inventories = InventoryBlocks.Where(b =>
                !(
                    b.Closed
                    || (b is IMyGasGenerator)
                    || (b is IMyReactor)
                    || (b is IMyShipWelder)
                    || (b is IMyParachute)
                    || b.BlockDefinition.TypeIdString.Contains("Turret")
                    || b.BlockDefinition.TypeIdString.Contains("Gatling")
                    || b.BlockDefinition.TypeIdString.Contains("Launcher")
                )
            ).SelectMany(block => {
                var inv = new List<IMyInventory>();
                for (int i = 0; i < block.InventoryCount; i++) {
                    inv.Add(block.GetInventory(i));
                }
                return inv;
            });

            foreach (var container in LoadoutContainers.Where(c => !c.Closed)) {
                if (LoadoutConfig.TryParse(container.CustomData)) {
                    var items = new List<MyIniKey>();
                    LoadoutConfig.GetKeys(items);

                    foreach (var item in items) {
                        var amount = LoadoutConfig.Get(item).ToSingle();
                        var itemType = Items[item.Name].TypeId;

                        // Find itemType in inventories
                        var currentAmount = (float)container.GetInventory().GetItemAmount(itemType);
                        if (currentAmount >= amount)
                            continue;
                        var neededAmount = amount - currentAmount;
                        foreach (var inventory in inventories) {
                            var itm = inventory.FindItem(itemType);
                            if (!itm.HasValue || itm.Value.Amount == 0)
                                continue;
                            var toTransfer = Math.Min(neededAmount, (float)itm.Value.Amount);
                            inventory.TransferItemTo(container.GetInventory(), itm.Value, (MyFixedPoint)toTransfer);
                            neededAmount -= toTransfer;
                            if (neededAmount <= 0)
                                break;
                        }
                        yield return null;
                    }
                }
            }
        }
    }
}