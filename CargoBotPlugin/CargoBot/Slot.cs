using Terraria.ID;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Widgets;

namespace CargoBot
{
    public class Slot : VisualObject
    {
        public static ushort[] Walls = { WallID.RubyGemspark, WallID.AmethystGemspark };
        public static byte[] ConditionColors = { PaintID2.DeepRed, PaintID2.DeepBlue, PaintID2.DeepLime, PaintID2.DeepYellow, PaintID2.Gray };
		private static object StaticLocker = new object();
		private static int FrameF = Label.StatueTextFrame('f');
		private static int[] FrameN = new int[]
        {
			Label.StatueTextFrame('1'),
			Label.StatueTextFrame('2'),
			Label.StatueTextFrame('3'),
			Label.StatueTextFrame('4')
		};

		public int Value;
		public int? Condition;
		public int Index;
		public bool WithCondition;
		public ushort? ParentWall;
		public byte? ParentWallColor;

		public Slot(int x, int y, int value, int? condition, int index, bool withCondition = true)
			: base(x, y, 4, withCondition ? 5 : 4,
			new UIConfiguration()
			{
				UseEnd = withCondition,
				SessionAcquire = false,
				UseOutsideTouches = true,
				BeginRequire = false
			})
		{
			Value = value;
			Condition = condition;
			Index = index % 2;
			WithCondition = withCondition;
			DrawWithSection = true;
		}

		protected override void UpdateThisNative()
		{
			base.UpdateThisNative();

			var collected = CollectStyle(false);
			ParentWall = collected.Wall;
			ParentWallColor = collected.WallColor;
		}

		protected override void ApplyThisNative()
		{
			ApplyValue();

			base.ApplyThisNative();

			if (WithCondition)
				ApplyCondition();
		}

		public void ApplyValue()
        {
			var y_range = WithCondition ? Height - 1 : Height;
			var dy = WithCondition ? 1 : 0;

			for (int x = 0; x < Width; x++)
				for (int y = 0; y < y_range; y++)
				{
					var tile = Tile(x, y + dy);
					if (tile == null)
						continue;
					tile.ClearEverything();
					tile.wall = Walls[Index];
					tile.wallColor((byte)PaintID2.Gray);
					if ((Value == 1 && (y == 1 || y == 2 || x == 2))
							|| (Value == 2 && (x == 1 || x == 2 || y == 2))
							|| (Value == 3 && (y == 1 || y == 2 || x == 1)))
					{
						tile.active(true);
						tile.inActive(true);
						tile.type = TileID.DiamondGemspark;
						if (Value == 1 && (x == 2 && y == 0 || x == 3 && y == 1))
							tile.slope((byte)1);
						else if (Value == 3 && (x == 1 && y == 0 || x == 0 && y == 1))
							tile.slope((byte)2);
						else if ((Value == 1 || Value == 2) && (x == 3 && y == 2 || x == 2 && y == 3))
							tile.slope((byte)3);
						else if ((Value == 2 || Value == 3) && (x == 0 && y == 2 || x == 1 && y == 3))
							tile.slope((byte)4);
					}
					else if (Value >= 4 && Value < 8 && y > 0 && y < 3)
					{
						tile.active(true);
						tile.inActive(true);
						tile.type = (ushort)TileID.AlphabetStatues;
						tile.color((byte)PaintID2.Gray);
						tile.frameX = (short)(x < 2
							? (FrameF + x * 18)
							: (FrameN[Value - 4] + (x - 2) * 18));
						tile.frameY = (short)((y - 1) * 18);
					}
					else if (Value >= 8 && Value < 12 && y > 0 && y < 3)
						tile.wallColor((byte)ConditionColors[Value - 8]);
					else if (Value == 12 && y > 0 && y < 3)
						tile.wallColor((byte)PaintID2.Black);
					else if (Value == 13 && y > 0 && y < 3)
						tile.wallColor((byte)ConditionColors[x]);
				}
		}

		public void ApplyCondition()
		{
			for (int x = 0; x < Width; x++)
			{
				var tile = Tile(x, 0);
				if (tile == null)
					continue;
				if (Condition.HasValue)
				{
					tile.wall = (ushort)Walls[1 - Index];
					if (Condition.Value < 5)
						tile.wallColor((byte)ConditionColors[Condition.Value]);
					else if (Condition.Value == 5)
						tile.wallColor((byte)ConditionColors[x]);
				}
				else
				{
					if (ParentWall.HasValue)
						tile.wall = (ushort)ParentWall.Value;
					if (ParentWallColor.HasValue)
						tile.wallColor((byte)ParentWallColor.Value);
				}
			}
		}

		public override void Invoke(Touch touch)
		{
			if (touch.State == TouchState.End && !GetAncestor<CargoBotGame>().Running)
			{
				var _begin_slot = touch.Session.BeginTouch.Object;
				if (!(_begin_slot is Slot begin_slot) || begin_slot.Root != Root)
					return;

				if (begin_slot.WithCondition)
				{
					if (touch.Session.BeginTouch.Y > 0)
					{
						if (begin_slot.Value == 0)
							return;
						int c = Value;
						Value = begin_slot.Value;
						begin_slot.Value = c;
					}
					else
					{
						if (!begin_slot.Condition.HasValue)
							return;
						int? c = null;
						if (Condition.HasValue)
							c = Condition.Value;
						Condition = begin_slot.Condition;
						begin_slot.Condition = c;
					}
					Apply().Draw();
					begin_slot.Apply().Draw();
				}
				else
				{
					if (begin_slot.Value < 8)
					{
						if (begin_slot.Value == 0)
							return;
						Value = begin_slot.Value;
					}
					else
						Condition = begin_slot.Value - 8;
					Apply().Draw();
				}
			}
		}
	}
}
