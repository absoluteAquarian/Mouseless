using Microsoft.Xna.Framework.Input;
using Mouseless.Edits;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Mouseless {
	public class CoreMod : Mod {
		public static ModKeybind MouseUp, MouseLeft, MouseDown, MouseRight,
			MouseClickLeft, MouseClickRight,
			MouseSensivityChange,
			LockOn;

		public const string Keybinding_LockOn = "Lock On";

		public static string Keybinding_LockOn_FullIdentifier => ModContent.GetInstance<CoreMod>().Name + ": " + Keybinding_LockOn;

		internal static bool KeybindsRegistered;

		public override void Load() {
			if (!Main.dedServ) {
				MouseUp = KeybindLoader.RegisterKeybind(this, "Mouse Move Up", Keys.Up);
				MouseLeft = KeybindLoader.RegisterKeybind(this, "Mouse Move Left", Keys.Left);
				MouseDown = KeybindLoader.RegisterKeybind(this, "Mouse Move Down", Keys.Down);
				MouseRight = KeybindLoader.RegisterKeybind(this, "Mouse Move Right", Keys.Right);

				MouseClickLeft = KeybindLoader.RegisterKeybind(this, "Mouse Click Left", Keys.PageUp);
				MouseClickRight = KeybindLoader.RegisterKeybind(this, "Mouse Click Right", Keys.PageDown);

				MouseSensivityChange = KeybindLoader.RegisterKeybind(this, "Mouse Sensitivity", Keys.OemPeriod);

				LockOn = KeybindLoader.RegisterKeybind(this, Keybinding_LockOn, Keys.Z);

				KeybindsRegistered = true;
			}

			LockOnHelper.ForceUsability = true;

			EditsLoader.Load();

			DirectDetourManager.Load();
		}

		public override void Unload() {
			EditsLoader.Unload();

			DirectDetourManager.Unload();

			LockOnHelper.ForceUsability = false;

			KeybindsRegistered = false;
		}
	}
}