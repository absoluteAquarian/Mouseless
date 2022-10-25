using Microsoft.Xna.Framework.Input;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Mouseless.Edits {
	internal class MouseClicksEdit : Edit {
		public static readonly MethodInfo Mouse_GetState = typeof(Mouse).GetMethod(nameof(Mouse.GetState), BindingFlags.Public | BindingFlags.Static);
		public delegate MouseState orig_Mouse_GetState();
		public delegate MouseState hook_Mouse_GetState(orig_Mouse_GetState orig);
		public event hook_Mouse_GetState On_Mouse_GetState {
			add => HookEndpointManager.Add<hook_Mouse_GetState>(Mouse_GetState, value);
			remove => HookEndpointManager.Remove<hook_Mouse_GetState>(Mouse_GetState, value);
		}

		public override void LoadEdits() {
			On_Mouse_GetState += Hook_Mouse_GetState;
		}

		public override void UnloadEdits() {
			On_Mouse_GetState -= Hook_Mouse_GetState;
		}

		internal static MouseState Hook_Mouse_GetState(orig_Mouse_GetState orig) {
			// Mouse movement is handled in another edit
			MouseState state = orig();

			bool clickLeft = state.LeftButton == ButtonState.Pressed, clickRight = state.RightButton == ButtonState.Pressed;

			if (IsPressed(CoreMod.MouseClickLeft))
				clickLeft = true;

			if (IsPressed(CoreMod.MouseClickRight))
				clickRight = true;

			return new MouseState(state.X, state.Y, state.ScrollWheelValue,
				clickLeft ? ButtonState.Pressed : state.LeftButton,
				state.MiddleButton,
				clickRight ? ButtonState.Pressed : state.RightButton,
				state.XButton1,
				state.XButton2);
		}

		private static bool IsPressed(ModKeybind key) {
			try {
				if (!CoreMod.KeybindsRegistered)
					return false;

				// Safeguard: manually ensure that the dictionaries are properly initialized
				if (!PlayerInput.CurrentProfile.InputModes.TryGetValue(InputMode.Keyboard, out var keyConfig) || !keyConfig.KeyStatus.TryGetValue(key.GetFullName(), out var keys))
					return false;

				if (keys.Count == 0 || !System.Enum.TryParse<Keys>(keys[0], out var keyEnum))
					return false;

				return Main.keyState.IsKeyDown(keyEnum);
			} catch {
				return false;
			}
		}
	}
}
