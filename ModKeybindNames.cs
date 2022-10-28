using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;

namespace Mouseless {
	internal static class ModKeybindNames {
		private class Loadable : ILoadable {
			public void Load(Mod mod) { }

			public void Unload() {
				names.Clear();
			}
		}

		private static readonly ConditionalWeakTable<ModKeybind, string> names = new();

		// v2022.10+
		public static readonly MethodInfo ModKeybind_get_FullName = typeof(ModKeybind).GetProperty("FullName", BindingFlags.NonPublic | BindingFlags.Instance)?.GetGetMethod(nonPublic: true);
		// v2022.9
		public static readonly FieldInfo ModKeybind_uniqueName = typeof(ModKeybind).GetField("uniqueName", BindingFlags.NonPublic | BindingFlags.Instance);

		public static string GetFullName(this ModKeybind key) {
			if (!names.TryGetValue(key, out string fullName)) {
				if (BuildInfo.tMLVersion >= new Version(2022, 10))
					fullName = ModKeybind_get_FullName.Invoke(key, null) as string;
				else
					fullName = ModKeybind_uniqueName.GetValue(key) as string;

				names.Add(key, fullName);
			}

			return fullName;
		}
	}
}
