using Terraria.GameInput;

namespace Mouseless.Edits.Detours {
	internal static class Vanilla {
		public delegate bool TriggersSet_get_LockOn_orig(TriggersSet self);

		internal static bool TriggersSet_get_LockOn(TriggersSet_get_LockOn_orig orig, TriggersSet self)
			=> orig(self) || self.KeyStatus[CoreMod.Keybinding_LockOn_FullIdentifier];
	}
}
