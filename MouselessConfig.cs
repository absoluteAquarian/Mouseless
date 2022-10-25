using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace Mouseless {
	[BackgroundColor(99, 180, 209)]
	[Label("Mouseless Config")]
	internal class MouselessConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;

		private const int OptionMin = -4, OptionMax = 2;

		private int option;
		[Range(OptionMin, OptionMax)]
		[DefaultValue(0)]
		[Slider]
		[Increment(1)]
		[DrawTicks]
		public int SensitivityOption {
			get => option;
			set {
				if (option != value) {
					option = value;

					option = Utils.Clamp(option, OptionMin, OptionMax);
					SensitivityScale = (float)Math.Pow(2, option);
				}
			}
		}

		[JsonIgnore]
		internal float SensitivityScale { get; private set; } = 1f;
	}
}
