using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using Terraria.GameInput;

namespace Mouseless.Edits {
	internal class LockOnEdit : Edit {
		public static readonly MethodInfo TriggersSet_get_LockOn = typeof(TriggersSet).GetProperty("LockOn", BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
		public delegate bool orig_TriggersSet_get_LockOn(TriggersSet self);
		public delegate bool hook_TriggersSet_get_LockOn(orig_TriggersSet_get_LockOn orig, TriggersSet self);
		public event hook_TriggersSet_get_LockOn On_TriggersSet_get_LockOn {
			add => HookEndpointManager.Add<hook_TriggersSet_get_LockOn>(TriggersSet_get_LockOn, value);
			remove => HookEndpointManager.Remove<hook_TriggersSet_get_LockOn>(TriggersSet_get_LockOn, value);
		}

		public override void LoadEdits() {
			On_TriggersSet_get_LockOn += Hook_TriggersSet_get_LockOn;
		}

		public override void UnloadEdits() {
			On_TriggersSet_get_LockOn -= Hook_TriggersSet_get_LockOn;
		}

		internal static bool Hook_TriggersSet_get_LockOn(orig_TriggersSet_get_LockOn orig, TriggersSet self) {
			if (orig(self))
				return true;

			if (!CoreMod.KeybindsRegistered)
				return false;

			return self.KeyStatus.TryGetValue(CoreMod.LockOn.GetFullName(), out bool triggered) && triggered;
		}
	}
}
