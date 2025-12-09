using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        List<IMyTerminalBlock> InventoryBlocks => Memo.Of("InventoryBlocks", TimeSpan.FromSeconds(5), () => Util.GetBlocks<IMyTerminalBlock>(block => block.HasInventory));
        IEnumerable<IMyInventory> Inventories => InventoryBlocks.Where(b =>
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

        void InitInventories() {
            Task.RunTask(InventoryTask()).Every(2);
        }

        IEnumerable InventoryTask() {
            var inventories = Inventories;

            var cargoContainers = InventoryBlocks.OfType<IMyCargoContainer>().SelectMany(block => {
                var inv = new List<IMyInventory>();
                for (int i = 0; i < block.InventoryCount; i++) {
                    inv.Add(block.GetInventory(i));
                }
                return inv;
            }).ToList();
            var OreContainers = cargoContainers.Where(c => Util.IsTagged(c.Owner as IMyTerminalBlock, oresTag));
            var IngotContainers = cargoContainers.Where(c => Util.IsTagged(c.Owner as IMyTerminalBlock, ingotsTag));
            var ComponentContainers = cargoContainers.Where(c => Util.IsTagged(c.Owner as IMyTerminalBlock, componentsTag));
            var ToolsContainers = cargoContainers.Where(c => Util.IsTagged(c.Owner as IMyTerminalBlock, toolsTag));
            var AmmoContainers = cargoContainers.Where(c => Util.IsTagged(c.Owner as IMyTerminalBlock, ammoTag));

            foreach (var inventory in inventories) {
                if (inventory.Owner.Closed)
                    continue;
                if (inventory.ItemCount == 0 || (IsInputInventory(inventory) && !IsFull(inventory)))
                    continue;

                var items = new List<MyInventoryItem>();
                inventory.GetItems(items);

                foreach (var item in items) {
                    IEnumerable<IMyInventory> materialContainers = null;
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
                    if (!materialContainers?.Contains(inventory) ?? false) {
                        var freeContainer = materialContainers.FirstOrDefault(c => c.CanItemsBeAdded(item.Amount, item.Type));
                        freeContainer?.TransferItemFrom(inventory, item);
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
