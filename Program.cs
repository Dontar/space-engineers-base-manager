using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        #region mdk preserve
        // Configuration
        bool manageAssemblers = true;
        bool manageInventories = true;
        bool managePower = true;
        bool manageAirLocks = true;

        string oresTag = "Ores";
        string ingotsTag = "Ingots";
        string componentsTag = "Components";
        string toolsTag = "Tools";
        string ammoTag = "Ammo";

        // end of config
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
            Task.RunTask(Util.StatusMonitorTask(this));
            Task.RunTask(Util.DisplayLogo("Base Manager", Me.GetSurface(0))).Every(1.5f);
        }

        public void Main(string argument, UpdateType updateSource) {
            if (!updateSource.HasFlag(UpdateType.Update10))
                return;

            Memo.Tick(Runtime.TimeSinceLastRun);
            Task.Tick(Runtime.TimeSinceLastRun);
        }
    }
}
