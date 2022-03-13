using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using Mouseless.Edits.Detours;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Mouseless.Edits {
	internal static class DirectDetourManager {
		private static readonly List<Hook> detours = new();
		private static readonly List<(MethodInfo, Delegate)> delegates = new();

		private static readonly Dictionary<string, MethodInfo> cachedMethods = new();

		public static void Load() {
			try {
				MonoModHooks.RequestNativeAccess();

				// Usage: allowing LockOn to be handled on keyboard inputs
				DetourHook(typeof(TriggersSet).GetProperty("LockOn").GetGetMethod(), typeof(Vanilla).GetCachedMethod("TriggersSet_get_LockOn"));

				// Usage: applying keyboard inputs to mouse stuff (since mods sometimes use a MouseState instead of the Main fields)
				DetourHook(typeof(Mouse).GetCachedMethod("GetState"), typeof(Xna).GetCachedMethod("Mouse_GetState"));
			} catch (Exception ex) {
				throw new Exception("An error occurred while doing patching in Mouseless." +
				                    "\nReport this error to the mod devs and disable the mod in the meantime." +
				                    "\n\n" +
				                    ex);
			}
		}

		private static MethodInfo GetCachedMethod(this Type type, string method) {
			string key = $"{type.FullName}::{method}";
			if (cachedMethods.TryGetValue(key, out MethodInfo value))
				return value;

			return cachedMethods[key] = type.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
		}

		public static void Unload() {
			foreach (Hook hook in detours)
				if (hook.IsValid && hook.IsApplied)
					hook.Undo();

			foreach ((MethodInfo method, Delegate hook) in delegates)
				HookEndpointManager.Unmodify(method, hook);
		}

		private static void IntermediateLanguageHook(MethodInfo orig, MethodInfo modify) {
			Delegate hook = Delegate.CreateDelegate(typeof(ILContext.Manipulator), modify);
			delegates.Add((orig, hook));
			HookEndpointManager.Modify(orig, hook);
		}

		private static void DetourHook(MethodInfo orig, MethodInfo modify) {
			Hook hook = new(orig, modify);
			detours.Add(hook);
			hook.Apply();
		}
	}
}
