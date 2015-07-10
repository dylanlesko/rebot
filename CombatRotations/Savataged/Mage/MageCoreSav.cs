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
	[Rotation("Mage Core [Savataged]", "Savataged", "v0.1", WoWClass.Mage, Specialization.None, 40)]
	public class MageCoreSav : CombatRotation
	{
		public MageCoreSav()
		{
			OverrideCombatModus = CombatModus.Fighter;
			OverrideCombatRole = CombatRole.DPS;
			
			GroupBuffs = new string[]
			{
				"Arcane Brilliance",
				"Dalaran Brilliance",
				"Conjure Refreshment Table"
			};
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
