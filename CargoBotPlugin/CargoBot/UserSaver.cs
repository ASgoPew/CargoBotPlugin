using System.IO;
using TerrariaUI.Base;

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
                if (game == null)
                    return;
                bool fast = br.ReadBoolean();
                game.RunDelay = fast ? CargoBotGame.FastDelay : CargoBotGame.SlowDelay;
                game.SpeedCheckbox.SetValue(fast, false, game.Player.Index);
            }
            catch
            {
                UDBWrite(user);
            }
        }

        protected override void UDBWriteNative(BinaryWriter bw, int user)
        {
            CargoBotGame game = CargoBotPlugin.GameByUser(user);
            if (game == null)
                return;
            bw.Write((bool)game.Fast);
        }
    }
}
