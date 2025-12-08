using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        List<IMyTerminalBlock> InventoryBlocks => Memo.Of("InventoryBlocks", TimeSpan.FromSeconds(5), () => Util.GetBlocks<IMyTerminalBlock>(block => block.HasInventory));
        void InitInventories() {
            Task.RunTask(InventoryTask()).Every(2);
        }

        IEnumerable InventoryTask() {
            var inventories = InventoryBlocks.Where(b =>
                !(
                    (b is IMyGasGenerator)
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

            var cargoContainers = InventoryBlocks.OfType<IMyCargoContainer>();
            var OreContainers = cargoContainers.Where(c => Util.IsTagged(c, oresTag));
            var IngotContainers = cargoContainers.Where(c => Util.IsTagged(c, ingotsTag));
            var ComponentContainers = cargoContainers.Where(c => Util.IsTagged(c, componentsTag));
            var ToolsContainers = cargoContainers.Where(c => Util.IsTagged(c, toolsTag));
            var AmmoContainers = cargoContainers.Where(c => Util.IsTagged(c, ammoTag));

            foreach (var inventory in inventories) {
                if (inventory.ItemCount == 0 || (IsInputInventory(inventory) && !IsFull(inventory)))
                    continue;

                var items = new List<MyInventoryItem>();
                inventory.GetItems(items);

                foreach (var item in items) {
                    IEnumerable<IMyCargoContainer> materialContainers = null;
                    switch (item.Type.TypeId) {
                        case "MyObjectBuilder_Ore":
                            materialContainers = OreContainers;
                            break;
                        case "MyObjectBuilder_Ingot":
                            materialContainers = IngotContainers;
                            break;
                        case "MyObjectBuilder_Component":
                            materialContainers = ComponentContainers;
                            break;
                        case "MyObjectBuilder_PhysicalGunObject":
                            materialContainers = ToolsContainers;
                            break;
                        case "MyObjectBuilder_AmmoMagazine":
                            materialContainers = AmmoContainers;
                            break;
                        default:
                            break;
                    }
                    if (!materialContainers?.Any(c => c == inventory.Owner) ?? false) {
                        var freeContainer = materialContainers.FirstOrDefault(c => c.GetInventory().CanItemsBeAdded(item.Amount, item.Type));
                        freeContainer?.GetInventory().TransferItemFrom(inventory, item);
                    }
                }
                yield return null;
            }
        }

        bool IsInputInventory(IMyInventory i) {
            return i.Owner is IMyProductionBlock && (i.Owner as IMyProductionBlock).InputInventory == i;
        }

        bool IsFull(IMyInventory i) {
            return i.VolumeFillFactor >= 0.98f;
        }
    }
}
