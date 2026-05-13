using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        #region mdk preserve

        // Version: 0.1.3

        // Configuration options for the base manager. Set these to false to disable features you don't need, and save some performance.
        bool manageAssemblers = true;
        // Inventories includes cargo containers, refineries, and assemblers. If you disable this, the base manager will not manage inventory transfers at all.
        bool manageInventories = true;
        // Power management includes batteries, solar panels, and reactors. If you disable this, the base manager will not manage power at all.
        bool managePower = true;
        // Air locks will automatically open and close when you approach them, and can be controlled remotely from the ship menu. If you disable this, the base manager will not manage air locks at all.
        bool manageAirLocks = true;
        // Loadout management includes automatic equipping of tools and weapons. If you disable this, the base manager will not manage loadouts at all.
        bool manageLoadout = true;
        // Ship controller includes remote control and menu management. If you disable this, the base manager will not manage ship controls at all.
        bool shipController = true;

        // Tags for identifying blocks. You can change these if you want, but make sure to update the code accordingly.
        string airLockTag = "AirLock_";
        // Screens tagged with this will be used for the menu when connected to a remote ship.
        string remoteShipMenuTag = "RemoteMenu";

        // Inventory tags. Blocks tagged with these will be considered part of that inventory type for loadout management and inventory transfers.
        string oresTag = "Ores";
        string iceTag = "Ice";
        string ingotsTag = "Ingots";
        string componentsTag = "Components";
        string toolsTag = "Tools";
        string ammoTag = "Ammo";
        string loadoutTag = "Loadout";

        #endregion

        public Program() {
            Util.Init(this);
            if (manageAssemblers)
                InitQuota();
            if (manageInventories)
                InitInventories();
            if (managePower)
                InitPower();
            if (manageAirLocks)
                InitAirLocks();
            if (manageLoadout)
                InitLoader();
            if (shipController) {
                InitComms();
                InitMenu();
            }

            Task.RunTask(Util.StatusMonitorTask(this));
            Task.RunTask(Util.DisplayLogo("Base Manager", Me.GetSurface(0))).Every(1.5f);
        }

        MyCommandLine Cmd = new MyCommandLine();
        public void Main(string argument, UpdateType updateSource) {
            if (!updateSource.HasFlag(UpdateType.Update10)) {
                if (Cmd.TryParse(argument))
                    ExecuteCommand();
                return;
            }

            Memo.Tick(Runtime.TimeSinceLastRun);
            Task.Tick(Runtime.TimeSinceLastRun);
        }

        public void ExecuteCommand() {
            switch (Cmd.Argument(0).ToLower()) {
                case "getgps":
                    GetGPSFromCameras();
                    break;
                default:
                    if (!Menu.ProcessMenuCommands(Cmd))
                        Echo($"Unknown command: {Cmd.Argument(0)}");
                    break;
            }
        }
    }
}
