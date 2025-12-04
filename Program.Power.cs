using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        List<IMyBatteryBlock> Batteries;
        List<IMyReactor> Reactors;
        List<IMyPowerProducer> Engines;

        void InitPower() {
            Batteries = Util.GetBlocks<IMyBatteryBlock>(b => Util.IsNotIgnored(b));
            var generators = Util.GetBlocks<IMyPowerProducer>(b => !(b is IMyBatteryBlock || b is IMySolarOccludable || b is IMyWindTurbine));
            Reactors = generators.Where(g => g is IMyReactor).Cast<IMyReactor>().ToList();
            Engines = generators.Where(g => !(g is IMyReactor)).ToList();
            Reactors.ForEach(r => r.Enabled = false);
            Engines.ForEach(e => e.Enabled = false);
            var maxStoredPower = Batteries.Sum(b => b.MaxStoredPower);

            Task.SetInterval(() => {
                var storedPower = Batteries.Sum(b => b.CurrentStoredPower);
                var powerPercent = storedPower / Math.Min(1, maxStoredPower);

                if (powerPercent < 0.1) {
                    Reactors.ForEach(r => r.Enabled = true);
                    Engines.ForEach(e => e.Enabled = true);
                }
                if (powerPercent > 0.9) {
                    Reactors.ForEach(r => r.Enabled = false);
                    Engines.ForEach(e => e.Enabled = false);
                }

            }, 2);
        }
    }
}