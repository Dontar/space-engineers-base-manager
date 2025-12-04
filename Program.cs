using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        public Program() {
            Util.Init(this);
            InitAirLocks();
            InitPower();
            Task.RunTask(Util.StatusMonitorTask(this));
            Task.RunTask(Util.DisplayLogo("Base Manager", Me.GetSurface(0))).Every(1.5f);
        }

        public void Main(string argument, UpdateType updateSource) {
            if (!updateSource.HasFlag(UpdateType.Update10))
                return;

            Task.Tick(Runtime.TimeSinceLastRun);
        }
    }
}
