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
	[Rotation("My Frost", "Savataged", "v1.0", WoWClass.Mage, Specialization.MageFrost, 40)]
	public class MyFrost : CombatRotation
	{
		[JsonProperty("Use Time Warp")]
		public bool UseTimeWarp = true;

		public override bool OutOfCombat()
		{
			//check for gcd
			if (	HasBlobalCooldown() == true	)
				return true;
				
			//check if self is falling
			if (	Me.FallingTime > 2 
					&& HasAura("Slow Fall") == false )
			{		
				//cast slow fall on self
				if (	CastSelfPreventDouble("Slow Fall") == true,
						() => HasSpell("Slow Fall") == true,
						500 )
					return true;					
			}
			
			//check for water elemental
			if (	Me.HasAlivePet == false )
			{
				if (	CastSelfPreventDouble("Summon Water Elemental") == true,
						() => HasSpell("Summon Water Elemental") == true,
						1500 )
					return true;
			}
				
			//check if self has int buff
			if (	HasAura("Arcane Brilliance") == false
					&& HasAura("Dalaran Brilliance") == false )
			{		
				//cast int buff on self
				if (	CastSelfPreventDouble("Arcane Brilliance") == true,
						() => HasSpell("Arcane Brilliance") == true,
						1500 )
					return true;
				if (	CastSelfPreventDouble("Dalaran Brilliance") == true,
						() => HasSpell("Dalaran Brilliance") == true,
						1500 )
					return true;						
			}
				
			//check if ice barrier is active
			if (	HasAura("Ice Barrier") == false )
					if (	CastSelfPreventDouble("Ice Barrier") == true,
							() => HasSpell("Ice Barrier") == true,
							1500 )
						return true;	
						
			//end of OutOfCombat
			return false;
		}
		
		public override void Combat()
		{
			
		}
	}
}
