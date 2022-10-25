using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Mouseless {
	public class CoreMod : Mod {
		public static ModKeybind MouseUp, MouseLeft, MouseDown, MouseRight,
			MouseClickLeft, MouseClickRight,
			MouseSensivityChange,
			LockOn;

		internal static bool KeybindsRegistered;

		public override void Load() {
			if (!Main.dedServ) {
				MouseUp = KeybindLoader.RegisterKeybind(this, "MoveUp", Keys.Up);
				MouseLeft = KeybindLoader.RegisterKeybind(this, "MoveLeft", Keys.Left);
				MouseDown = KeybindLoader.RegisterKeybind(this, "MoveDown", Keys.Down);
				MouseRight = KeybindLoader.RegisterKeybind(this, "MoveRight", Keys.Right);

				MouseClickLeft = KeybindLoader.RegisterKeybind(this, "ClickLeft", Keys.PageUp);
				MouseClickRight = KeybindLoader.RegisterKeybind(this, "ClickRight", Keys.PageDown);

				MouseSensivityChange = KeybindLoader.RegisterKeybind(this, "Sensitivity", Keys.OemPeriod);

				LockOn = KeybindLoader.RegisterKeybind(this, "LockOn", Keys.Z);

				KeybindsRegistered = true;
			}

			LockOnHelper.ForceUsability = true;
		}

		public override void Unload() {
			LockOnHelper.ForceUsability = false;

			KeybindsRegistered = false;
		}
	}
}