using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ID;
using TerrariaUI;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Hooks.Args;
using TerrariaUI.Widgets;
using TShockAPI;
using TUIPlugin;

namespace CargoBot
{
	public class CargoBotGame : VisualContainer
	{
		public const int SlowDelay = 600;
		public const int FastDelay = 200;
		public const int MaxRunningTime = 300000;
		public const int SessionLength = 600000;
		public const int ResultsDelay = 3000;

		public Field Field;
		public List<SlotLine> Lines;
		public Label StarsLabel;
		public Label StarsCountLabel;
		public Toolbox Toolbox;
		public Checkbox SpeedCheckbox;

		public bool Running = false;
		public int RunningIndex = 0;
		public int RunLine = 0;
		public int RunSlot = 0;
		public int TotalStars = 0;
		public int StarsGained = 0;
		public int UsedSlots = Int32.MaxValue;
		public CargoBotLevel Level = null;
		public Slot OldSlot = null;
		public TSPlayer Player = null;
		public int User = -1;
		public bool Playing = false;
		public int SessionIndex = 0;
		public DateTime BeginTime;
		public bool WaitingForReset = false;
		public bool ExitRequested = false;
		public bool Fast = false;
		public Stack<(int, int)> FunctionStack = new Stack<(int, int)>();

		public string LeaderboardDatabaseKey => "CargoBotGame";
		public string LevelLeaderboardDatabaseKey => Level.LevelName;
		public int RunDelay => Fast ? FastDelay : SlowDelay;
		public void StartPlayerSession(int playerIndex, int timeout) =>
			GetAncestor<CargoBotApplication>().StartPlayerSession(new int[] { playerIndex }, timeout);
		public void EndPlayerSession() =>
			GetAncestor<CargoBotApplication>().EndPlayerSession();

		public CargoBotGame(int x, int y)
			: base(x, y, 0, 0, new UIConfiguration() { UseEnd = true, BeginRequire = false },
				  new ContainerStyle() { Wall = 155 })
		{
			Name = "CargoBot";

			Field = Add(new Field(1, 1, 8, 7, 2, 2, 2, 1,
				TileID.SlimeBlock, PaintID2.White, TileID.SlimeBlock, PaintID2.White,
				new UIStyle() { Wall = 155, WallColor = PaintID2.White }));
			Add(new VisualObject(1, Field.Height + 5, 4, 22, new UIConfiguration() { UseBegin = true },
				new UIStyle() { Wall = WallID.SapphireGemspark, WallColor = PaintID2.DeepSkyBlue },
				(self, touch) => Player.SendInfoMessage(Slot.InfoMessage(0))));

			Lines = new List<SlotLine>();
			for (int i = 0; i < 4; i++)
				Lines.Add(Add(new SlotLine(1, Field.Height + 4 + i * 6, i, i < 3 ? 8 : 5)));
			OldSlot = Lines[0].Slots[0];

			StarsLabel = Add(new Label(1, 1 + Field.Height + 1, 11, 2, "stars", new LabelStyle() { WallColor = PaintID2.DeepRed }));
			StarsCountLabel = Add(new Label(12, 1 + Field.Height + 1, 2, 2, "0", new LabelStyle() { WallColor = PaintID2.DeepRed }));

			Add(new Label(39, 1 + Field.Height + 1, 16, 2, "toolbox"));
			Toolbox = Add(new Toolbox(39, 1 + Field.Height + 4, 4, 4));

			Add(new Button(27, 1 + Field.Height + 4 + Toolbox.Height + 2, 8, 4, "run", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156 },
				(self, t) => StartRunning()));

			Add(new Button(43, 1 + Field.Height + 4 + Toolbox.Height + 2, 12, 4, "clear", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.Gray },
				(self, t) => GetAncestor<Application>().Confirm("Clear all slots", (result) =>
				{
					if (result)
                    {
						Level.ClearSlots(this);
						Level.UDBWrite(User);
						foreach (var line in Lines)
							line.Apply();
                    }
				})));

			Add(new Button(1, 1 + Field.Height + 4 + Toolbox.Height + 2 + 5, 24, 4, "leaderboard", null, new ButtonStyle()
			{
				Wall = WallID.AmberGemspark,
				WallColor = PaintID2.DeepSkyBlue,
				BlinkStyle = ButtonBlinkStyle.Full
			}, (self, touch) => ShowLeaderbord()));

			Add(new Button(26, 1 + Field.Height + 4 + Toolbox.Height + 2 + 5, 10, 4, "next", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.Gray },
				(self, t) =>
				{
					Level.UDBWrite(User);
					int index = CargoBotPlugin.Levels.IndexOf(Level);
					if (index + 1 >= CargoBotPlugin.Levels.Count)
                    {
						t.Player().SendInfoMessage("This level is the last one.");
						return;
                    }
					Level = CargoBotPlugin.Levels[index + 1];
					Level.LoadStatic(this);
					Level.ClearSlots(this);
					if (!Level.UDBRead(User))
						StarsGained = 0;
					TotalStars = Leaderboard.GetLeaderboardValue(LeaderboardDatabaseKey, User) ?? 0;
					UsedSlots = Leaderboard.GetLeaderboardValue(LevelLeaderboardDatabaseKey, User) ?? Int32.MaxValue;
					UpdateStarsLabel(false);
					Update().Apply().Draw();
				}));

			Add(new Button(37, 1 + Field.Height + 4 + Toolbox.Height + 2 + 5, 10, 4, "hint", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.Gray },
				(self, t) => t.Player().SendInfoMessage($"Hint: {Level.Hint}")));

			SpeedCheckbox = Add(new Checkbox(48, 1 + Field.Height + 4 + Toolbox.Height + 3 + 5, 2, new CheckboxStyle()
				{ Wall = 156, WallColor = PaintID2.Gray, CheckedColor = PaintID2.DeepOrange },
				new Input<bool>(false, false, (self, value, player) =>
				{
					Fast = value;
					if (value)
						Player.SendInfoMessage("Fast mode on.");
					else
						Player.SendInfoMessage("Fast mode off.");
				})));

			Add(new Button(51, 1 + Field.Height + 4 + Toolbox.Height + 2 + 5, 4, 4, "x", null,
				new ButtonStyle() { BlinkStyle = ButtonBlinkStyle.Full, Wall = 156, WallColor = PaintID2.DeepRed },
				(self, t) => EndPlayerSession()));

			SetWH(1 + Field.Width + 1, 1 + Field.Height + 4 + Toolbox.Height + 2 + 4 + 1 + 5, false);
		}

		public override void Invoke(Touch touch)
		{
			if (touch.State == TouchState.End && !Running)
			{
				var _begin_slot = touch.Session.BeginTouch.Object;
				if (_begin_slot is Slot begin_slot)
				{
					if (begin_slot.WithCondition)
					{
						if (touch.Session.BeginTouch.Y > 0)
							begin_slot.Value = 0;
						else
							begin_slot.Condition = null;
						begin_slot.Apply().Draw();
					}
					else
						Player.SendInfoMessage(Slot.InfoMessage(0));
				}
			}
		}

        protected override bool CanTouchNative(Touch touch)
        {
			return base.CanTouchNative(touch) &&
				(touch.PlayerIndex == Player.Index || touch.Player().HasPermission("TUI.control"));
        }

		public void Start(CargoBotLevel level, TSPlayer player, int user)
		{
			User = user;
			StartPlayerSession(player.Index, SessionLength / 1000);

			Player = player;
			Playing = true;
			SessionIndex++;
			Level = level;
			Level.LoadStatic(this);

			if (!CargoBotPlugin.UserSaver.UDBRead(User))
				Fast = false;
			Level.ClearSlots(this);
			if (!Level.UDBRead(User))
				StarsGained = 0;
			TotalStars = Leaderboard.GetLeaderboardValue(LeaderboardDatabaseKey, User) ?? 0;
			UsedSlots = Leaderboard.GetLeaderboardValue(LevelLeaderboardDatabaseKey, User) ?? Int32.MaxValue;
			UpdateStarsLabel(false);

			GetAncestor<Panel>().Summon(this, Alignment.Down);
			Player.SendInfoMessage($"You session has begun. You currently have {StarsGained} stars for this level.{(StarsGained == 3 ? "" : " Try gaining 3.")}\nYou have {SessionLength/60000} minutes.");
		}

		public void Stop()
		{
			StopRunning();
			Playing = false;
			ExitRequested = false;

			Level.UDBWrite(User);
			CargoBotPlugin.UserSaver.UDBWrite(User);
			GetAncestor<Panel>().UnsummonAll();
			User = -1;

			if (Player.Active)
				Player.SendInfoMessage("Your session has ended.");
		}

		public void StopRunning()
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
					int usedSlots = Lines.Sum(slotLine => slotLine.ChildrenFromBottom.Skip(1).Count(slot => ((Slot)slot).Value > 0));
					int stars = usedSlots <= xxx ? 3 : (usedSlots <= xx ? 2 : 1);

					UpdateUsedSlots(usedSlots);
					UpdateStarsCount(stars);
					UpdateStarsLabel(true);
					Player.SendSuccessMessage($"You won the game. You have achieved [c/ff0000:{stars}] stars for this solution.");
					Player.Firework(stars);
                }
				else
					Player.SendErrorMessage("You lost...");
			}
			else
				Player.SendErrorMessage("Running for too long...");

			int runningIndex = RunningIndex;
			int sessionIndex = SessionIndex;
			Task.Delay(ResultsDelay).ContinueWith(_ =>
			{
				if (Playing && SessionIndex == sessionIndex)
                {
					if (ExitRequested)
						EndPlayerSession();
					else if (WaitingForReset && RunningIndex == runningIndex)
					{
						StopRunning();
						Level.LoadField(this);
						Apply().Draw();
					}
                }
			});
		}

		private void UpdateUsedSlots(int slots)
		{
			if (slots >= UsedSlots)
				return;

			UsedSlots = slots;
			Leaderboard.SetLeaderboardValue(LevelLeaderboardDatabaseKey, User, slots);
		}

		private void UpdateStarsCount(int stars)
		{
			if (stars <= StarsGained)
				return;

			int diff = stars - StarsGained;
			StarsGained = stars;
			Leaderboard.SetLeaderboardValue(LeaderboardDatabaseKey, User, TotalStars + diff);
		}

		private void UpdateStarsLabel(bool draw)
		{
			if (StarsCountLabel.GetText() != StarsGained.ToString())
			{
				StarsCountLabel.SetText(StarsGained.ToString());
				switch (StarsGained)
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
				StarsLabel.Style.WallColor = StarsCountLabel.Style.WallColor;
				if (draw)
                {
					StarsCountLabel.Update().Apply();
					StarsLabel.Update().Apply().Draw(width: StarsLabel.Width + 2);
                }
            }
		}

		public void ShowLeaderbord()
        {
			if (Running || WaitingForReset)
            {
				StopRunning();
				Level.LoadField(this);
			}
			var app = GetAncestor<CargoBotApplication>();
			var leaderboard = new Leaderboard(0, 0, 50, 50, LevelLeaderboardDatabaseKey, new LeaderboardStyle() { Count = 100 });
			leaderboard.Configuration.Custom.CanTouch = (self, touch) => touch.PlayerIndex == Player.Index;
			leaderboard.LoadDBData();
			leaderboard.AddFooter(new Button(0, 0, 0, 4, "back", null, new ButtonStyle() { Wall = 154, WallColor = 27 }, (self, touch) => app.Unsummon()));
			app.Summon(leaderboard, Alignment.Down);
        }

		public void StartRunning()
		{
			if (Running || WaitingForReset)
			{
				StopRunning();
				Level.LoadField(this);
				Apply().Draw();
			}
			else
			{
				Level.UDBWrite(User);

				Running = true;
				RunningIndex++;
				BeginTime = DateTime.UtcNow;
				FunctionStack.Clear();
				RunMove();
			}
		}

		public void RunMove()
		{
			try
            {
				if (!Running || !CalculateActive())
					return;

				int? value = PullAction();
				if (value.HasValue)
					RunAction(value.Value);
				else if (!Running)
					return;
				else
					RunSlot++;
						
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

				if (Running)
					Task.Delay(RunDelay).ContinueWith(_ => RunMove());
            }
			catch (Exception e)
            {
				TUI.HandleException(e);
            }
		}

		public int? PullAction()
		{
			Slot slot = FindNextSlot();
			if (slot == null)
			{
				EndGame(false);
				return null;
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

			return condition_fit ? (int?)slot.Value : null;
		}

		public Slot FindNextSlot()
        {
			while (RunSlot >= Lines[RunLine].Slots.Count || Lines[RunLine].Slots[RunSlot].Value == 0)
            {
				while (RunSlot >= Lines[RunLine].Slots.Count)
					if (FunctionStack.Count == 0)
						return null;
					else
					{
						var pair = FunctionStack.Pop();
						RunLine = pair.Item1;
						RunSlot = pair.Item2;
					}
				RunSlot++;
			}
			return Lines[RunLine].Slots[RunSlot];
		}

		public void RunAction(int action)
		{
			if (action == 1)
            {
				Field.Crane.MoveRight();
				RunSlot += 1;
			}
			else if (action == 2)
			{
				Field.Crane.MoveDown();
				RunSlot += 1;
				Task.Delay(RunDelay / 2).ContinueWith(_ => Field.Crane.MoveUp());
			}
			else if (action == 3)
            {
				Field.Crane.MoveLeft();
				RunSlot += 1;
			}
			else if (action >= 4 && action < 8)
            {
				FunctionStack.Push((RunLine, RunSlot));
				RunLine = action - 4;
				RunSlot = 0;
			}
		}
	}
}
