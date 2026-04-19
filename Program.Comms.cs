using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        ShipMenu Menu;
        Dictionary<string, long> shipList = new Dictionary<string, long>();

        void InitComms() {
            var listener = IGC.RegisterBroadcastListener("SHIP_PING");

            Task.SetInterval(() => {
                while (listener.HasPendingMessage) {
                    var msg = listener.AcceptMessage();
                    if (msg.Tag == "SHIP_PING") {
                        var senderId = msg.Source;
                        shipList[msg.Data.ToString()] = senderId;
                    }
                }
            }, 1);
        }

        class ShipMenu : MenuManager
        {
            public ShipMenu(Program program) : base(program) {
                ShowShipMenu();
            }

            void ShowShipMenu() {
                var menu = CreateMenu("Ship List");
                foreach (var ship in program.shipList) {
                    var localId = ship.Value;
                    menu.Add(new Item(ship.Key, () => ConnectToRemoteMenu(localId)));
                }
            }
        }

        void InitMenu() {
            Menu = new ShipMenu(this);

            var screens = Util.GetScreens("{Base}");

            Task.SetInterval(() => {
                screens.ForEach(s => Menu.Render(s));
            }, 1);

        }
    }
}
