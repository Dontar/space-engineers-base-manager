using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        List<IMyDoor> Doors;
        List<MyTuple<IMyDoor, IMyDoor>> AirLocks;
        void InitAirLocks() {
            Doors = Util.GetBlocks<IMyDoor>(b => Util.IsNotIgnored(b));
            AirLocks = new List<MyTuple<IMyDoor, IMyDoor>>();
            var usedDoors = new HashSet<IMyDoor>();
            foreach (var door in Doors) {
                if (usedDoors.Contains(door))
                    continue;

                IMyDoor closestDoor = null;

                foreach (var otherDoor in Doors) {
                    if (otherDoor == door || usedDoors.Contains(otherDoor))
                        continue;

                    var doorPos = door.Position;
                    var otherDoorPos = otherDoor.Position;
                    double distance = Vector3I.DistanceManhattan(doorPos, otherDoorPos);
                    if (distance < 3 && doorPos.Z == otherDoorPos.Z) {
                        closestDoor = otherDoor;
                    }
                }

                if (closestDoor != null) {
                    AirLocks.Add(MyTuple.Create(door, closestDoor));
                    usedDoors.Add(door);
                    usedDoors.Add(closestDoor);
                }
            }
            Task.SetInterval(() => {
                var openingDoors = Doors.Where(d => d.Status == DoorStatus.Opening);

                foreach (var door in openingDoors) {
                    var airLock = AirLocks.Find(l => l.Item1 == door || l.Item2 == door);
                    if (airLock.Item1 != null && airLock.Item2 != null)
                    {
                        IMyDoor otherDoor = door == airLock.Item1 ? airLock.Item2 : airLock.Item1;
                        otherDoor.Enabled = false;
                    }
                }

            }, 0);

            Task.SetInterval(() => {
                var openedDoors = Doors.Where(d => d.Status == DoorStatus.Open);

                foreach (var door in openedDoors) {
                    door.CloseDoor();
                    var airLock = AirLocks.Find(l => l.Item1 == door || l.Item2 == door);
                    if (airLock.Item1 == null || airLock.Item2 == null)
                        continue;
                    IMyDoor otherDoor = door == airLock.Item1 ? airLock.Item2 : airLock.Item1;
                    if (otherDoor.Status == DoorStatus.Closed) {
                        Task.SetTimeout(() => {
                            otherDoor.Enabled = true;
                        }, 1);
                    }
                }
            }, 2);
        }
    }
}