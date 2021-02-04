using System;
using System.Collections.Generic;
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
		public Button ExitButton;
		public bool Playing;
		public int RunLine;
		public int RunSlot;
		public int RunDelay;
		public CargoBotLevel Level;
		public Slot OldSlot;

		public CargoBotGame(int x, int y)
			: base(x, y, 56, 47, new UIConfiguration() { UseEnd = true, BeginRequire = false },
				  new ContainerStyle() { Wall = 155 })
		{
			this.Field = this.Add(new Field(1, 1, 8, 6, 2, 2, 2, 1, TileID.EmeraldGemspark,
				PaintID2.Shadow, new UIStyle() { Wall = 155, WallColor = 26 }));
			this.Add(new VisualObject(1, this.Field.Height + 5, 4, 22, new UIConfiguration() { UseBegin = false },
				new UIStyle() { Wall = WallID.SapphireGemspark }));

			this.Lines = new List<SlotLine>();
			for (int i = 0; i < 4; i++)
				this.Lines.Add(this.Add(new SlotLine(1, this.Field.Height + 4 + i * 6, i, i < 3 ? 8 : 5)));

			this.ToolboxLabel = this.Add(new Label(39, 21, 16, 2, "toolbox"));
			this.Toolbox = this.Add(new ToolBox(39, 24, 4, 4));

			this.RunButton = this.Add(new Button(27, 42, 8, 4, "run", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156 },
				(self, t) => this.Run()));

			this.HintButton = this.Add(new Button(36, 42, 10, 4, "hint", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.Gray },
				(self, t) => TShock.Players[t.PlayerIndex]?.SendInfoMessage($"Hint: {Level.Hint}")));

			this.ExitButton = this.Add(new Button(51, 42, 4, 4, "x", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.DeepRed },
				(self, t) => ((Panel)Root).UnsummonAll()));

			this.Playing = false;
			this.RunLine = 0;
			this.RunSlot = 0;
			this.RunDelay = 1000;

			this.Level = null;
			this.OldSlot = this.Lines[0].Slot[0];
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
				if (this.Playing)
					this.Reset();
				else
				{
					this.Playing = true;
					this.RunMove();
				}
		}

		public void Reset()
		{
			this.Playing = false;
			this.RunLine = 0;
			this.RunSlot = 0;
			this.Field.LoadLevel(this.Level);
			this.Field.Apply().Draw();
			this.OldSlot.Style.WallColor = null;
			this.OldSlot.Apply().Draw();
			this.OldSlot = this.Lines[0].Slot[0];
		}

		public void GameOver()
		{
			this.Reset();
			Console.WriteLine("GAME OVER");
		}

		public void Win()
		{
			Console.WriteLine("YOU WON!!!");
			Task.Delay(3000).ContinueWith(_ =>
			{
				if (this.Playing)
					this.Reset();
			});
		}

		public void RunMove()
		{
			lock (StaticLocker)
			{
				if (this.Disposed || !this.Playing)
					return;
				var value = this.PullAction();
				this.RunAction(value);
				if (value == 2 && this.Field.Crane.Box == null && this.Field.CheckWin())
                {
					this.Win();
					return;
                }
				else if (this.RunSlot + 1 == this.Lines[this.RunLine].Slot.Count) // Checking if there is no next slot
                {
					this.GameOver();
					return;
                }
				if (this.Playing)
					Task.Delay(this.RunDelay).ContinueWith(_ => this.RunMove());
			}
		}

		public int? PullAction()
		{
			var slot = this.Lines[this.RunLine].Slot[this.RunSlot];

			// Disabling old slot selection
			this.OldSlot.Style.WallColor = null;
			this.OldSlot.Apply().Draw();
			this.OldSlot = slot;

			// Enabling new slot selection
			slot.Style.WallColor = PaintID2.White;
			slot.Apply().Draw();

			// Checking if a condition
			bool condition_fit = true;
			if (slot.Condition != null)
			{
				var box = this.Field.Crane.Box;
				if ((slot.Condition < 4 && (box == null || box.Color != slot.Condition)
						|| slot.Condition == 4 && box != null
						|| slot.Condition == 5 && box == null))
					condition_fit = false;
			}

			// Moving current slot
			this.RunSlot += 1;
			if (condition_fit)
				if (slot.Value >= 4 && slot.Value < 8)
				{
					this.RunLine = slot.Value - 4;
					this.RunSlot = 0;
				}

			if (condition_fit)
				return slot.Value;
			return null;
		}

		public string ActionName(int action)
		{
			if (action == 1)
				return "MOVE RIGHT";
			else if (action == 2)
				return "MOVE DOWN";
			else if (action == 3)
				return "MOVE LEFT";
			else if (action == 4)
				return "GOTO F1";
			else if (action == 5)
				return "GOTO F2";
			else if (action == 6)
				return "GOTO F3";
			else if (action == 7)
				return "GOTO F4";
			throw new Exception();
		}

		public void RunAction(int? action)
		{
			if (action.HasValue && action.Value > 0)
			{
				if (action == 1)
					this.Field.Crane.MoveRight();
				else if (action == 2)
				{
					this.Field.Crane.MoveDown();
					Task.Delay(this.RunDelay / 2).ContinueWith(_ => this.Field.Crane.MoveUp());
				}
				else if (action == 3)
					this.Field.Crane.MoveLeft();
			}
		}

		public void LoadLevel(CargoBotLevel level)
		{
			this.Level = level;
			this.Field.LoadLevel(level);

			for (int i = 0; i < level.SlotLines.Count(); i++)
			{
				var line = level.SlotLines.ElementAt(i);
				for (int j = 0; j < line.Count(); j++)
				{
					var command = line.ElementAt(j);
					var slot = this.Lines[i].Slot[j];
					slot.Value = command.ElementAt(0);
					if (command.Count() > 1)
						slot.Condition = command.ElementAt(1);
				}
			}

			for (int i = 0; i < level.Tools.Count(); i++)
			{
				var tool = level.Tools.ElementAt(i);
				var slot = this.Toolbox[i % 4, i / 4];
				((Slot)slot).Value = tool;
			}
		}
	}
}
