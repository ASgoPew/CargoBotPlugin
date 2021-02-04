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
    public class ToolBox : VisualObject
    {
        public ToolBox(int x, int y, int columns, int lines)
            : base(x, y, 4 * columns, 4 * lines, new UIConfiguration() { UseBegin = false })
        {
            SetupGrid(Enumerable.Repeat(new Absolute(4), columns), Enumerable.Repeat(new Absolute(4), lines));
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < lines; j++)
                    this[i, j] = new Slot(i * 4, j * 4, 0, null, i + j, false);
        }
    }
}
