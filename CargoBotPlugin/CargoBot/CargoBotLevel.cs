using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;
using TerrariaUI.Widgets;

namespace CargoBot
{
    public class CargoBotLevel
    {
        public int CraneColumn;
        public IEnumerable<IEnumerable<int>> Columns;
        public IEnumerable<IEnumerable<int>> ResultColumns;
        public IEnumerable<IEnumerable<IEnumerable<int>>> SlotLines;
        public IEnumerable<int> Tools;
        public (int, int, int) Stars;
        public string Hint;

        public CargoBotLevel(int crane_column, IEnumerable<IEnumerable<int>> columns,
            IEnumerable<IEnumerable<int>> result_columns, IEnumerable<IEnumerable<IEnumerable<int>>> slot_lines,
            IEnumerable<int> tools, (int, int, int) stars, string hint)
        {
            this.CraneColumn = crane_column;
            this.Columns = columns;
            this.ResultColumns = result_columns;
            this.SlotLines = slot_lines;
            this.Tools = tools;
            this.Stars = stars;
            this.Hint = hint;
        }
    }
}
