using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        List<IMyPowerProducer> Generators => Memo.Of("Generators", 30, () => Util.GetBlocks<IMyPowerProducer>(b => !(b is IMyBatteryBlock || b is IMySolarOccludable || b is IMyWindTurbine)));
        List<IMyBatteryBlock> Batteries => Memo.Of("Batteries", 20, () => Util.GetBlocks<IMyBatteryBlock>(b => Util.IsNotIgnored(b) && b.ChargeMode == ChargeMode.Auto));
        List<IMyReactor> Reactors => Generators.Where(g => g is IMyReactor).Cast<IMyReactor>().ToList();
        List<IMyPowerProducer> Engines => Generators.Where(g => !(g is IMyReactor)).ToList();

        void InitPower() {
            Reactors.ForEach(r => r.Enabled = false);
            Engines.ForEach(e => e.Enabled = false);

            Task.SetInterval(() => {
                var maxStoredPower = Batteries.Sum(b => b.MaxStoredPower);
                var storedPower = Batteries.Sum(b => b.CurrentStoredPower);
                var powerPercent = Util.NormalizeValue(storedPower, maxStoredPower, 100);

                if (powerPercent < 10) {
                    Reactors.ForEach(r => r.Enabled = true);
                    Engines.ForEach(e => e.Enabled = true);
                }
                if (powerPercent > 95) {
                    Reactors.ForEach(r => r.Enabled = false);
                    Engines.ForEach(e => e.Enabled = false);
                }
            }, 2);
        }
    }
}