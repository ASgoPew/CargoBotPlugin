using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TerrariaUI;

namespace CargoBot
{
    [ApiVersion(2, 1)]
    public class CargoBotPlugin : TerrariaPlugin
    {
        public override string Name => "CargoBot";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public override string Author => "ASgo & Anzhelika";
        public override string Description => "CargoBot TUI game";

        internal static UserSaver UserSaver = new UserSaver();
        public static ApplicationType ApplicationType;

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
        #region 21

        public static CargoBotLevel level_21 = new CargoBotLevel("Up The Greens", 1,
            new List<int[]>
            {
                new int[] { g },
                new int[] { b, b },
                new int[] { g },
                new int[] { },
                new int[] { b, b, b },
                new int[] { g },
                new int[] { b, b },
                new int[] { b, b },
            },
            new List<int[]>
            {
                new int[] { g, b, b },
                new int[] { },
                new int[] { g, b, b, b },
                new int[] { },
                new int[] { },
                new int[] { g, b, b, b, b },
                new int[] { },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, green_condition, no_condition, multi_condition },
            (12, 9, 7),
            "Very similar to the previous two levels but let the stack unwind and reset when you find a green. To do this only go left if holding a blue.\n\nThe shortest known solution uses 7 registers.");

        #endregion
        #region 22

        public static CargoBotLevel level_22 = new CargoBotLevel("Fill The Blanks", 1,
            new List<int[]>
            {
                new int[] { g, g, g, g },
                new int[] { r },
                new int[] { },
                new int[] { r },
                new int[] { },
                new int[] { },
                new int[] { r },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { r },
                new int[] { g },
                new int[] { r },
                new int[] { g },
                new int[] { g },
                new int[] { r },
                new int[] { g },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                red_condition, green_condition, no_condition, multi_condition },
            (20, 14, 11),
            "As in the \"Lay It Out\" level, move the entire pile one slot to the right and bring one crate back to the left, except in the first iteration.\n\nThe shortest known solution uses 11 registers.");

        #endregion
        #region 23

        public static CargoBotLevel level_23 = new CargoBotLevel("Count The Blues", 1,
            new List<int[]>
            {
                new int[] { y, b, b },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { y, b },
                new int[] { },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { b, b },
                new int[] { },
                new int[] { y },
                new int[] { },
                new int[] { b },
                new int[] { y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, yellow_condition, no_condition, multi_condition },
            (15, 12, 9),
            "Another stack puzzle. The number of blues indicates how many times to go right with the yellow.\n\nThe shortest known solution uses 9 registers.");

        #endregion
        #region 24

        public static CargoBotLevel level_24 = new CargoBotLevel("Multi Sort", 1,
            new List<int[]>
            {
                new int[] { },
                new int[] { b, y },
                new int[] { },
                new int[] { y, y, b },
                new int[] { y, b, y, b },
                new int[] { b, y },
                new int[] { b },
                new int[] { },
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
                new int[] { b, b, b, b, b, b },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, yellow_condition, no_condition, multi_condition },
            (16, 11, 11),
            "Come Together for yellows, The Stacker for blues. Go forward until you find a crate. If blue, move it one slot further and come all the way back (using the stack) empty handed. If yellow, bring it back and drop it. Repeat.\n\nThe shortest known solution uses 11 registers.");

        #endregion
        #region 25

        public static CargoBotLevel level_25 = new CargoBotLevel("Divide by two", 1,
            new List<int[]>
            {
                new int[] { b, b, b, b },
                new int[] { },
                new int[] { b, b },
                new int[] { },
                new int[] { b, b, b, b, b, b },
                new int[] { },
                new int[] { b, b, b, b },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { b, b },
                new int[] { b, b },
                new int[] { b },
                new int[] { b },
                new int[] { b, b, b },
                new int[] { b, b, b },
                new int[] { b, b },
                new int[] { b, b },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, no_condition },
            (20, 14, 12),
            "Wind up the stack for every two crates. Move one crate back each time it unwinds.\n\nThe shortest known solution uses 12 registers.");

        #endregion
        #region 26

        public static CargoBotLevel level_26 = new CargoBotLevel("The Merger", 1,
            new List<int[]>
            {
                new int[] { b, b, b },
                new int[] { },
                new int[] { r, r, r },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { b, r, b, r, b, r },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, red_condition, no_condition, multi_condition },
            (9, 7, 6),
            "Use the stack once in each blue, and unwind it in each red.\n\nThe shortest known solution uses 6 registers.");

        #endregion
        #region 27

        public static CargoBotLevel level_27 = new CargoBotLevel("Even the Odds", 1,
            new List<int[]>
            {
                new int[] { g, g, g, g, g },
                new int[] { },
                new int[] { r, r },
                new int[] { },
                new int[] { b, b, b },
                new int[] { },
                new int[] { y, y, y, y },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { g },
                new int[] { g, g, g, g },
                new int[] { },
                new int[] { r, r },
                new int[] { b },
                new int[] { b, b },
                new int[] { },
                new int[] { y, y, y, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, red_condition, green_condition, yellow_condition, no_condition, multi_condition },
            (13, 11, 10),
            "If the pile has an odd number of crates, leave one crate behind, otherwise move all of them. Use a sequence of moves that undoes itself when repeated to move the crates right, and make sure to execute it an even number of times.\n\nThe shortest known solution uses 10 registers.");

        #endregion
        #region 28

        public static CargoBotLevel level_28 = new CargoBotLevel("Genetic Code", 1,
            new List<int[]>
            {
                new int[] { g, y, y, g, y, g },
                new int[] { },
                new int[] { y, y, y },
                new int[] { },
                new int[] { g, g, g },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { g, y, g, y, y, g },
                new int[] { },
                new int[] { g, y, y, g, y, g },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                green_condition, yellow_condition, no_condition, multi_condition },
            (29, 20, 17),
            "The left pile gives instructions for how to construct the right pile. Wind up the entire stack on the left and unwind on the right.\n\nThe shortest known solution uses 17 registers.");

        #endregion
        #region 29

        public static CargoBotLevel level_29 = new CargoBotLevel("Multi Sort 2", 1,
            new List<int[]>
            {
                new int[] { },
                new int[] { b, y, r, g, y },
                new int[] { },
                new int[] { r, b, b, g, g, y },
                new int[] { },
                new int[] { r, g, y, r, b },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { b, b, b, b },
                new int[] { },
                new int[] { r, r, r, r },
                new int[] { },
                new int[] { g, g, g, g },
                new int[] { },
                new int[] { y, y, y, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, red_condition, green_condition, yellow_condition, no_condition, multi_condition },
            (25, 17, 17),
            "Go over each pile and either pick up conditional on none if over the even slots, or drop conditional on the corresponding color if over the odd slots.\n\nThe shortest known solution uses 17 registers.");

        #endregion
        #region 30

        public static CargoBotLevel level_30 = new CargoBotLevel("The Swap", 2,
            new List<int[]>
            {
                new int[] { r, r, r },
                new int[] { },
                new int[] { g, g, g },
            },
            new List<int[]>
            {
                new int[] { g, g, g },
                new int[] { },
                new int[] { r, r, r },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                red_condition, green_condition, no_condition, multi_condition },
            (15, 12, 10),
            "Merge the piles in the middle, change parity, and unmerge.\n\nThe shortest known solution uses 10 registers.");

        #endregion
        #region 31

        public static CargoBotLevel level_31 = new CargoBotLevel("Restoring Order", 1,
            new List<int[]>
            {
                new int[] { },
                new int[] { b, r, b, b },
                new int[] { r, b, r, b },
                new int[] { b, b, b },
                new int[] { r },
                new int[] { r, b },
                new int[] { b },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { b, b, b },
                new int[] { b, b },
                new int[] { b, b, b },
                new int[] { },
                new int[] { b },
                new int[] { b },
                new int[] { r, r, r, r, r },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, red_condition, no_condition, multi_condition },
            (29, 20, 16),
            "For each pile move the reds one slot to the right and the blues one slot to the left, but make sure to wind up a stack for the blues so that you can put them back afterwards. Repeat for each pile.\n\nThe shortest known solution uses 16 registers.");

        #endregion
        #region 32

        public static CargoBotLevel level_32 = new CargoBotLevel("Changing  Places", 1,
            new List<int[]>
            {
                new int[] { r },
                new int[] { r, r, r },
                new int[] { g, g, g },
                new int[] { },
                new int[] { r, r, r, r },
                new int[] { r, r },
                new int[] { g, g, g, g },
                new int[] { g },
            },
            new List<int[]>
            {
                new int[] { r, r, r },
                new int[] { r },
                new int[] { },
                new int[] { g, g, g },
                new int[] { r, r },
                new int[] { r, r, r, r },
                new int[] { g },
                new int[] { g, g, g, g },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                red_condition, green_condition, no_condition, multi_condition },
            (20, 18, 17),
            "Switch each pair of piles, in place. First move the left pile to the right, winding up the stack. Then move all crates to the left slot. Finally, unwind the stack moving a crate to the right each time.\n\nThe shortest known solution uses 17 registers.");

        #endregion
        #region 33

        public static CargoBotLevel level_33 = new CargoBotLevel("Palette Swap", 2,
            new List<int[]>
            {
                new int[] { },
                new int[] { r, b },
                new int[] { b, r, b, r },
                new int[] { b, r },
                new int[] { b, r, b, r },
                new int[] { },
                new int[] { b, r, b, r, b, r },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { b, r },
                new int[] { r, b, r, b },
                new int[] { r, b },
                new int[] { r, b, r, b },
                new int[] { },
                new int[] { r, b, r, b, r, b },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, red_condition, no_condition, multi_condition },
            (29, 18, 15),
            "Go left and go right. Each time you do so, wind up the stack. When no more crates are left, unwind the stack going left and going right. Repeat. \n\nThe shortest known solution uses 15 registers.");

        #endregion
        #region 34

        public static CargoBotLevel level_34 = new CargoBotLevel("Mirror 2", 1,
            new List<int[]>
            {
                new int[] { y, y, y },
                new int[] { y, y },
                new int[] { y },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { y },
                new int[] { y, y },
                new int[] { y, y, y },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                yellow_condition, no_condition },
            (20, 15, 12),
            "Move the top crate of the 2nd pile one slot to the right, and bring the left pile all the way to the right.\n\nThe shortest known solution uses 12 registers.");

        #endregion
        #region 35

        public static CargoBotLevel level_35 = new CargoBotLevel("Changing Places 2", 1,
            new List<int[]>
            {
                new int[] { r },
                new int[] { r, r, r },
                new int[] { r },
                new int[] { r, r, r, r, r },
                new int[] { },
                new int[] { r, r },
                new int[] { r, r, r, r },
                new int[] { r, r, r },
            },
            new List<int[]>
            {
                new int[] { r, r, r },
                new int[] { r },
                new int[] { r, r, r, r, r },
                new int[] { },
                new int[] { r, r },
                new int[] { r, r, r, r },
                new int[] { r, r, r },
                new int[] { r },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                red_condition, no_condition },
            (25, 19, 16),
            "As in Changing Places, swap piles. Do that once for each pair of consecutive piles and you're done.\n\nThe shortest known solution uses 16 registers.");

        #endregion
        #region 36

        public static CargoBotLevel level_36 = new CargoBotLevel("Vertical Sort", 2,
            new List<int[]>
            {
                new int[] { },
                new int[] { g, b, g, b, b },
                new int[] { b, g, b },
                new int[] { g, b, b, g },
                new int[] { b, g },
                new int[] { b, g, g, g, b },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { },
                new int[] { g, g, b, b, b },
                new int[] { g, b, b },
                new int[] { g, g, b, b },
                new int[] { g, b },
                new int[] { g, g, g, b, b },
                new int[] { },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, green_condition, no_condition, multi_condition },
            (29, 29, 20),
            "Draw on ideas from previous sort levels.");

        #endregion
        #region 37

        public static CargoBotLevel level_37 = new CargoBotLevel("Count in Binary", 1,
            new List<int[]>
            {
                new int[] { g, g, g, g, g, g },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { g, g },
                new int[] { },
                new int[] { g },
                new int[] { g },
                new int[] { g },
                new int[] { },
                new int[] { g },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                green_condition, no_condition },
            (29, 23, 17),
            "Count up all the numbers in binary: 1, 10, 11, 100,...");

        #endregion
        #region 38

        public static CargoBotLevel level_38 = new CargoBotLevel("Parting the Sea", 1,
            new List<int[]>
            {
                new int[] { },
                new int[] { b, b },
                new int[] { b, b },
                new int[] { b, b },
                new int[] { b, b },
                new int[] { b, b },
                new int[] { },
            },
            new List<int[]>
            {
                new int[] { b, b, b, b, b },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { },
                new int[] { b, b, b, b, b },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                blue_condition, no_condition },
            (17, 17, 17),
            "Nothing.");

        #endregion
        #region 39

        public static CargoBotLevel level_39 = new CargoBotLevel("The Trick", 2,
            new List<int[]>
            {
                new int[] { y, r },
                new int[] { },
                new int[] { r, y },
            },
            new List<int[]>
            {
                new int[] { r, y },
                new int[] { },
                new int[] { y, r },
            },
            new int[] { right_arrow, down_arrow, left_arrow, empty, f1, f2, f3, f4,
                red_condition, yellow_condition, no_condition, multi_condition },
            (20, 14, 11),
            "Bring the right pile to the middle, then the left pile to the middle. Finally unmerge the piles to their respective sides. \n\nThe shortest known solution uses 11 registers.");

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
                        { "Up The Greens", level_21 },
                        { "Fill The Blanks", level_22 },
                        { "Count The Blues", level_23 },
                        { "Multi Sort", level_24 },
                    }
                },
                { "Crazy", new Dictionary<string, CargoBotLevel>()
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
                },
                { "Unknown", new Dictionary<string, CargoBotLevel>()
                    {
                        { "Count in Binary", level_37 },
                        { "Parting the Sea", level_38 },
                        { "The Trick", level_39 },
                    }
                }
            };

        #endregion

        public override void Initialize()
        {
            ApplicationType = new ApplicationType("cargobot", (name) => new CargoBotApplication(name));
            TUI.RegisterApplication(ApplicationType);
        }

        public static IEnumerable<CargoBotGame> Games => ApplicationType.IterateInstances
            .Select(instance => ((CargoBotApplication)instance.Value).Game);

        public static CargoBotGame GameByUser(int user) => Games.Where(game => game.User == user).FirstOrDefault();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
    }
}
