using Microsoft.Xna.Framework.Input;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Mouseless.Edits {
	//Handles loading/unloading any method detours and IL edits
	internal static class EditsLoader {
		internal static MethodInfo ConfigManager_Save, ConfigManager_Load;
		internal static FieldInfo Interface_modConfig, UIModConfig_updateNeeded;

		public static void Load() {
			ConfigManager_Save = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
			ConfigManager_Load = typeof(ConfigManager).GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic);

			Interface_modConfig = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface").GetField("modConfig", BindingFlags.Static | BindingFlags.NonPublic);
			UIModConfig_updateNeeded = typeof(Mod).Assembly.GetType("Terraria.ModLoader.Config.UI.UIModConfig").GetField("updateNeeded", BindingFlags.Instance | BindingFlags.NonPublic);

			IL.Terraria.GameInput.PlayerInput.MouseInput += PatchMouseInput;
		}

		public static void Unload() {
			ConfigManager_Save = null;
			ConfigManager_Load = null;

			Interface_modConfig = null;
			UIModConfig_updateNeeded = null;
		}

		private static void PatchMouseInput(ILContext il) {
			MethodInfo Mouse_GetState = typeof(Mouse).GetMethod("GetState", BindingFlags.Static | BindingFlags.Public);

			ILCursor c = new(il);

			if(!c.TryGotoNext(MoveType.After, i => i.MatchCall(Mouse_GetState)))
				throw new InvalidOperationException("Could not patch method " + il.Method.FullName);

			c.EmitDelegate<Func<MouseState, MouseState>>(state => {
				//Clicks handled in Detours/Xna.Mouse.cs
				int offsetX = 0, offsetY = 0;
				
				var config = ModContent.GetInstance<MouselessConfig>();

				if (IsToggledOn(CoreMod.MouseSensivityChange)) {
					int old = config.SensitivityOption;

					if (Main.keyState.PressingShift())
						config.SensitivityOption--;
					else
						config.SensitivityOption++;

					if (config.SensitivityOption != old) {
						ConfigManager_Save?.Invoke(null, new object[]{ config });

						UIModConfig_updateNeeded.SetValue(Interface_modConfig.GetValue(null), true);

						SoundEngine.PlaySound(SoundID.Item, Style: 37, pitchOffset: config.SensitivityOption / 4f);
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
				
				if (Main.instance.IsActive)
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

				var keys = key.GetAssignedKeys();

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

				var keys = key.GetAssignedKeys();

				if (keys.Count == 0 || !Enum.TryParse<Keys>(keys[0], out var keyEnum))
					return false;

				return Main.keyState.IsKeyDown(keyEnum) && !Main.oldKeyState.IsKeyDown(keyEnum);
			} catch {
				return false;
			}
		}
	}
}
