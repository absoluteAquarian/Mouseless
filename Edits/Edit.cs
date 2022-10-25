using Terraria.ModLoader;

namespace Mouseless.Edits {
	public abstract class Edit : ILoadable {
		public Mod Mod { get; private set; }

		void ILoadable.Load(Mod mod) {
			Mod = mod;

			LoadEdits();
		}

		void ILoadable.Unload() {
			UnloadEdits();

			Mod = null;
		}

		public abstract void LoadEdits();

		public abstract void UnloadEdits();
	}
}
