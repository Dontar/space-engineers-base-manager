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
        List<IMyTerminalBlock> LoadoutContainers => Memo.Of("LoaderContainers", 5, () => {
            var list = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(list, b => Util.IsTagged(b, loadoutTag));
            return list;
        });

        MyIni LoadoutConfig = new MyIni();

        void InitLoader() {
            Task.RunTask(LoadoutTask()).Every(1);
        }

        IEnumerable LoadoutTask() {
            var inventories = Inventories;
            var items = new List<MyIniKey>();

            foreach (var container in LoadoutContainers.Where(c => !c.Closed)) {
                if (LoadoutConfig.TryParse(container.CustomData) && !string.IsNullOrEmpty(container.CustomData)) {
                    LoadoutConfig.GetKeys(items);

                    foreach (var item in items) {
                        var amount = LoadoutConfig.Get(item).ToSingle();
                        var itemType = Items[item.Name].TypeId;

                        // Find itemType in inventories
                        var currentAmount = (float)container.GetInventory().GetItemAmount(itemType);
                        var neededAmount = amount - currentAmount;
                        if (neededAmount <= 0)
                            continue;
                        foreach (var inventory in inventories) {
                            var itm = inventory.FindItem(itemType);
                            if (!itm.HasValue)
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