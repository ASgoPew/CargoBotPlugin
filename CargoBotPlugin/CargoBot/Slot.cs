using OTAPI.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Widgets.Media;
using static System.Net.Mime.MediaTypeNames;

namespace CargoBot
{
    public class Slot : VisualObject
    {
        public static ushort[] Walls = { WallID.RubyGemspark, WallID.AmethystGemspark };
        public static byte[] ConditionColors = { PaintID2.DeepRed, PaintID2.DeepBlue, PaintID2.DeepLime, PaintID2.DeepYellow, PaintID2.Gray };
        public static ITile[,] Image = null;
		private static object StaticLocker = new object();

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

		protected override void LoadThisNative()
        {
			lock (StaticLocker)
				if (Image == null)
                    Image = ImageData.LoadImage(CargoBotPlugin.ImagePath).Tiles;
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
			var image = Image;
			var y_range = WithCondition ? Height - 1 : Height;
			var dy = WithCondition ? 1 : 0;
			var image_dx = 4 * Value;
			var image_dy = Index == 0 ? 0 : 4;
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < y_range; y++)
					Tile(x, y + dy).CopyFrom(image[x + image_dx, y + image_dy]);

			base.ApplyThisNative();

			if (WithCondition)
				ApplyCondition();
		}

		public void ApplyCondition()
		{
			for (int x = 0; x < Width; x++)
			{
				var tile = Tile(x, 0);
				if (tile != null)
					if (Condition.HasValue)
					{
						tile.wall = Walls[1 - Index];
						if (Condition.Value < 5)
							tile.wallColor(ConditionColors[Condition.Value]);
						else if (Condition.Value == 5)
							tile.wallColor(ConditionColors[x]);
					}
					else
					{
						if (ParentWall.HasValue)
							tile.wall = ParentWall.Value;
						if (ParentWallColor.HasValue)
							tile.wallColor(ParentWallColor.Value);
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
