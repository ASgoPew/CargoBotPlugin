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

        private const int empty = 0;
        private const int right_arrow = 1;
        private const int down_arrow = 2;
        private const int left_arrow = 3;
        private const int f1 = 4;
        private const int f2 = 5;
        private const int f3 = 6;
        private const int f4 = 7;
        private const int red_condition = 8;
        private const int blue_condition = 9;
        private const int green_condition = 10;
        private const int yellow_condition = 11;
        private const int no_condition = 12;
        private const int multi_condition = 13;

        private const int r = 0;
        private const int b = 1;
        private const int g = 2;
        private const int y = 3;

        #region 1

        public static CargoBotLevel level_1 = new CargoBotLevel("Cargo 101", 1,
            new List<int[]>
            {
                new int[] { y },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4 },
            (3, 3, 3),
            "Down, Right, Down");

        #endregion
        #region 2

        public static CargoBotLevel level_2 = new CargoBotLevel("Transporter", 1,
            new List<int[]>
            {
                new int[] { y },
                new int[] { },
                new int[] { },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4 },
            (5, 4, 4),
            "Reuse the solution from level 1 and loop through it.\n\nThe shortest solution uses 4 registers.");

        #endregion
        #region 3 

        public static CargoBotLevel level_3 = new CargoBotLevel("Re-Curses", 1,
            new List<int[]>
            {
                new int[] { y, y, y, y },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { y, y, y, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4 },
            (10, 5, 5),
            "Move one crate to the right, go back to the original position, and then loop.\n\nThe shortest solution uses 5 registers.");

        #endregion
        #region 4

        public static CargoBotLevel level_4 = new CargoBotLevel("Inverter", 1,
            new List<int[]>
            {
                new int[] { b, r, g, y },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { y, g, r, b }
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4 },
            (15, 10, 10),
            "Move all four blocks one spot to the right, and repeat.\n\nThe shortest solution uses 10 registers.");

        #endregion
        #region 5

        public static CargoBotLevel level_5 = new CargoBotLevel("From Beneath", 1,
            new List<int[]>
            {
                new int[] { y, b, b, b, b },
                new int[] { },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { b, b, b, b },
                new int[] { y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, yellow_condition, no_condition, multi_condition },
            (8, 6, 5),
            "Go right once if holding blue, twice if holding yellow, and left if holding none. Repeat.\n\nThe shortest solution uses 5 registers.");

        #endregion
        #region 6

        public static CargoBotLevel level_6 = new CargoBotLevel("Go Left", 1,
            new List<int[]>
            {
                new int[] { },
                new int[] { r, r, r },
                new int[] { g, g, g },
                new int[] { b, b, b },
            },
            new List<int[]>
            {
                new int[] { r, r, r },
                new int[] { g, g, g },
                new int[] { b, b, b },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4 },
            (15, 9, 9),
            "Move each pile to the left. Repeat.\n\nThe shortest solution uses 9 registers.");

        #endregion
        #region 7

        public static CargoBotLevel level_7 = new CargoBotLevel("Double Flip", 1,
            new List<int[]>
            {
                new int[] { b, r, g, y },
                new int[] { },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { },
                new int[] { b, r, g, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, red_condition, green_condition, yellow_condition, no_condition, multi_condition },
            (12, 6, 5),
            "Go right once if holding blue, twice if holding yellow, and left if holding none. Repeat.\n\nThe shortest solution uses 5 registers.");

        #endregion
        #region 8

        public static CargoBotLevel level_8 = new CargoBotLevel("Go Left 2", 1,
            new List<int[]>
            {
                new int[] { },
                new int[] { r, r, r },
                new int[] { b, b, b },
                new int[] { g, g, g },
            },
            new List<int[]>
            {
                new int[] { r, r, r },
                new int[] { b, b, b },
                new int[] { g, g, g },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, red_condition, green_condition, empty, no_condition, multi_condition },
            (8, 6, 4),
            "Go right if holding none, and left if holding any. Repeat.\n\nThe shortest solution uses 4 registers.");

        #endregion
        #region 9

        public static CargoBotLevel level_9 = new CargoBotLevel("Shuffle Sort", 2,
            new List<int[]>
            {
                new int[] { },
                new int[] { b, y, b, y, b, y },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { b, b, b },
                new int[] { },
                new int[] { y, y, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4 },
            (15, 10, 9),
            "Alternate left and right, and make sure to use F2 to shorten your solution.\n\nThe shortest solution uses 9 registers.");

        #endregion
        #region 10

        public static CargoBotLevel level_10 = new CargoBotLevel("Go the Distance", 1,
            new List<int[]>
            {
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { },
                new int[] { r, r, r, r },
            },
            new List<int[]>
            {
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { r, r, r, r },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                red_condition, yellow_condition, no_condition, multi_condition },
            (12, 6, 4),
            "Go right if holding none, and left if holding red. Repeat.\n\nThe shortest solution uses 4 registers.");

        #endregion
        #region 11

        public static CargoBotLevel level_11 = new CargoBotLevel("Color Sort", 2,
            new List<int[]>
            {
                new int[] { },
                new int[] { g, g, r, g, r, r },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { r, r, r },
                new int[] { },
                new int[] { g, g, g },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                red_condition, green_condition, no_condition, multi_condition },
            (14, 10, 8),
            "Go over each of the 3 piles and drop or pick up based on the color. When over the left pile drop if red, when over the right pile drop if green.\n\nThe shortest known solution uses 8 registers, all in F1.");

        #endregion
        #region 12

        public static CargoBotLevel level_12 = new CargoBotLevel("Walking Piles", 1,
            new List<int[]>
            {
                new int[] { b, b, b, b },
                new int[] { b, b, b, b },
                new int[] { b, b, b, b },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { b, b, b, b },
                new int[] { b, b, b, b },
                new int[] { b, b, b, b },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, no_condition },
            (13, 11, 9),
            "For a 3 star solution, move each pile 3 slots to the right, and then repeat. This method can be implemented with 10 registers.\n\nThe shortest known solution uses 9 registers (with an approach that is very specific to this configuration)");

        #endregion
        #region 13

        public static CargoBotLevel level_13 = new CargoBotLevel("Repeat Inverter", 1,
            new List<int[]>
            {
                new int[] { y, r, g, b },
                new int[] { },
                new int[] { y, r, g, b },
                new int[] { },
                new int[] { y, r, g, b },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { b, g, r, y },
                new int[] { },
                new int[] { b, g, r, y },
                new int[] { },
                new int[] { b, g, r, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, red_condition, green_condition, yellow_condition, no_condition, multi_condition },
            (9, 7, 5),
            "It can be done with the usual 5 instructions and clever usage of conditional modifiers. Solutions with up to 7 instructions earn 3 stars.");

        #endregion
        #region 14

        public static CargoBotLevel level_14 = new CargoBotLevel("Double Sort", 2,
            new List<int[]>
            {
                new int[] { },
                new int[] { b, b, y, y },
                new int[] { y, b, y, b },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { b, b, b, b },
                new int[] { },
                new int[] { },
                new int[] { y, y, y, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, yellow_condition, no_condition, multi_condition },
            (20, 14, 11),
            "Sort, go right, sort, go left. Repeat. Use at most 14 instructions for 3 stars.\n\nThe shortest known solution uses 11 registers.");

        #endregion
        #region 15

        public static CargoBotLevel level_15 = new CargoBotLevel("Mirror", 1,
            new List<int[]>
            {
                new int[] { y, y, y, y },
                new int[] { g, g },
                new int[] { g },
                new int[] { g },
                new int[] { g, g },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { g, g },
                new int[] { g },
                new int[] { g },
                new int[] { g, g },
                new int[] { y, y, y, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                green_condition, yellow_condition, no_condition, multi_condition },
            (9, 7, 6),
            "Use at most 7 registers for 3 stars. There are various known solutions with 6 registers in F1, but no known solution with only 5.");

        #endregion
        #region 16

        public static CargoBotLevel level_16 = new CargoBotLevel("Lay it out", 1,
            new List<int[]>
            {
                new int[] { g, g, g, g, g, g },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { g },
                new int[] { g },
                new int[] { g },
                new int[] { g },
                new int[] { g },
                new int[] { g },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                green_condition, no_condition },
            (13, 9, 7),
            "Move the pile one slot to the right and bring one crate back to the left.\n\nThe shortest known solution uses 7 registers.");

        #endregion
        #region 17

        public static CargoBotLevel level_17 = new CargoBotLevel("The Stacker", 5,
            new List<int[]>
            {
                new int[] { },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { y, y, y, y, y, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                yellow_condition, no_condition },
            (12, 10, 8),
            "Go left until you find an empty slot, and then move the last yellow crate one slot to the right. Repeat.\n\nThe shortest known solution uses 8 registers.");

        #endregion
        #region 18

        public static CargoBotLevel level_18 = new CargoBotLevel("Clarity", 1,
            new List<int[]>
            {
                new int[] { g, r, g },
                new int[] { g, g, g, r, g },
                new int[] { r, g, r, g },
                new int[] { r, g, g },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { g, r },
                new int[] { g, g, g, r },
                new int[] { r, g, r },
                new int[] { r },
                new int[] { g, g, g, g, g },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                red_condition, green_condition, no_condition, multi_condition },
            (9, 7, 6),
            "A disguised version of Mirror.");

        #endregion
        #region 19

        public static CargoBotLevel level_19 = new CargoBotLevel("Come Together", 1,
            new List<int[]>
            {
                new int[] { },
                new int[] { },
                new int[] { y, y, y },
                new int[] { y },
                new int[] { },
                new int[] { },
                new int[] { y, y },
            },
            new List<int[]>
            {
                new int[] { y, y, y, y, y, y },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                yellow_condition, no_condition },
            (15, 9, 7),
            "You can go right and find a yellow crate, but when bringing it back how do you know when to stop so that you don't crash into the wall?\n\nIn F2 use the programming stack to count the number of times you have to go right until you find a yellow crate, then go back left that same number of times. Another way to look at it: F2 is a recursive function that goes right until it finds a crate, and then it goes back to the original position. It can be implemented with 4 registers.\n\nThe shortest known solution uses a total of 7 registers.");

        #endregion
        #region 20

        public static CargoBotLevel level_20 = new CargoBotLevel("Come Together 2", 1,
            new List<int[]>
            {
                new int[] { },
                new int[] { y },
                new int[] { y, g, g },
                new int[] { y },
                new int[] { y, g },
                new int[] { y },
                new int[] { g, g, g, g },
            },
            new List<int[]>
            {
                new int[] { g, g, g, g, g, g, g },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { y },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                green_condition, yellow_condition, no_condition, multi_condition },
            (12, 10, 8),
            "Another stack puzzle. Re-use the solution from the previous level with a small modification.\n\nThe shortest known solution uses 8 registers.");

        #endregion

        public static Dictionary<string, Dictionary<string, CargoBotLevel>> Levels =
            new Dictionary<string, Dictionary<string, CargoBotLevel>>()
            {
                { "Tutorial", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Cargo 101", level_1 },
                        { "Transporter", level_2 },
                        { "Re-Curses", level_3 },
                        { "Inverter", level_4 },
                        { "From Beneath", level_5 },
                        { "Go Left", level_6 },
                    }
                },
                { "Easy", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Double Flip", level_7 },
                        { "Go Left 2", level_8 },
                        { "Shuffle Sort", level_9 },
                        { "Go the Distance", level_10 },
                        { "Color Sort", level_11 },
                        { "Walking Piles", level_12 },
                    }
                },
                { "Medium", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Repeat Inverter", level_13 },
                        { "Double Sort", level_14 },
                        { "Mirror", level_15 },
                        { "Lay it out", level_16 },
                        { "The Stacker", level_17 },
                        { "Clarity", level_18 },
                    }
                },
                { "Hard", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Come Together", level_19 },
                        { "Come Together 2", level_20 },
                        /*{ "Up The Greens", level_21 },
                        { "Fill The Blanks", level_22 },
                        { "Count The Blues", level_23 },
                        { "Multi Sort", level_24 },*/
                    }
                },
                /*{ "Crazy", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Divide by two", level_25 },
                        { "The Merger", level_26 },
                        { "Even the Odds", level_27 },
                        { "Genetic Code", level_28 },
                        { "Multi Sort 2", level_29 },
                        { "The Swap", level_30 },
                    }
                },
                { "Impossible", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Restoring Order", level_31 },
                        { "Changing  Places", level_32 },
                        { "Palette Swap", level_33 },
                        { "Mirror 2", level_34 },
                        { "Changing Places 2", level_35 },
                        { "Vertical Sort", level_36 },
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
