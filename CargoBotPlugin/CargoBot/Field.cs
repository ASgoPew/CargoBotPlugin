using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;

namespace CargoBot
{
	public class Field : VisualObject
	{
		public int Divider;
		public ushort TileType;
		public byte TileColor;
		public Crane Crane;
		public List<Column> Columns;
		public List<Column> ResultColumns;
		public List<(int, int)> Border;

		public Field(int x, int y, int max_columns, int max_boxes, int box_size, int box_delay,
			int left_border, int right_border, ushort tile, byte tile_color, UIStyle style)
			: base(x, y, max_columns * (box_size + 1) + (max_columns - 1) * (box_delay + 1) + 3 +
				  2 * left_border + 2 * right_border,
				  max_boxes * box_size + box_size + 5, null, style)
		{
			Divider = 1 + 2 * left_border + max_columns * box_size + (max_columns - 1) * box_delay;
			TileType = tile;
			TileColor = tile_color;

			var crane_style = new UIStyle() { Wall = Style.Wall, WallColor = Style.WallColor, TileColor = PaintID2.Brown };
			Crane = Add(new Crane(0, 0, max_columns, max_boxes, box_size, box_delay, left_border, tile, crane_style));

			var column_style = new UIStyle() { Wall = Style.Wall, WallColor = Style.WallColor };
			Columns = new List<Column>();
			for (int i = 0; i < max_columns; i++)
				Columns.Add(Add(new Column(1 + left_border + i * (box_size + box_delay), 4 + box_size,
					max_boxes, box_size, column_style)));
			ResultColumns = new List<Column>();
			for (int i = 0; i < max_columns; i++)
				ResultColumns.Add(Add(new Column(1 + 2 * left_border + right_border + max_columns * box_size +
					(max_columns - 1) * box_delay + 1 + 2 * i,
					Height - 1 - max_boxes, max_boxes, 1, column_style)));

			Border = new List<(int, int)>();
			for (x = 0; x < Width; x++)
				Border.Add((x, 0));
			for (x = 0; x < Width; x++)
				Border.Add((x, Height - 1));
			for (y = 0; y < Height; y++)
				Border.Add((0, y));
			for (y = 0; y < Height; y++)
				Border.Add((Divider, y));
			for (y = 0; y < Height; y++)
				Border.Add((Width - 1, y));

			DrawWithSection = true;
		}

		protected override void ApplyThisNative()
		{
			base.ApplyThisNative();

			foreach (var point in Border)
			{
				var tile = Tile(point.Item1, point.Item2);
				if (tile == null)
					continue;
				tile.active(true);
				tile.type = TileType;
				tile.color(TileColor);
				tile.inActive(true);
			}
		}

		public bool CheckWin()
		{
			for (int i = 0; i < Columns.Count; i++)
			{
				var column = Columns[i];
				var result_column = ResultColumns[i];
				if (column.Boxes.Count != result_column.Boxes.Count)
					return false;
				for (int j = 0; j < column.Boxes.Count; j++)
					if (column.Boxes[j].Color != result_column.Boxes[j].Color)
						return false;
			}
			return true;
		}
	}
}
