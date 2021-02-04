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
    public class SlotLine : VisualObject
    {
        public List<Slot> Slots;

        public SlotLine(int x, int y, int index, int slot_count)
            : base(x, y, 5 + 4 * slot_count, 5, new UIConfiguration() { UseBegin = false })
        {
            Slots = new List<Slot>();
            Add(new Label(0, 2, 4, 2, $"f{index + 1}"));
            for (int i = 0; i < slot_count; i++)
                Slots.Add(Add(new Slot(5 + i * 4, 0, 0, null, i)));
        }
    }
}
