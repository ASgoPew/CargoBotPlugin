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

        public Column(int x, int y, int max_boxes, int box_size, UIStyle style)
            : base(x, y, box_size, box_size * max_boxes, null, style)
        {
            BoxSize = box_size;
            SetupLayout(Alignment.Down, Direction.Up, Side.Left, null, 0, false);
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
