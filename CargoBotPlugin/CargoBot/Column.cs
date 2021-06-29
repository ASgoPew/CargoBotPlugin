using System.Collections.Generic;
using System.Linq;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;

namespace CargoBot
{
    public class Column : VisualObject
    {
        public int BoxSize { get; set; }
        public List<Box> Boxes { get; set; } = new List<Box>();

        public Column(int x, int y, int maxBoxes, int boxSize, UIStyle style = null)
            : base(x, y, boxSize, boxSize * maxBoxes, null, style)
        {
            BoxSize = boxSize;
            SetupLayout(Alignment.Down, Direction.Up, Side.Left, null, 0);
        }

        public void Reset()
        {
            foreach (var box in Boxes)
                Remove(box);
            Boxes.Clear();
        }

        public Box Push(byte color)
        {
            ushort wall = Box.Walls[ChildCount % 2 == 0 ? 0 : 1];
            Box box = AddToLayout(new Box(0, 0, BoxSize, wall, color));
            Boxes.Add(box);
            return box;
        }

        public Box Pull()
        {
            Box box = Boxes.Last();
            Boxes.Remove(box);
            Remove(box);
            return box;
        }

        public int Count() =>
            ChildCount;
    }
}
