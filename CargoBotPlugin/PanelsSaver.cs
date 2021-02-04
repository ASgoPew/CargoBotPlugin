using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaUI.Base;
using TerrariaUI.Base.Style;

namespace CargoBot
{
    public class PanelsSaver : VisualObject
    {
        public PanelsSaver(string name)
            : base(0, 0, 0, 0)
        {
            Name = name;
        }

        protected override void DBReadNative(BinaryReader br)
        {
            try
            {
                int count = br.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    int panelIndex = br.ReadInt32();
                    CargoBotPlugin.CreateGameInstance(0, 0, panelIndex);
                }
            }
            catch
            {
                DBWrite();
            }
        }

        protected override void DBWriteNative(BinaryWriter bw)
        {
            bw.Write((byte)CargoBotPlugin.Games.Count);
            foreach (var pair in CargoBotPlugin.Games)
                bw.Write((int)pair.Key);
        }
    }
}
