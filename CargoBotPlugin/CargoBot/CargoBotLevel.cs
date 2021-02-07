using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrariaUI;
using TerrariaUI.Base;
using TerrariaUI.Hooks.Args;

namespace CargoBot
{
    public class CargoBotLevel : VisualObject
    {
        public int Index;
        public string LevelName;
        public int CraneColumn;
        public IEnumerable<IEnumerable<int>> Columns;
        public IEnumerable<IEnumerable<int>> ResultColumns;
        public IEnumerable<int> Tools;
        public (int, int, int) Stars;
        public int StarsGained = 0;
        public int UsedSlots = Int32.MaxValue;
        public string Hint;

        public CargoBotLevel(string name, int crane_column, IEnumerable<IEnumerable<int>> columns,
            IEnumerable<IEnumerable<int>> result_columns, IEnumerable<int> tools, (int, int, int) stars, string hint)
            : base(0, 0, 0, 0)
        {
            CraneColumn = crane_column;
            Columns = columns;
            ResultColumns = result_columns;
            Tools = tools;
            Stars = stars;
            Hint = hint;

            LevelName = name;
            Name = $"CargoBotLevel_{name}";
        }

        protected override void UDBReadNative(BinaryReader br, int user)
        {
            CargoBotGame game = CargoBotPlugin.GameByUser(user);
            if (game == null)
                return;

            foreach (var line in game.Lines)
                foreach (var _slot in line.ChildrenFromBottom)
                    if (_slot is Slot slot)
                    {
                        slot.Value = 0;
                        slot.Condition = null;
                    }
            int count;
            try
            {
                StarsGained = br.ReadByte();
                count = br.ReadByte();
            }
            catch (EndOfStreamException)
            {
                TUI.Log("CargoBotLevel invalid database data", LogType.Warning);
                ClearSlots(game);
                UDBWrite(user);
                return;
            }
            for (int i = 0; i < count; i++)
            {
                int lineIndex, slotIndex, value, condition;
                try
                {
                    lineIndex = br.ReadByte();
                    slotIndex = br.ReadByte();
                    value = br.ReadByte();
                    condition = br.ReadByte();
                }
                catch (EndOfStreamException)
                {
                    TUI.Log("CargoBotLevel invalid database data", LogType.Warning);
                    ClearSlots(game);
                    UDBWrite(user);
                    return;
                }

                Slot slot = game.Lines[lineIndex].GetChild(slotIndex + 1) as Slot;
                slot.Value = value;
                slot.Condition = condition;
                if (slot.Condition == 255)
                    slot.Condition = null;
            }
        }

        protected override void UDBWriteNative(BinaryWriter bw, int user)
        {
            CargoBotGame game = CargoBotPlugin.GameByUser(user);
            if (game == null)
                return;

            bw.Write((byte)StarsGained);
            int count = game.Lines.Sum(slotLine => slotLine.ChildrenFromBottom.Skip(1).Count(slot =>
                ((Slot)slot).Value > 0 || ((Slot)slot).Condition.HasValue));
            bw.Write((byte)count);
            for (int lineIndex = 0; lineIndex < game.Lines.Count; lineIndex++)
            {
                SlotLine line = game.Lines[lineIndex];
                for (int slotIndex = 0; slotIndex < game.Lines[lineIndex].ChildCount - 1; slotIndex++)
                {
                    Slot slot = line.GetChild(slotIndex + 1) as Slot;
                    if (slot.Value > 0 || slot.Condition.HasValue)
                    {
                        bw.Write((byte)lineIndex);
                        bw.Write((byte)slotIndex);
                        bw.Write((byte)slot.Value);
                        bw.Write((byte)(slot.Condition.HasValue ? slot.Condition.Value : -1));
                    }
                }
            }
        }

        public void LoadStatic(CargoBotGame game)
        {
            LoadField(game);
            LoadResultField(game);
            LoadTools(game);
        }

        public void LoadField(CargoBotGame game)
        {
            Field field = game.Field;

            // Crane
            field.Crane.Reset(CraneColumn);

            // Field
            foreach (var column in field.Columns)
                column.Reset();
            for (int i = 0; i < Columns.Count(); i++)
            {
                var column = Columns.ElementAt(i);
                foreach (var box_color in column)
                {
                    field.Columns[i].Push((byte)box_color);
                    field.Columns[i].Update();
                }
            }
        }

        public void LoadResultField(CargoBotGame game)
        {
            Field field = game.Field;

            // Field result
            foreach (var column in field.ResultColumns)
                column.Reset();
            for (int i = 0; i < ResultColumns.Count(); i++)
            {
                var column = ResultColumns.ElementAt(i);
                foreach (var box_color in column)
                {
                    field.ResultColumns[i].Push((byte)box_color);
                    field.ResultColumns[i].Update();
                }
            }
        }

        public void LoadTools(CargoBotGame game)
        {
            var toolbox = game.Toolbox;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    ((Slot)toolbox[i, j]).Value = 0;
            for (int i = 0; i < Tools.Count(); i++)
            {
                var tool = Tools.ElementAt(i);
                var slot = toolbox[i % 4, i / 4];
                ((Slot)slot).Value = tool;
            }
        }

        public void ClearSlots(CargoBotGame game)
        {
            foreach (SlotLine line in game.Lines)
                foreach (Slot slot in line.ChildrenFromBottom.Skip(1))
                {
                    slot.Value = 0;
                    slot.Condition = null;
                }
        }
    }
}
