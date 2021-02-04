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
    public class UserSaver : VisualObject
    {
        public UserSaver()
            : base(0, 0, 0, 0)
        {
            Name = "CargobotUserSaver";
        }

        protected override void UDBReadNative(BinaryReader br, int user)
        {
            try
            {
                CargoBotGame game = CargoBotPlugin.GameByUser(user);
                bool fast = br.ReadBoolean();
                game.RunDelay = fast ? 300 : 600;
                game.SpeedCheckbox.SetValue(fast, false, game.Player.Index);
                Console.WriteLine($"FAST? {fast}");
            }
            catch
            {
                UDBWrite(user);
            }
        }

        protected override void UDBWriteNative(BinaryWriter bw, int user)
        {
            CargoBotGame game = CargoBotPlugin.GameByUser(user);
            bw.Write((bool)game.Fast);
            Console.WriteLine($"SAVE FAST {game.Fast}");
        }
    }
}
