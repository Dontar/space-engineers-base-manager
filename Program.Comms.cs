// <mdk sortorder="1000" />
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Sandbox.Game.VoiceChat;
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
                Menu.HandleRemoteMessages();
            }, 1);
        }

        class ShipMenu : MenuManager
        {
            public ShipMenu(Program program) : base(program) {
                ShowShipMenu();
            }

            void ConnectToShip(long entityId) {
                var screen = program.Screens.FirstOrDefault();
                if (screen == null) return;
                var screenLines = Util.ScreenLines(screen);
                var screenColumns = Util.ScreenColumns(screen, '=');
                ConnectToRemoteMenu(entityId, screenLines, screenColumns);
            }

            void ShowShipMenu() {
                var menu = CreateMenu("Ship List");
                menu.AddArray(program.shipList.Select(kv => new Item(kv.Key, () => ConnectToShip(kv.Value))).ToArray());
            }
        }

        List<IMyTextSurface> Screens => Memo.Of("Screens", TimeSpan.FromSeconds(3), () => Util.GetScreens(remoteShipMenuTag));
        void InitMenu() {
            Menu = new ShipMenu(this);
            Task.SetInterval(() => {
                Screens.ForEach(s => Menu.Render(s));
            }, 1);

        }
    }
}
