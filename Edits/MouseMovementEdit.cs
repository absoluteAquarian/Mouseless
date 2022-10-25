using Microsoft.Xna.Framework.Input;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Mouseless.Edits {
	internal class MouseMovementEdit : Edit {
		public static readonly MethodInfo ConfigManager_Save = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);

		public static readonly FieldInfo Interface_modConfig = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface").GetField("modConfig", BindingFlags.Static | BindingFlags.NonPublic);
		public static readonly FieldInfo UIModConfig_updateNeeded = typeof(Mod).Assembly.GetType("Terraria.ModLoader.Config.UI.UIModConfig").GetField("updateNeeded", BindingFlags.Instance | BindingFlags.NonPublic);

		public override void LoadEdits() {
			IL.Terraria.GameInput.PlayerInput.MouseInput += Patch_PlayerInput_MouseInput;
		}

		public override void UnloadEdits() {
			IL.Terraria.GameInput.PlayerInput.MouseInput -= Patch_PlayerInput_MouseInput;
		}

		private static void Patch_PlayerInput_MouseInput(ILContext il) {
			ILCursor c = new(il);

			if(!c.TryGotoNext(MoveType.After, i => i.MatchCall(MouseClicksEdit.Mouse_GetState)))
				throw new InvalidOperationException("Could not patch method " + il.Method.FullName);

			c.EmitDelegate<Func<MouseState, MouseState>>(state => {
				//Clicks handled in Detours/Xna.Mouse.cs
				int offsetX = 0, offsetY = 0;
				
				var config = ModContent.GetInstance<MouselessConfig>();

				if (CoreMod.ShouldCheckKeybinds() && IsToggledOn(CoreMod.MouseSensivityChange)) {
					int old = config.SensitivityOption;

					if (Main.keyState.PressingShift())
						config.SensitivityOption--;
					else
						config.SensitivityOption++;

					if (config.SensitivityOption != old) {
						ConfigManager_Save?.Invoke(null, new object[]{ config });

						UIModConfig_updateNeeded.SetValue(Interface_modConfig.GetValue(null), true);

						SoundEngine.PlaySound(SoundID.Item37 with { Pitch = config.SensitivityOption / 4f });
					}
				}

				int sensitvity = (int)(16 * config.SensitivityScale);

				if (IsPressed(CoreMod.MouseLeft))
					offsetX -= sensitvity;

				if (IsPressed(CoreMod.MouseRight))
					offsetX += sensitvity;

				if (IsPressed(CoreMod.MouseUp))
					offsetY -= sensitvity;

				if (IsPressed(CoreMod.MouseDown))
					offsetY += sensitvity;
				
				if (Main.instance.IsActive && (offsetX != 0 || offsetY != 0))
					Mouse.SetPosition(state.X + offsetX, state.Y + offsetY);

				return new MouseState(state.X + offsetX, state.Y + offsetY,
					state.ScrollWheelValue,
					state.LeftButton,
					state.MiddleButton,
					state.RightButton,
					state.XButton1,
					state.XButton2);
			});
		}

		private static bool IsPressed(ModKeybind key) {
			try {
				if (!CoreMod.KeybindsRegistered)
					return false;

				// Safeguard: manually ensure that the dictionaries are properly initialized
				if (!PlayerInput.CurrentProfile.InputModes.TryGetValue(InputMode.Keyboard, out var keyConfig) || !keyConfig.KeyStatus.TryGetValue(key.GetFullName(), out var keys))
					return false;

				if (keys.Count == 0 || !Enum.TryParse<Keys>(keys[0], out var keyEnum))
					return false;

				return Main.keyState.IsKeyDown(keyEnum);
			} catch {
				return false;
			}
		}

		private static bool IsToggledOn(ModKeybind key) {
			try {
				if (!CoreMod.KeybindsRegistered)
					return false;

				// Safeguard: manually ensure that the dictionaries are properly initialized
				if (!PlayerInput.CurrentProfile.InputModes.TryGetValue(InputMode.Keyboard, out var keyConfig) || !keyConfig.KeyStatus.TryGetValue(key.GetFullName(), out var keys))
					return false;

				if (keys.Count == 0 || !Enum.TryParse<Keys>(keys[0], out var keyEnum))
					return false;

				return Main.keyState.IsKeyDown(keyEnum) && !Main.oldKeyState.IsKeyDown(keyEnum);
			} catch {
				return false;
			}
		}
	}
}
