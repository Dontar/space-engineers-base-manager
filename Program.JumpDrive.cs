using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        List<IMyJumpDrive> JumpDrives => Memo.Of("JumpDrive", TimeSpan.FromMinutes(5), () => Util.GetBlocks<IMyJumpDrive>(b => Util.IsNotIgnored(b)));
        List<IMyCameraBlock> Cameras => Memo.Of("Cameras", TimeSpan.FromMinutes(5), () => Util.GetBlocks<IMyCameraBlock>(b => Util.IsNotIgnored(b)));

        void GetGPSFromCameras() {
            var camera = Cameras.FirstOrDefault(c => c.IsActive);
            if (camera != null) {
                var scanResult = camera.Raycast(camera.AvailableScanRange);
                if (!scanResult.IsEmpty()) {
                    var distance = Vector3D.Distance(camera.GetPosition(), scanResult.Position);
                    JumpDrives.ForEach(jd => jd.JumpDistanceMeters = (float)distance);
                    var gps = new MyWaypointInfo(scanResult.Name, scanResult.Position);
                    camera.CustomData += gps.ToString() + Environment.NewLine;
                }
            }
        }
    }
}
