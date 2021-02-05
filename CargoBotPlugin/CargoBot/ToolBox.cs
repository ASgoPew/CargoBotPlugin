using System.Linq;
using TerrariaUI.Base;

namespace CargoBot
{
    public class Toolbox : VisualObject
    {
        public Toolbox(int x, int y, int columns, int lines)
            : base(x, y, 4 * columns, 4 * lines, new UIConfiguration() { UseBegin = false })
        {
            SetupGrid(Enumerable.Repeat(new Absolute(4), columns), Enumerable.Repeat(new Absolute(4), lines));
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < lines; j++)
                    this[i, j] = new Slot(i * 4, j * 4, 0, null, i + j, false);
        }
    }
}
