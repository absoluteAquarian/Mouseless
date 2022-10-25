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

		public static readonly MethodInfo ModKeybind_get_FullName = typeof(ModKeybind).GetProperty("FullName", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod();

		public static string GetFullName(this ModKeybind key) {
			if (!names.TryGetValue(key, out string fullName))
				names.Add(key, fullName = ModKeybind_get_FullName.Invoke(key, null) as string);

			return fullName;
		}
	}
}
