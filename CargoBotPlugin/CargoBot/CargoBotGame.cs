using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Widgets;
using TShockAPI;

namespace CargoBot
{
	public class CargoBotGame : VisualContainer
	{
		private static object StaticLocker = new object();

		public Field Field;
		public List<SlotLine> Lines;
		public Label ToolboxLabel;
		public ToolBox Toolbox;
		public Button RunButton;
		public Button HintButton;
		public Checkbox SpeedCheckbox;
		public Button ExitButton;
		public bool Playing;
		public int PlayingIndex = 0;
		public int RunLine;
		public int RunSlot;
		public int RunDelay;
		public CargoBotLevel Level;
		public Slot OldSlot;
		public TSPlayer Player;
		public int User;

		public bool Fast => RunDelay == 300;

		public CargoBotGame(int x, int y)
			: base(x, y, 56, 47, new UIConfiguration() { UseEnd = true, BeginRequire = false },
				  new ContainerStyle() { Wall = 155 })
		{
			Field = Add(new Field(1, 1, 8, 6, 2, 2, 2, 1, TileID.EmeraldGemspark,
				PaintID2.Shadow, new UIStyle() { Wall = 155, WallColor = PaintID2.White }));
			Add(new VisualObject(1, Field.Height + 5, 4, 22, new UIConfiguration() { UseBegin = false },
				new UIStyle() { Wall = WallID.SapphireGemspark }));

			Lines = new List<SlotLine>();
			for (int i = 0; i < 4; i++)
				Lines.Add(Add(new SlotLine(1, Field.Height + 4 + i * 6, i, i < 3 ? 8 : 5)));

			ToolboxLabel = Add(new Label(39, 21, 16, 2, "toolbox"));
			Toolbox = Add(new ToolBox(39, 24, 4, 4));

			RunButton = Add(new Button(27, 42, 8, 4, "run", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156 },
				(self, t) => Run()));

			HintButton = Add(new Button(36, 42, 10, 4, "hint", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.Gray },
				(self, t) => TShock.Players[t.PlayerIndex]?.SendInfoMessage($"Hint: {Level.Hint}")));

			SpeedCheckbox = Add(new Checkbox(47, 43, 2, new CheckboxStyle()
				{ Wall = 156, WallColor = PaintID2.Gray, CheckedColor = PaintID2.DeepOrange },
				new Input<bool>(false, false, (self, value, player) => RunDelay = value ? 300 : 600)));

			ExitButton = Add(new Button(51, 42, 4, 4, "x", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.DeepRed },
				(self, t) => Stop()));

			Playing = false;
			RunLine = 0;
			RunSlot = 0;
			RunDelay = 600;

			Level = null;
			OldSlot = Lines[0].Slots[0];
		}

		public override void Invoke(Touch touch)
		{
			if (touch.State == TouchState.End)
			{
				var _begin_slot = touch.Session.BeginTouch.Object;
				if (_begin_slot is Slot begin_slot && begin_slot.WithCondition)
				{
					if (touch.Session.BeginTouch.Y > 0)
						begin_slot.Value = 0;
					else
						begin_slot.Condition = null;
					begin_slot.Apply().Draw();
				}
			}
			else
				((Panel)Parent).Unsummon();
		}

        public void Run()
		{
			lock (StaticLocker)
				if (Playing)
                {
					Reset();
					Level.LoadField(this);
					Apply().Draw();
                }
				else
				{
					Playing = true;
					PlayingIndex++;
					RunMove();
				}
		}

		public void Reset()
		{
			Playing = false;
			RunLine = 0;
			RunSlot = 0;
			OldSlot.Style.WallColor = null;
			OldSlot = Lines[0].Slots[0];
		}

		public void GameOver()
		{
			Player.SendSuccessMessage("You lost...");
			int playingIndex = PlayingIndex;
			Task.Delay(3000).ContinueWith(_ =>
			{
				if (Playing && PlayingIndex == playingIndex)
				{
					Reset();
					Level.LoadField(this);
					Apply().Draw();
				}
			});
		}

		public void Win()
		{
			Player.SendSuccessMessage("You won the game!");
			int playingIndex = PlayingIndex;
			Task.Delay(3000).ContinueWith(_ =>
			{
				if (Playing && PlayingIndex == playingIndex)
                {
					Reset();
					Level.LoadField(this);
					Apply().Draw();
				}
			});
		}

		public void RunMove()
		{
			lock (StaticLocker)
			{
				if (Disposed || !Playing)
					return;
				var value = PullAction();
				RunAction(value);
				if (value == 2 && Field.Crane.Box == null && Field.CheckWin())
                {
					Win();
					return;
                }
				else if (RunSlot == Lines[RunLine].Slots.Count // Checking if there is no next slot
					|| Lines[RunLine].Slots.Skip(RunSlot).All(slot => slot.Value == 0))
                {
					GameOver();
					return;
                }
				if (Playing)
					Task.Delay(RunDelay).ContinueWith(_ => RunMove());
			}
		}

		public int? PullAction()
		{
			Slot slot = Lines[RunLine].Slots[RunSlot];
			while (slot.Value == 0)
				slot = Lines[RunLine].Slots[++RunSlot];

			// Disabling old slot selection
			OldSlot.Style.WallColor = null;
			OldSlot.Apply().Draw();
			OldSlot = slot;

			// Enabling new slot selection
			slot.Style.WallColor = PaintID2.White;
			slot.Apply().Draw();

			// Checking if a condition
			bool condition_fit = true;
			if (slot.Condition != null)
			{
				var box = Field.Crane.Box;
				if ((slot.Condition < 4 && (box == null || box.Color != slot.Condition)
						|| slot.Condition == 4 && box != null
						|| slot.Condition == 5 && box == null))
					condition_fit = false;
			}

			// Moving current slot
			RunSlot += 1;
			if (condition_fit)
				if (slot.Value >= 4 && slot.Value < 8)
				{
					RunLine = slot.Value - 4;
					RunSlot = 0;
				}

			if (condition_fit)
				return slot.Value;
			return null;
		}

		public void RunAction(int? action)
		{
			if (action.HasValue && action.Value > 0)
			{
				if (action == 1)
					Field.Crane.MoveRight();
				else if (action == 2)
				{
					Field.Crane.MoveDown();
					Task.Delay(RunDelay / 2).ContinueWith(_ => Field.Crane.MoveUp());
				}
				else if (action == 3)
					Field.Crane.MoveLeft();
			}
		}

		public void LoadLevel(CargoBotLevel level, TSPlayer player, int user)
		{
			Player = player;
			User = user;

			Level = level;
			Level.UDBRead(User);
			Level.LoadTools(this);
		}

		public void Stop()
        {
			Level.UDBWrite(User);
			Reset();
			((Panel)Root).UnsummonAll();
		}
	}
}
