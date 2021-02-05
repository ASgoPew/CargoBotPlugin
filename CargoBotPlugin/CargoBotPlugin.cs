using FakeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TerrariaUI;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Widgets;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace CargoBot
{
    [ApiVersion(2, 1)]
    public class CargoBotPlugin : TerrariaPlugin
    {
        public override string Name => "CargoBot";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public override string Author => "ASgo & Anzhelika";
        public override string Description => "CargoBot TUI game";

        public const int MaxInstances = 255;
        internal static UserSaver UserSaver = new UserSaver();
        public static Application Application;

        public CargoBotPlugin(Main game)
            : base(game)
        {

        }

        #region Levels

        #region 1

        public static CargoBotLevel level_1 = new CargoBotLevel(1, 1,
            new List<int[]> {
                new int[] { 0 },
                new int[] { 0, 1 },
                new int[] { 0, 1, 2 },
                new int[] { 0, 1, 2, 3 },
                new int[] { },
                new int[] { 3 },
                new int[] { },
                new int[] { 3, 2, 3, 2, 1, 0 }
            },
            new List<int[]>
            {
                new int[] { 0 },
                new int[] { 0, 1 },
                new int[] { 0, 1 },
                new int[] { 0, 1, 2, 3 },
                new int[] { 2 },
                new int[] { 3 },
                new int[] { },
                new int[] { 3, 2, 3, 2, 1, 0 }
            },
            new int[] { 1, 2, 3, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
            (10, 10, 6),
            "kek hint");

        #endregion
        #region 2

        public static CargoBotLevel level_2 = new CargoBotLevel(2, 1,
            new List<int[]> {
                new int[] { 0 },
                new int[] { 0, 1 },
                new int[] { 0, 1, 2 },
                new int[] { 0, 1, 2, 3 },
                new int[] { },
                new int[] { 3 },
                new int[] { },
                new int[] { 3, 2, 3, 2, 1, 0 }
            },
            new List<int[]>
            {
                new int[] { 0 },
                new int[] { 0, 1 },
                new int[] { 0, 1 },
                new int[] { 0, 1, 2, 3, 2 },
                new int[] { },
                new int[] { 3 },
                new int[] { },
                new int[] { 3, 2, 3, 2, 1, 0 }
            },
            new int[] { 1, 2, 3, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
            (10, 10, 6),
            "kek hint 2");

        #endregion

        public static Dictionary<string, Dictionary<string, CargoBotLevel>> Levels =
            new Dictionary<string, Dictionary<string, CargoBotLevel>>()
            {
                { "Tutorial", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Cargo 101", level_1},
                        { "Transporter", level_2},
                        /*{ "Re-Curses", level_3},
                        { "Inverter", level_4},
                        { "From Beneath", level_5},
                        { "Go Left", level_6}*/
                    }
                },
                /*{ "Easy", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Double Flip", level_6},
                        { "Go Left 2", level_7},
                        { "Shuffle Sort", level_8},
                        { "Go the Distance", level_9},
                        { "Color Sort", level_10},
                        { "Walking Piles", level_11}
                    }
                },
                { "Medium", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Repeat Inverter", level_12},
                        { "Double Sort", level_13},
                        { "Mirror", level_14},
                        { "Lay it out", level_15},
                        { "The Stacker", level_16},
                        { "Clarity", level_17}
                    }
                },
                { "Hard", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Come Together", level_18},
                        { "Come Together 2", level_19},
                        { "Up The Greens", level_20},
                        { "Fill The Blanks", level_21},
                        { "Count The Blues", level_22},
                        { "Multi Sort", level_23}
                    }
                },
                { "Crazy", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Divide by two", level_24},
                        { "The Merger", level_25},
                        { "Even the Odds", level_26},
                        { "Genetic Code", level_27},
                        { "Multi Sort 2", level_28},
                        { "The Swap", level_29}
                    }
                },
                { "Impossible", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Restoring Order", level_30},
                        { "Changing  Places", level_31},
                        { "Palette Swap", level_32},
                        { "Mirror 2", level_33},
                        { "Changing Places 2", level_34},
                        { "Vertical Sort", level_35}
                    }
                }*/
            };

        #endregion

        private static Command[] ChatCommands = new Command[] { };

        public override void Initialize()
        {
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            PlayerHooks.PlayerLogout += OnPlayerLogout;

            Commands.ChatCommands.AddRange(ChatCommands);

            Application = new Application("cargobot", CreateGameInstance);
            TUI.RegisterApplication(Application);
        }

        public static IEnumerable<CargoBotGame> Games => Application.Instances.Values
            .Select(instance => (CargoBotGame)instance["game"]);

        public static CargoBotGame GameByUser(int user) => Games.Where(game => game.User == user).FirstOrDefault();

        private void OnServerLeave(LeaveEventArgs args)
        {
            foreach (CargoBotGame game in Games)
                if (game.Playing && game.Player.Index == args.Who)
                    game.Stop();
        }

        private void OnPlayerLogout(PlayerLogoutEventArgs args)
        {
            foreach (CargoBotGame game in Games)
                if (game.Playing && game.Player.Index == args.Player.Index)
                    game.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
                PlayerHooks.PlayerLogout -= OnPlayerLogout;

                foreach (Command cmd in ChatCommands)
                    Commands.ChatCommands.Remove(cmd);
            }
            base.Dispose(disposing);
        }

        public static Panel CreateGameInstance(string name)
        {
            int w = 20;
            int h = 4;

            Panel cargopanel = new Panel(name, 0, 0, w, h,
                style: new PanelStyle() { SavePosition = true, SaveSize = false, SaveEnabled = true },
                provider: FakeProviderAPI.CreateTileProvider(name, 0, 0, w, h));

            Button summon_button = cargopanel.Add(new Button(0, 0, w, h, "cargobot", null,
                new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.None, Wall = 155 }));

            Menu cargomenu1 = cargopanel.Add(new Menu(0, 0, Levels.Keys.Append("back"),
                new ButtonStyle() { Wall = 154, WallColor = PaintID2.Gray },
                new ButtonStyle() { Wall = 156, WallColor = PaintID2.Gray },
                "Select pack", new LabelStyle() { Wall = 155, WallColor = PaintID2.Gray },
                new Input<string>(null, null))
            ).Disable(false) as Menu;

            CargoBotGame cargoBot = cargopanel.Add(new CargoBotGame(0, 0));
            cargoBot.Disable(false);
            cargopanel["game"] = cargoBot;

            Dictionary<string, Menu> menus = new Dictionary<string, Menu>();
            foreach (string pack in Levels.Keys)
                menus[pack] = cargopanel.Add(
                    new Menu(0, 0, Levels[pack].Keys.Append("back"),
                    new ButtonStyle() { Wall = 154, WallColor = PaintID2.Gray },
                    new ButtonStyle() { Wall = 156, WallColor = PaintID2.Gray },
                    "Select level", new LabelStyle() { Wall = 155, WallColor = PaintID2.Gray },
                    new Input<string>(null, null, (self, value, playerIndex) =>
                    {
                        TSPlayer player = TShock.Players[playerIndex];
                        if (value == "back")
                            cargopanel.Unsummon();
                        else if (!(player.Account is UserAccount account2))
                            player.SendErrorMessage("You have to be logged in to play this game.");
                        else if (Games.Any(pair => pair.Playing && pair.User == account2.ID))
                            player.SendErrorMessage("You are already playing this game.");
                        else
                            cargoBot.Start(Levels[pack][value], player, account2.ID);
                    }))
                ).Disable(false) as Menu;

            summon_button.Callback = (self, t) => cargopanel.Summon(cargomenu1);
            cargomenu1.Input.Callback = (self, value, player) =>
            {
                if (value == "back")
                    cargopanel.Unsummon();
                else
                    cargopanel.Summon(menus[value]);
            };

            return cargopanel;
        }
    }
}
