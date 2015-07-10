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
	[Rotation("Frost Mage [Savataged]", "Savataged", "v0.1", WoWClass.Mage, Specialization.MageFrost, 40)]
	public class MageFrostSav : CombatRotation
	{
		public MageFrostSav()
		{
			OverrideCombatModus = CombatModus.Fighter;
			OverrideCombatRole = CombatRole.DPS;
			
			PullSpells = new string[]
			{
				"Frostbolt"
			};			
			
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
			//check for gcd
			if (	HasGlobalCooldown() == true	)
				return true;
			
			//check if falling
			if (	SlowFall() == true )
				return true;
				
			//summon elemental
			if (	SummonElemental() == true )
				return true;

			//check if self has int buff
			if (	HasAura("Arcane Brilliance") == false
					&& HasAura("Dalaran Brilliance") == false )
			{		
				//cast int buff on self
				if (	CastSelfPreventDouble("Arcane Brilliance",
						() => HasSpell("Arcane Brilliance") == true,
						1500 ) == true )
					return true;
				if (	CastSelfPreventDouble("Dalaran Brilliance",
						() => HasSpell("Dalaran Brilliance") == true,
						1500 ) == true )
					return true;						
			}
			
			//cast icebarrier
			if (	IceBarrier() == true )
				return true;	
						
			//end of OutOfCombat
			return false;
		}
		
		public override void Combat()
		{
			//check for gcd
			if (	HasGlobalCooldown() == true	)
				return;
			
			//check if falling
			if (	SlowFall() == true )
				return;
				
			//summon elemental
			if (	SummonElemental() == true )
				return;
				
			//cast icebarrier
			if (	IceBarrier() == true )
				return;	
						
			//check if target is casting
			if (	Target.IsCastingAndInterruptible() == true )
			{
				if (	Interrupt() == true )
					return;
			}
			
			//defensive cooldowns
			if (	DefensiveCoolDowns() == true )
				return;
				
			//offensive cooldowns
			if (	OffensiveCoolDowns() == true )
				return;
			
			//Damage Routine
			if (	DamageRoutine() == true )
				return;
				
			if (	HasAura("Fingers of Frost") == true )
				if (	Cast("Ice Lance",
					() => HasSpell("Ice Lance") ) == true )
				return;
				
			if (	Cast("Frostbolt",
					() => HasSpell("Frostbolt") ) == true )
				return;
			if (	Cast("Frostfire Bolt",
					() => HasSpell("Frostfire Bolt") ) == true )
				return;
			
			//end of Combat
			return;	
		}
		
		private bool SlowFall()
		{
			//check if self is falling
			if (	Me.FallingTime > 2 
					&& HasAura("Slow Fall") == false )
			{		
				//cast slow fall on self
				if (	CastSelfPreventDouble("Slow Fall",
						() => HasSpell("Slow Fall") == true,
						500 ) == true )
				return true;					
			}
			
			//end of SlowFall
			return false;
		}
		
		private bool SummonElemental()
		{
			//check for water elemental
			if (	Me.HasAlivePet == false )
				if (	CastSelfPreventDouble("Summon Water Elemental",
						() => HasSpell("Summon Water Elemental") == true,
						1500 ) == true )
				return true;
			
			//end of SummonElemental
			return false;
		}
		
		private bool IceBarrier()
		{
			//check if ice barrier is active
			if (	HasAura("Ice Barrier") == false )
				if (	CastSelfPreventDouble("Ice Barrier",
						() => HasSpell("Ice Barrier") == true,
						1500 ) == true )
				return true;
					
			//end of IceBarrier
			return false;
		}
		
		private bool Interrupt()
		{
			if (	Target.IsInCombatRangeAndLoS == false 
					|| Target.RemainingCastTime < 100 )
				return false;
			
			if (	CastPreventDouble("Counterspell",
					() => HasSpell("Counterspell") == true,
					1500 ) == true )
				return true;
			if (	CastPreventDouble("Frostjaw",
					() => HasSpell("Frostjaw") == true,
					1500 ) == true )
				return true;		
	
			//end of Interrupt
			return false;
		}
		
		private bool DefensiveCoolDowns()
		{
			if (	Me.CanParticipateInCombat == true
					&& Me.HealthFraction > 0.8 )
				return false;
			if (	Me.HealthFraction < 0.4 )
				if (	CastSelf("Cold Snap",
						() => HasSpell("Cold Snap") ) == true )
				return true;
			if (	Me.HealthFraction < 0.5 
					&& HasAura("Temporal Shield") == false )
				if (	CastSelf("Temporal Shield",
						() => HasSpell("Temporal Shield") ) == true )
				return true;
			if (	Me.HealthFraction < 0.6 
					&& Target.Target == Me )
				if (	CastSelf("Evanesce",
						() => HasSpell("Evanesce") ) == true )
				return true;
			if (	IceBarrier() == true )
				return true;
			if (	CastSelf("Blink", 
					() => Me.CanNotParticipateInCombat() ) == true ) 
				return true;

		
			//end of DefensiveCoolDowns
			return false;
		}
		
		private bool OffensiveCoolDowns()
		{
			if (	CastSelf("Icy Veins",
					() => Target.HpGreaterThanOrElite(0.5) ) == true )
				return true;
			if (	Target.IsElite() == true )
				if (	CastSelf("Time Warp", 
						() => HasSpell("Time Warp") ) == true )
				return true;
			if (	Target.IsElite() == true )
				if (	CastSelf("Mirror Image", 
						() => HasSpell("Mirror Image") ) == true )
				return true;
			if (	Target.HasAura("Freeze") == true 
					&& HasSpell("Presence of Mind") == true )
				if (	Cast("Frostbolt",
					() => HasAura("Presence of Mind") ) == true) 
				return true;
				
			if (	CastOnTerrain("Freeze",
					Target.Position,
					() => HasSpell("Freeze") ) == true )
				return true;
				
			if (	Cast("Frostfire Bolt",
					() => HasAura("Brain Freeze") ) == true )
				return true;
				
			//end of OffensiveCoolDowns
			return false;
		}
		
		private bool DamageRoutine()
		{
			if (	Cast("Ice Lance", 
					() => Target.HasAura("Freeze") ) == true )
				return true;
			if (	HasSpell("Presence of Mind") == true )
			{
				if (	CastSelf("Presence of Mind",
						() => Target.HealthFraction < 0.95 ) == true )
					return true;				
				if (	Cast("Frostbolt",
						() => HasAura("Presence of Mind")) == true) 
					return true;
			}
			if (	Cast("Deep Freeze",
					() => Target.HasAura("Freeze") 
					|| Target.HasAura("Frost Nova")) == true ) 
				return true;
			if (	Cast("Ice Lance",
					() => Target.HasAura("Freeze") 
					|| Target.HasAura("Frost Nova")) == true ) 
				return true;
			if (	Cast("Frozen Orb",
					() => Target.HealthFraction > 0.9 
					&& Target.IsInLoS) == true )
				return true;

			//end of Damageroutine
			return false;

		}
	}
}
