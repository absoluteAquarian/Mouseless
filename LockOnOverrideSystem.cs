using Microsoft.Xna.Framework.Input;
using System;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Mouseless {
	internal class LockOnOverrideSystem : ModSystem {
		public override void PostUpdateInput() {
			if (!CoreMod.KeybindsRegistered)
				return;

			// Safeguard: manually ensure that the dictionaries are properly initialized
			if (!PlayerInput.CurrentProfile.InputModes.TryGetValue(InputMode.Keyboard, out var keyConfig) || !keyConfig.KeyStatus.TryGetValue(CoreMod.LockOn.GetFullName(), out var keys))
				return;

			if (keys.Count == 0 || !Enum.TryParse<Keys>(keys[0], out _))
				return;

			if (CoreMod.LockOn.Current)
				PlayerInput.Triggers.Current.LockOn = true;

			if (CoreMod.LockOn.JustPressed)
				PlayerInput.Triggers.JustPressed.LockOn = true;

			if (CoreMod.LockOn.JustReleased)
				PlayerInput.Triggers.JustReleased.LockOn = true;

			if (CoreMod.LockOn.Old)
				PlayerInput.Triggers.Old.LockOn = true;
		}
	}
}
