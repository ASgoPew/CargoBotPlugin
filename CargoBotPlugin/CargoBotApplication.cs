using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
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
        public const int fieldH = 12;

        public static int width = 24;
        public static int height = 4 + fieldH + 8;

        public CargoBotGame Game { get; set; }

        public CargoBotApplication(string name)
            : base(name, width, height, new ApplicationStyle()
            {
                SavePosition = true,
                SaveSize = false,
                SaveEnabled = true
            })
        {
            SetupLayout(Alignment.Up, Direction.Down, childIndent: 0);

            AddToLayout(new Label(0, 0, width, 4, "cargo bot", new LabelStyle() { Wall = WallID.DiamondGemspark }));

            VisualContainer field = AddToLayout(new VisualContainer(0, 4, width, fieldH, null, new ContainerStyle() { Wall = WallID.DiamondGemspark }));
            field.Add(new VisualObject(0, 0, width, 1, null, new UIStyle()
                { Tile = TileID.SlimeBlock, InActive = true, TileColor = PaintID2.White }));
            field.Add(new VisualObject(0, 11, width, 1, null, new UIStyle()
                { Tile = TileID.SlimeBlock, InActive = true, TileColor = PaintID2.White }));
            field.Add(new Crane(2, 1, 8, 7, 2, 2, 1, TileID.SlimeBlock,
                new UIStyle() { TileColor = PaintID2.Brown }));

            Column[] columns = new Column[3];
            for (int i = 0; i < 3; i++)
            {
                columns[i] = new Column(9 + i * 4, 3, 4, 2);
                field.Add(columns[i]);
            }
            columns[0].Push(0);
            columns[0].Push(1);
            columns[0].Push(1);
            columns[0].Push(2);
            columns[1].Push(1);
            columns[2].Push(0);
            columns[2].Push(2);
            columns[2].Push(3);

            Button startButton = AddToLayout(new Button(0, 0, width, 4, "start", null,
                new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.None, Wall = WallID.AmberGemspark, WallColor = PaintID2.Gray }));
            Button leaderboardButton = AddToLayout(new Button(0, 0, width, 4, "leaderboard", null,
                new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.None, Wall = WallID.RubyGemspark, WallColor = PaintID2.Gray }));

            Menu menu1 = Add(new Menu(0, 0, 0, 0, CargoBotPlugin.LevelsByName.Keys.Append("back"),
                new ButtonStyle() { Wall = 154, WallColor = PaintID2.Gray },
                new ButtonStyle() { Wall = 156, WallColor = PaintID2.Gray },
                "Select pack", new LabelStyle() { Wall = 155, WallColor = PaintID2.Gray },
                new Input<string>(null, null))
            ).Disable(false) as Menu;

            Game = Add(new CargoBotGame(0, 0));
            Game.Disable(false);

            Dictionary<string, Menu> menus = new Dictionary<string, Menu>();
            foreach (string pack in CargoBotPlugin.LevelsByName.Keys)
                menus[pack] = Add(
                    new Menu(0, 0, 0, 0, CargoBotPlugin.LevelsByName[pack].Keys.Append("back"),
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
                                Game.Start(CargoBotPlugin.LevelsByName[pack][value], player, account2.ID);
                        }))
                ).Disable(false) as Menu;

            startButton.Callback = (self, t) => Summon(menu1, Alignment.Down);
            leaderboardButton.Callback = (self, t) =>
            {
                Leaderboard globalLeaderboard = new Leaderboard(0, 0, 50, 50, Game.LeaderboardDatabaseKey,
                    new LeaderboardStyle() { Ascending = false, Count = 100 });
                globalLeaderboard.AddFooter(new Button(0, 0, 0, 4, "back", null, new ButtonStyle() { Wall = 154, WallColor = 27 }, (self2, touch) => Unsummon()));
                globalLeaderboard.LoadDBData();
                Summon(globalLeaderboard, Alignment.Down);
            };
            menu1.Input.Callback = (self, value, player) =>
            {
                if (value == "back")
                    Unsummon();
                else
                    Summon(menus[value], Alignment.Down);
            };
        }
        protected override void EndPlayerSessionNative()
        {
            base.EndPlayerSessionNative();

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
