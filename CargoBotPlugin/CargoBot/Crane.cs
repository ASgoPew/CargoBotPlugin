using TerrariaUI.Base;
using TerrariaUI.Base.Style;

namespace CargoBot
{
    public class Crane : VisualObject
    {
        public int Column;
        public int Position;
        public int MaxColumns;
        public int MaxBoxes;
        public int BoxSize;
        public int BoxDelay;
        public int LeftBorder;
        public ushort TileType;
        public Box Box;

        public Crane(int x, int y, int max_columns, int max_boxes, int box_size, int box_delay, int left_border, ushort tile, UIStyle style)
            : base(x, y, box_size + 2, box_size + 2, null, style)
        {
            MaxColumns = max_columns;
            MaxBoxes = max_boxes;
            BoxSize = box_size;
            BoxDelay = box_delay;
            LeftBorder = left_border;
            TileType = tile;
            Box = null;
            Reset(0);
        }

        public void Reset(int column)
        {
            Column = column;
            SetXY(LeftBorder + (BoxSize + BoxDelay) * Column, 1, false);
            Position = 0;
            if (Box != null)
                Remove(Box);
            Box = null;
        }

        public void GameOver() =>
            GetAncestor<CargoBotGame>().EndGame(false);

        protected override (int, int) GetSizeNative() =>
            (4, Position + BoxSize + 2);

        protected override void ApplyThisNative()
        {
            base.ApplyThisNative();

            // Crane stalk
            for (int i = 0; i < Position + 2; i++)
                for (int j = 1; j < 1 + BoxSize; j++)
                {
                    var tile = Tile(j, i);
                    if (tile == null)
                        continue;
                    tile.active(true);
                    tile.type = TileType;
                }

            // Crane claw
            for (int i = Position + 1; i < Position + BoxSize + 2; i++)
            {
                var tile = Tile(0, i);
                if (tile != null)
                {
                    tile.active(true);
                    tile.type = TileType;
                }
                tile = Tile(Width - 1, i);
                if (tile != null)
                {
                    tile.active(true);
                    tile.type = TileType;
                }
            }
            Tile(0, Position + 1)?.slope(2);
            Tile(Width - 1, Position + 1)?.slope(1);
            Tile(0, Position + BoxSize + 1)?.slope(4);
            Tile(Width - 1, Position + BoxSize + 1)?.slope(3);

            // Crane box
            for (int x = 0; x < BoxSize; x++)
                for (int y = 0; y < BoxSize; y++)
                    Tile(1 + x, 2 + Position + y)?.active(false);

            // Clearing space after move_up
            if (Height > Position + BoxSize + 2)
                for (int y = Position + BoxSize + 2; y < GetMaxPosition(); y++)
                    for (int x = 0; x < Width; x++)
                        Tile(x, y)?.active(false);
        }

        public Column GetColumn() =>
            GetAncestor<Field>().Columns[Column];

        public int GetMaxPosition() =>
            (MaxBoxes - GetColumn().Count() + 1) * BoxSize + 1;

        public void AddBox(Box box)
        {
            Box = Add(new Box(1, 2 + Position, BoxSize, box.Style.Wall.Value, box.Color));
            Box.SetAlignmentInParent(Alignment.Down);
        }

        public void MoveDown()
        {
            int box_count = GetColumn().Count();
            int max_Position = GetMaxPosition();
            if (Box != null || box_count == 0)
                max_Position -= 2;
            if (Position == max_Position || (Box != null && Position == max_Position - 1))
            {
                GameOver();
                return;
            }
            Position = max_Position;
            Parent.UpdateChildSize();
            Box removed_box = null;
            if (Box == null)
            {
                if (box_count > 0)
                    AddBox(GetColumn().Pull());
            }
            else
            {
                Remove(Box);
                removed_box = GetColumn().Push(Box.Color);
                GetColumn().Update();
                Box = null;
            }
            Update().Apply();
            if (removed_box != null)
                removed_box.Apply();
            Draw();
        }

        public void MoveUp()
        {
            Position = 0;
            Apply();
            int old_height = Height;

            Parent.UpdateChildSize();
            if (Box != null)
            {
                Update();
                Box.Apply();
            }

            GetColumn().Apply();
            Draw(height: old_height);
        }

	    public void MoveRight()
        {
		    if (Column == MaxColumns - 1)
            {
                GameOver();
                return;
            }
            Column += 1;
		    foreach (var point in Points)
            {
                var tile = Tile(point.Item1, point.Item2);
                if (tile == null)
                    continue;
                tile.active(false);
                tile.wall = Style.Wall.Value;
                tile.wallColor(Style.WallColor.Value);
            }
            Move(BoxSize + BoxDelay, 0, true);
        }

        public void MoveLeft()
        {
            if (Column == 0)
            {
                GameOver();
                return;
            }
            Column -= 1;
            foreach (var point in Points)
            {
                var tile = Tile(point.Item1, point.Item2);
                if (tile == null)
                    continue;
                tile.active(false);
                tile.wall = Style.Wall.Value;
                tile.wallColor(Style.WallColor.Value);
            }
            Move(-(BoxSize + BoxDelay), 0, true);
        }
    }
}