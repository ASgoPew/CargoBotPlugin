using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ID;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Widgets;
using TShockAPI;
using TUIPlugin;

namespace CargoBot
{
	public class CargoBotGame : VisualContainer
	{
		public const int SlowDelay = 600;
		public const int FastDelay = 200;
		public const int MaxRunningTime = 60000;
		public const int SessionLength = 180000;
		public const int ResultsDelay = 3000;

		private static object StaticLocker = new object();

		public Field Field;
		public List<SlotLine> Lines;
		public Label StarsCountLabel;
		public Toolbox Toolbox;
		public Checkbox SpeedCheckbox;
		public bool Running;
		public int RunningIndex = 0;
		public int RunLine;
		public int RunSlot;
		public int RunDelay;
		public CargoBotLevel Level;
		public Slot OldSlot;
		public TSPlayer Player;
		public int User;
		public bool Playing;
		public int SessionIndex = 0;
		public DateTime BeginTime;
		public bool WaitingForReset = false;
		public bool ExitRequested = false;

		public bool Fast => RunDelay == FastDelay;
		public void StartPlayerSession(int playerIndex, int timeout) =>
			GetAncestor<CargoBotApplication>().StartPlayerSession(new int[] { playerIndex }, timeout);
		public void EndPlayerSession() =>
			GetAncestor<CargoBotApplication>().EndPlayerSession();

		public CargoBotGame(int x, int y)
			: base(x, y, 0, 0, new UIConfiguration() { UseEnd = true, BeginRequire = false },
				  new ContainerStyle() { Wall = 155 })
		{
			Field = Add(new Field(1, 1, 8, 7, 2, 2, 2, 1, TileID.EmeraldGemspark,
				PaintID2.Shadow, new UIStyle() { Wall = 155, WallColor = PaintID2.White }));
			Add(new VisualObject(1, Field.Height + 5, 4, 22, new UIConfiguration() { UseBegin = false },
				new UIStyle() { Wall = WallID.SapphireGemspark }));

			Lines = new List<SlotLine>();
			for (int i = 0; i < 4; i++)
				Lines.Add(Add(new SlotLine(1, Field.Height + 4 + i * 6, i, i < 3 ? 8 : 5)));

			Add(new Label(1, 1 + Field.Height + 1, 10, 2, "stars"));
			StarsCountLabel = Add(new Label(12, 1 + Field.Height + 1, 2, 2, "0", new LabelStyle() { WallColor = PaintID2.DeepRed }));

			Add(new Label(39, 1 + Field.Height + 1, 16, 2, "toolbox"));
			Toolbox = Add(new Toolbox(39, 1 + Field.Height + 4, 4, 4));

			Add(new Button(27, 1 + Field.Height + 4 + Toolbox.Height + 2, 8, 4, "run", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156 },
				(self, t) => Run()));

			Add(new Button(36, 1 + Field.Height + 4 + Toolbox.Height + 2, 10, 4, "hint", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.Gray },
				(self, t) => t.Player().SendInfoMessage($"Hint: {Level.Hint}")));

			SpeedCheckbox = Add(new Checkbox(47, 1 + Field.Height + 4 + Toolbox.Height + 3, 2, new CheckboxStyle()
				{ Wall = 156, WallColor = PaintID2.Gray, CheckedColor = PaintID2.DeepOrange },
				new Input<bool>(false, false, (self, value, player) => RunDelay = value ? FastDelay : SlowDelay)));

			Add(new Button(51, 1 + Field.Height + 4 + Toolbox.Height + 2, 4, 4, "x", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.DeepRed },
				(self, t) => EndPlayerSession()));

			Playing = false;
			User = -1;
			Level = null;
			RunDelay = SlowDelay;
			Reset();

			Name = "CargoBot";

			SetWH(1 + Field.Width + 1, 1 + Field.Height + 4 + Toolbox.Height + 2 + 4 + 1, false);
		}

		public override void Invoke(Touch touch)
		{
			if (touch.State == TouchState.End && !Running)
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
		}

        protected override bool CanTouchNative(Touch touch)
        {
            return base.CanTouchNative(touch) && touch.PlayerIndex == Player.Index
				|| touch.Player().HasPermission("TUI.control");
        }

		public void Start(CargoBotLevel level, TSPlayer player, int user)
		{
			Player = player;
			StartPlayerSession(player.Index, SessionLength / 1000);
			User = user;

			Playing = true;
			SessionIndex++;
			Level = level;
			CargoBotPlugin.UserSaver.UDBRead(User);
			Level.UDBRead(User);
			Field.Update();
			UpdateStarsCount(false);
			Level.LoadStatic(this);

			GetAncestor<Panel>().Summon(this);
			Player.SendInfoMessage($"You session has begun. You have {SessionLength/60000} minutes.");

			/*int sessionIndex = SessionIndex;
			Task.Delay(SessionLength).ContinueWith(_ =>
			{
				if (Running)
					ExitRequested = true;
				else
					EndSession(sessionIndex);
			});*/
		}
		/*public void EndSession(int sessionIndex)
		{
			if (sessionIndex != SessionIndex || !Playing)
				return;
			Stop();
		}*/

		public void Stop()
		{
			Playing = false;
			ExitRequested = false;
			Level.UDBWrite(User);
			CargoBotPlugin.UserSaver.UDBWrite(User);
			User = -1;
			Level = null;
			Reset();
			GetAncestor<Panel>().UnsummonAll();

			if (Player.Active)
				Player.SendInfoMessage("Your session has ended.");
		}

		public void Reset()
		{
			WaitingForReset = false;
			Running = false;
			RunLine = 0;
			RunSlot = 0;
			if (OldSlot != null)
				OldSlot.Style.WallColor = null;
			OldSlot = Lines[0].Slots[0];
		}

		public void EndGame(bool? win)
		{
			Running = false;
			WaitingForReset = true;
			if (win.HasValue)
			{
				if (win.Value)
                {
					(int x, int xx, int xxx) = Level.Stars;
					int count = Lines.Sum(slotLine => slotLine.ChildrenFromBottom.Skip(1).Count(slot => ((Slot)slot).Value > 0));
					int stars = count <= xxx ? 3 : (count <= xx ? 2 : 1);
					bool updateStars = true;// stars > Level.StarsGained;
					Level.StarsGained = stars;
					Level.UDBWrite(User);
					if (updateStars)
						UpdateStarsCount(true);
					Player.SendSuccessMessage($"You won the game. You have achieved [c/ff0000:{stars}] stars.");
					Player.Firework(stars);
                }
				else
					Player.SendErrorMessage("You lost...");
			}
			else
				Player.SendErrorMessage("Running for too long...");
			int playingIndex = RunningIndex;
			int sessionIndex = SessionIndex;
			Task.Delay(ResultsDelay).ContinueWith(_ =>
			{
				if (Playing && SessionIndex == sessionIndex)
                {
					if (ExitRequested)
						EndPlayerSession();
					else if (WaitingForReset && RunningIndex == playingIndex)
					{
						Level.UDBWrite(User);
						Reset();
						Level.LoadField(this);
						Apply().Draw();
					}
                }
			});
		}

		private void UpdateStarsCount(bool draw)
        {
			int count = Level.StarsGained;
			if (StarsCountLabel.GetText() != count.ToString())
            {
				StarsCountLabel.SetText(count.ToString());
				switch (count)
                {
					case 0:
						StarsCountLabel.Style.WallColor = PaintID2.DeepRed;
						break;
					case 1:
						StarsCountLabel.Style.WallColor = PaintID2.DeepOrange;
						break;
					case 2:
						StarsCountLabel.Style.WallColor = PaintID2.DeepYellow;
						break;
					case 3:
						StarsCountLabel.Style.WallColor = PaintID2.DeepGreen;
						break;
                }
				if (draw)
					StarsCountLabel.Update().Apply().Draw();
            }
		}

		public void Run()
		{
			lock (StaticLocker)
				if (Running || WaitingForReset)
				{
					Reset();
					Level.LoadField(this);
					Apply().Draw();
				}
				else
				{
					Running = true;
					RunningIndex++;
					BeginTime = DateTime.UtcNow;
					RunMove();
				}
		}

		public void RunMove()
		{
			lock (StaticLocker)
			{
				if (Disposed || !Running)
					return;
				var value = PullAction();
				RunAction(value);
				if ((DateTime.UtcNow - BeginTime).TotalMilliseconds > MaxRunningTime)
                {
					EndGame(null);
					return;
                }
				if (value == 2 && Field.Crane.Box == null && Field.CheckWin())
                {
					EndGame(true);
					return;
                }
				else if (RunSlot == Lines[RunLine].Slots.Count // Checking if there is no next slot
					|| Lines[RunLine].Slots.Skip(RunSlot).All(slot => slot.Value == 0))
                {
					EndGame(false);
					return;
                }
				if (Running)
					Task.Delay(RunDelay).ContinueWith(_ => RunMove());
			}
		}

		public int? PullAction()
		{
			SlotLine line = Lines[RunLine];
			Slot slot = line.Slots[RunSlot];
			while (slot.Value == 0)
            {
				if (RunSlot + 1 >= line.Slots.Count)
					return null;
				slot = line.Slots[++RunSlot];
            }

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
	}
}
