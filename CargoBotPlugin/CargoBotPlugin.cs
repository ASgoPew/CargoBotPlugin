using CargoBot;
using FakeProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TerrariaUI;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Widgets;
using TerrariaUI.Widgets.Data;
using TerrariaUI.Widgets.Media;
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

        //public static CargoBotGame CargoBot { get; private set; }
        internal static List<CargoBotGame> Games = new List<CargoBotGame>();
        public const string ImagePath = "worldedit\\schematic-cargobot.dat";
        private static int PanelIndex = 0;

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
            new List<List<int[]>>
            {
                new List<int[]>
                {
                    new int[] { 1 },
                    new int[] { 2 },
                    new int[] { 1, 5 },
                    new int[] { 5 }
                },
                new List<int[]>
                {
                    new int[] { 3 },
                    new int[] { 4 }
                },
                new List<int[]> { },
                new List<int[]> { }
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
            new List<List<int[]>>
            {
                new List<int[]>
                {
                    new int[] { 1 },
                    new int[] { 2 },
                    new int[] { 1, 5 },
                    new int[] { 5 }
                },
                new List<int[]>
                {
                    new int[] { },
                    new int[] { 4 }
                },
                new List<int[]> { },
                new List<int[]> { }
            },
            new int[] { 1, 2, 3, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
            (10, 10, 6),
            "kek hint 2");

        #endregion

        #endregion

        public static Dictionary<string, Dictionary<string, CargoBotLevel>> Levels =
            new Dictionary<string, Dictionary<string, CargoBotLevel>>()
            {
                { "Tutorial", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Cargo 101", level_1},
                        { "Transporter", level_2},
                        { "Re-Curses", level_1},
                        { "Inverter", level_1},
                        { "From Beneath", level_1},
                        { "Go Left", level_1}
                    }
                },
                { "Easy", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Double Flip", level_1},
                        { "Go Left 2", level_1},
                        { "Shuffle Sort", level_1},
                        { "Go the Distance", level_1},
                        { "Color Sort", level_1},
                        { "Walking Piles", level_1}
                    }
                },
                { "Medium", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Repeat Inverter", level_1},
                        { "Double Sort", level_1},
                        { "Mirror", level_1},
                        { "Lay it out", level_1},
                        { "The Stacker", level_1},
                        { "Clarity", level_1}
                    }
                },
                { "Hard", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Come Together", level_1},
                        { "Come Together 2", level_1},
                        { "Up The Greens", level_1},
                        { "Fill The Blanks", level_1},
                        { "Count The Blues", level_1},
                        { "Multi Sort", level_1}
                    }
                },
                { "Crazy", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Divide by two", level_1},
                        { "The Merger", level_1},
                        { "Even the Odds", level_1},
                        { "Genetic Code", level_1},
                        { "Multi Sort 2", level_1},
                        { "The Swap", level_1}
                    }
                },
                { "Impossible", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Restoring Order", level_1},
                        { "Changing  Places", level_1},
                        { "Palette Swap", level_1},
                        { "Mirror 2", level_1},
                        { "Changing Places 2", level_1},
                        { "Vertical Sort", level_1}
                    }
                },
            };

        private static Command[] ChatCommands = new Command[]
        {
            new Command("TUI.control", CargoBotCommand, "cargobot", "cargo")
        };

        public override void Initialize()
        {
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            PlayerHooks.PlayerLogout += OnPlayerLogout;

            Commands.ChatCommands.AddRange(ChatCommands);
        }

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

                foreach (var cmd in ChatCommands)
                    Commands.ChatCommands.Remove(cmd);

                foreach (var game in Games)
                    TUI.Destroy(game.Root);
                Games.Clear();
            }
            base.Dispose(disposing);
        }

        private static void CargoBotCommand(CommandArgs args)
        {
            switch (args.Parameters.FirstOrDefault()?.ToLower())
            {
                case "add":
                    StartGame(args);
                    break;
                case "remove":
                    if (Games.Count == 0)
                        args.Player.SendErrorMessage("There are no games.");
                    else
                    {
                        int x = args.Player.TileX;
                        int y = args.Player.TileY;
                        foreach (var game in Games.ToArray())
                            if (game.Root.Contains(x, y))
                            {
                                TUI.Destroy(game.Root);
                                Games.Remove(game);
                                args.Player.SendSuccessMessage($"Removed instance {game.Root.Name}.");
                                return;
                            }
                        args.Player.SendErrorMessage("There are no games at this point.");
                    }
                    break;
                default:
                    args.Player.SendErrorMessage("Usage: /cargobot <add/remove>");
                    break;
            }
        }

        private static void StartGame(CommandArgs args)
        {
            int x = args.Player.TileX;
            int y = args.Player.TileY;
            int w = 20;
            int h = 4;
            string name = $"Cargobot{PanelIndex++}";

            Panel cargopanel = new Panel(name, x, y, w, h,
                style: new PanelStyle() { SaveSize = false },
                provider: FakeProviderAPI.CreateTileProvider(name, x, y, w, h));
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
                        else if (Games.Any(game => game.Playing && game.User == account2.ID))
                            player.SendErrorMessage("You are already plying this game.");
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

            Games.Add(cargoBot);
            TUI.Create(cargopanel);

            args.Player.SendSuccessMessage("Created new game instance.");
        }
    }
}
