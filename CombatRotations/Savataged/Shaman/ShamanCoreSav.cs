using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ReBot.API;
using System.ComponentModel;
using Geometry;

namespace ReBot
{
	[Rotation("Shaman Core [Savataged]", "Savataged", "v0.1", WoWClass.Shaman, Specialization.None, 40)]
	public class ShamanCoreSav : CombatRotation
	{
		public ShamanCoreSav()
		{
			OverrideCombatModus = CombatModus.Fighter;
			OverrideCombatRole = CombatRole.DPS;
		}	
		[JsonProperty("Use Time Warp")]
		public bool UseTimeWarp = true;

		public override bool OutOfCombat()
		{
			return false;
		}

		public override void Combat()
		{
			return;
		}
	}
}
