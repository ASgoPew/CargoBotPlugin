using TerrariaUI.Base;
using TerrariaUI.Base.Style;

namespace CargoBot
{
    public class Box : VisualObject
    {
        public static ushort[] Walls = { 164, 154 };
        public static byte[] Colors = { PaintID2.DeepRed, PaintID2.DeepBlue, PaintID2.DeepLime, PaintID2.DeepYellow };

        public byte Color;

        public Box(int x, int y, int size, ushort wall, byte color)
            : base(x, y, size, size, null,
            new UIStyle() { Wall = wall, WallColor = Colors[color] })
        {
            Color = color;
        }
    }
}
