using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;

namespace CargoBot
{
    public class Field : VisualObject
	{
		public int MaxColumns;
		public int MaxBoxes;
		public int BoxSize;
		public int BoxDelay;
		public int LeftBorder;
		public int RightBorder;

		public int ColumnsCount;
		public int ColumnsX;
		public int ResultColumnsX;

		public int Divider;
		public ushort BorderTileType;
		public byte BorderTileColor;
		public ushort BoxPlaceTileType;
		public byte BoxPlaceTileColor;
		public Crane Crane;
		public List<Column> Columns;
		public List<Column> ResultColumns;
		public List<(int, int)> Border;

		public CargoBotGame Game => GetAncestor<CargoBotGame>();

		public Field(int x, int y, int maxColumns, int maxBoxes, int boxSize, int boxDelay,
			int leftBorder, int rightBorder, ushort borderTileType, byte borderTileColor, ushort boxPlaceTileType,
			byte boxPlaceTileColor, UIStyle style)
			: base(x, y, maxColumns * (boxSize + 1) + (maxColumns - 1) * (boxDelay + 1) + 3 +
				  2 * leftBorder + 2 * rightBorder,
				  maxBoxes * boxSize + boxSize + 6, null, style)
		{
			MaxColumns = maxColumns;
			MaxBoxes = maxBoxes;
			BoxSize = boxSize;
			BoxDelay = boxDelay;
			LeftBorder = leftBorder;
			RightBorder = rightBorder;

			Divider = 1 + 2 * LeftBorder + MaxColumns * BoxSize + (MaxColumns - 1) * BoxDelay;
			BorderTileType = borderTileType;
			BorderTileColor = borderTileColor;
			BoxPlaceTileType = boxPlaceTileType;
			BoxPlaceTileColor = boxPlaceTileColor;

			ColumnsX = 1 + LeftBorder;
			var craneStyle = new UIStyle() { Wall = Style.Wall, WallColor = Style.WallColor, TileColor = PaintID2.Brown };
			Crane = Add(new Crane(0, 0, MaxColumns, MaxBoxes, BoxSize, BoxDelay, LeftBorder, borderTileType, craneStyle));

			var columnStyle = new UIStyle() { Wall = Style.Wall, WallColor = Style.WallColor };
			Columns = new List<Column>();
			for (int i = 0; i < MaxColumns; i++)
				Columns.Add(Add(new Column(1 + LeftBorder + i * (BoxSize + BoxDelay),
					Height - 2 - MaxBoxes * BoxSize, MaxBoxes, BoxSize, columnStyle)));
			ResultColumns = new List<Column>();
			for (int i = 0; i < MaxColumns; i++)
				ResultColumns.Add(Add(new Column(1 + 2 * LeftBorder + RightBorder + MaxColumns * BoxSize +
					(MaxColumns - 1) * BoxDelay + 1 + 2 * i,
					Height - 2 - MaxBoxes * 1, MaxBoxes, 1, columnStyle)));

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

        protected override void UpdateThisNative()
        {
            base.UpdateThisNative();

			if (Game.Level == null)
				return;

			ColumnsCount = Game.Level.Columns.Count();
			int space = MaxColumns * BoxSize + (MaxColumns - 1) * BoxDelay;
			ColumnsX = 1 + LeftBorder + (space - (ColumnsCount * BoxSize + (ColumnsCount - 1) * BoxDelay)) / 2;
			int resultSpace = MaxColumns * 1 + (MaxColumns - 1) * 1;
			ResultColumnsX = 1 + LeftBorder * 2 + space + 1 + RightBorder +
				(resultSpace - (ColumnsCount * 1 + (ColumnsCount - 1) * 1)) / 2;

			Crane.SetXY(ColumnsX - 1, 1, false);

			int x = ColumnsX;
			int y = Height - 2 - MaxBoxes * BoxSize;
			for (int i = 0; i < MaxColumns; i++)
				if (i < ColumnsCount)
				{
					Columns[i].Enable(false);
					Columns[i].SetXY(x, y, false);
					x += BoxSize + BoxDelay;
				}
				else
					Columns[i].Disable(false);

			x = ResultColumnsX;
			y = Height - 2 - MaxBoxes * 1;
			for (int i = 0; i < MaxColumns; i++)
				if (i < ColumnsCount)
				{
					ResultColumns[i].Enable(false);
					ResultColumns[i].SetXY(x, y, false);
					x += 1 + 1;
				}
				else
					ResultColumns[i].Disable(false);
		}

        protected override void ApplyThisNative()
		{
			base.ApplyThisNative();

			// Draw border of tiles
			foreach (var point in Border)
			{
				var tile = Tile(point.Item1, point.Item2);
				if (tile == null)
					continue;
				tile.active(true);
				tile.type = BorderTileType;
				tile.color(BorderTileColor);
				tile.inActive(true);
			}

			// Stop walls
			if (ColumnsCount != MaxColumns && Game.Level != null)
				for (int i = 1; i < Height - 1; i++)
				{
					var tile = Tile(ColumnsX - 3, i);
					tile.active(true);
					tile.inActive(true);
					tile.type = TileID.SlimeBlock;
					tile.color(PaintID2.DeepRed);
					tile = Tile(ColumnsX + ColumnsCount * BoxSize + (ColumnsCount - 1) * BoxDelay + 2, i);
					tile.active(true);
					tile.inActive(true);
					tile.type = TileID.SlimeBlock;
					tile.color(PaintID2.DeepRed);
				}

			// Mark places for box columns
			int x = ColumnsX;
			int y = Height - 2;
			byte color = PaintID2.White;
			for (int i = 0; i < ColumnsCount; i++)
			{
				for (int j = 0; j < BoxSize; j++)
                {
					var tile = Tile(x + j, y);
					if (tile == null)
						continue;
					tile.active(true);
					tile.inActive(true);
					tile.type = BoxPlaceTileType;
					tile.color(BoxPlaceTileColor);
					//if (j == 0)
						//tile.slope(2);
					//else if (j == BoxSize - 1)
						//tile.slope(1);
				}
				x += BoxSize + BoxDelay;
			}
			x = ResultColumnsX;
			for (int i = 0; i < ColumnsCount; i++)
			{
				var tile = Tile(x, y);
				if (tile == null)
					continue;
				tile.active(true);
				tile.inActive(true);
				tile.type = BoxPlaceTileType;
				tile.color(BoxPlaceTileColor);
				x += 2;
			}
		}

		public bool CheckWin()
		{
			for (int i = 0; i < Columns.Count; i++)
			{
				var column = Columns[i];
				var resultColumn = ResultColumns[i];
				if (column.Boxes.Count != resultColumn.Boxes.Count)
					return false;
				for (int j = 0; j < column.Boxes.Count; j++)
					if (column.Boxes[j].Color != resultColumn.Boxes[j].Color)
						return false;
			}
			return true;
		}
	}
}
