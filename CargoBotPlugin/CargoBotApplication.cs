using FakeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaUI;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Widgets;
using TShockAPI;
using TShockAPI.DB;

namespace CargoBot
{
    public class CargoBotApplication : Application
    {
        public static int width = 24;
        public static int height = 8;

        public CargoBotGame Game { get; set; }

        public CargoBotApplication(string name)
            : base(name, width, height, new ApplicationStyle()
            {
                SavePosition = true,
                SaveSize = false,
                SaveEnabled = true
            }, FakeProviderAPI.CreateTileProvider(name, 0, 0, width, height))
        {
            SetupLayout(Alignment.Up, Direction.Down, childIndent: 0);
            Button summonButton = AddToLayout(new Button(0, 0, width, 4, "cargobot", null,
                new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.None, Wall = 155 }));
            Button leaderboardButton = AddToLayout(new Button(0, 0, width, 4, "leaderboard", null,
                new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.None, Wall = 155 }));

            Menu menu1 = Add(new Menu(0, 0, CargoBotPlugin.Levels.Keys.Append("back"),
                new ButtonStyle() { Wall = 154, WallColor = PaintID2.Gray },
                new ButtonStyle() { Wall = 156, WallColor = PaintID2.Gray },
                "Select pack", new LabelStyle() { Wall = 155, WallColor = PaintID2.Gray },
                new Input<string>(null, null))
            ).Disable(false) as Menu;

            Game = Add(new CargoBotGame(0, 0));
            Game.Disable(false);

            Dictionary<string, Menu> menus = new Dictionary<string, Menu>();
            foreach (string pack in CargoBotPlugin.Levels.Keys)
                menus[pack] = Add(
                    new Menu(0, 0, CargoBotPlugin.Levels[pack].Keys.Append("back"),
                    new ButtonStyle() { Wall = 154, WallColor = PaintID2.Gray },
                    new ButtonStyle() { Wall = 156, WallColor = PaintID2.Gray },
                    "Select level", new LabelStyle() { Wall = 155, WallColor = PaintID2.Gray },
                    new Input<string>(null, null, (self, value, playerIndex) =>
                    {
                        TSPlayer player = TShock.Players[playerIndex];
                        if (value == "back")
                            Unsummon();
                        else if (!(player.Account is UserAccount account2))
                            player.SendErrorMessage("You have to be logged in to play this game.");
                        else if (CargoBotPlugin.Games.Any(pair => pair.Playing && pair.User == account2.ID))
                            player.SendErrorMessage("You are already playing this game.");
                        else
                            Game.Start(CargoBotPlugin.Levels[pack][value], player, account2.ID);
                    }))
                ).Disable(false) as Menu;

            summonButton.Callback = (self, t) => Summon(menu1);
            leaderboardButton.Callback = (self, t) =>
            {
                Leaderboard globalLeaderboard = new Leaderboard(0, 0, 40, 60, Game.LeaderboardDatabaseKey,
                    new LeaderboardStyle() { Ascending = false });
                globalLeaderboard.AddFooter(new Button(0, 0, 0, 4, "back", null, new ButtonStyle() { Wall = 155 }, (self2, touch) => Unsummon()));
                globalLeaderboard.LoadDBData();
                Summon(globalLeaderboard);
            };
            menu1.Input.Callback = (self, value, player) =>
            {
                if (value == "back")
                    Unsummon();
                else
                    Summon(menus[value]);
            };
        }

        protected override void StartPlayerSessionNative()
        {
            base.StartPlayerSessionNative();

            //ApplicationStyle.TrackInMotion = true;
            //ApplicationStyle.TrackAlignment = Alignment.Center;
            //ApplicationStyle.TrackingPlayer = SessionPlayers[0];
        }

        protected override void EndPlayerSessionNative()
        {
            base.EndPlayerSessionNative();

            //ApplicationStyle.TrackingPlayer = -1;

            Game.Stop();
        }

        protected override void OnTimeoutNative()
        {
            if (!Game.Playing)
                return;

            if (Game.Running)
                Game.ExitRequested = true;
            else
                EndPlayerSession();
        }
    }
}
