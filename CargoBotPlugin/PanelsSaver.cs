using System.IO;
using TerrariaUI.Base;

namespace CargoBot
{
    public class PanelsSaver : VisualObject
    {
        public PanelsSaver()
            : base(0, 0, 0, 0)
        {
            Name = "CargobotPanelsSaver";
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
