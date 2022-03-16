using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Mouseless.Edits.Detours {
	internal static class Xna {
		internal delegate MouseState Mouse_GetState_orig();

		internal static MouseState Mouse_GetState(Mouse_GetState_orig orig) {
			//Mouse movement handled in EditsLoader.cs
			MouseState state = orig();

			bool clickLeft = state.LeftButton == ButtonState.Pressed, clickRight = state.RightButton == ButtonState.Pressed;

			if (IsPressed(CoreMod.MouseClickLeft))
				clickLeft = true;

			if (IsPressed(CoreMod.MouseClickRight))
				clickRight = true;

			return new MouseState(state.X, state.Y, state.ScrollWheelValue,
				clickLeft ? ButtonState.Pressed : ButtonState.Released,
				state.MiddleButton,
				clickRight ? ButtonState.Pressed : ButtonState.Released,
				state.XButton1,
				state.XButton2);
		}

		private static bool IsPressed(ModKeybind key) {
			try {
				if (!CoreMod.KeybindsRegistered)
					return false;

				var keys = key.GetAssignedKeys();

				if (keys.Count == 0 || !Enum.TryParse<Keys>(keys[0], out var keyEnum))
					return false;

				return Main.keyState.IsKeyDown(keyEnum);
			} catch {
				return false;
			}
		}
	}
}
