// <mdk sortorder="1000" />

using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        List<IMyDoor> Doors => Memo.Of("Doors", TimeSpan.FromSeconds(30), () => Util.GetBlocks<IMyDoor>(b => Util.IsNotIgnored(b)));
        List<IMyDoor[]> AirLocks => Memo.Of("AirLocks", Doors, () => {
            var list = new List<IMyDoor[]>();
            var usedDoors = new HashSet<IMyDoor>();

            foreach (var door in Doors) {
                if (usedDoors.Contains(door))
                    continue;

                var doorPos = door.Position;

                foreach (var otherDoor in Doors) {
                    if (otherDoor == door || usedDoors.Contains(otherDoor))
                        continue;

                    if (otherDoor.CustomData.Contains(airLockTag) && door.CustomData == otherDoor.CustomData) {
                        list.Add(new[] { door, otherDoor });
                        usedDoors.Add(door);
                        usedDoors.Add(otherDoor);
                        break;
                    }

                    var otherDoorPos = otherDoor.Position;
                    var distance = Vector3I.DistanceManhattan(doorPos, otherDoorPos);
                    if (distance < 2) {
                        list.Add(new[] { door, otherDoor });
                        usedDoors.Add(door);
                        usedDoors.Add(otherDoor);
                        break;
                    }
                }
            }
            return list;
        });

        void InitAirLocks() {
            Doors.ForEach(d => d.CloseDoor());

            Task.SetInterval(() => {
                var openingDoors = Doors.Where(d => d.Status == DoorStatus.Opening);

                foreach (var door in openingDoors) {
                    IMyDoor otherDoor = null;
                    var airLock = AirLocks.Find(l => l.Contains(door));
                    if (airLock != null) {
                        otherDoor = door == airLock[0] ? airLock[1] : airLock[0];
                        otherDoor.Enabled = false;
                    }
                    Task.SetTimeout(() => {
                        door.CloseDoor();
                        if (otherDoor != null && otherDoor.Status == DoorStatus.Closed) {
                            new Promise(res => {
                                if (door.Status == DoorStatus.Closed)
                                    res(true);
                            }).Then(_ => otherDoor.Enabled = true);
                        }
                    }, 2);
                }
            }, 0);
            Util.Echo(AirLocks.Count.ToString() + " airlocks initialized.");
        }
    }
}
