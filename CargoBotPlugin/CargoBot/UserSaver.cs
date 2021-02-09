using System.IO;
using TerrariaUI;
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
            CargoBotGame game = CargoBotPlugin.GameByUser(user);
            if (game == null)
                return;

            bool fast;
            try
            {
                fast = br.ReadBoolean();
            }
            catch (EndOfStreamException)
            {
                TUI.Log("UserSaver invalid database data", LogType.Warning);
                UDBWrite(user);
                return;
            }
            game.Fast = fast;
            game.SpeedCheckbox.SetValue(fast, false, game.Player.Index);
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
