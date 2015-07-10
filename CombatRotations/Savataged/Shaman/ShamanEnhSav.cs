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
	[Rotation("Enhancement Shaman [Savataged]", "Savataged", "0.1", WoWClass.Shaman, Specialization.ShamanEnhancement)]
    public class ShamanEnhSav : CombatRotation
	{
		public ShamanEnhSav()
		{
			PullSpells = new string[]
			{
				"Lightning Bolt"
			};
			OverrideCombatModus = CombatModus.Fighter;
            OverrideCombatRole = CombatRole.DPS;
		}

		public override bool OutOfCombat()
		{
			//check for gcd
			if (	HasGlobalCooldown() == true	)
				return true;
			if (	Heal() == true )
				return true;
			if (	Buffs() == true )
				return true;
			if (	GhostWolf() == true )
				return true;
			if (	WaterWalking() == true )
				return true;

			return false;
		}
		
		private boolean Resurrect()
		{
			if (CurrentBotName == "Combat")
			{
				List<PlayerObject> members = Group.GetGroupMemberObjects();
                if (members.Count > 0)
				{
					PlayerObject deadPlayer = members.FirstOrDefault(x => x.IsDead);
					if (Cast("Ancestral Spirit", () => deadPlayer != null, deadPlayer)) return true;
				}
			}
			return false;
		}
		private boolean GhostWolf()
		{
			if (	Me.IsSwimming == false
					&& HasAura("Ghost Wolf") == false
					&& Me.DistanceTo(API.GetNaviTarget()) > 20 )
				if (	CastSelf("Ghost Wolf",
						() => HasSpell("Ghost Wolf")
						&& Me.DisplayId == Me.NativeDisplayId
						&& Me.MovementSpeed > 0 ) == true )
					return true;
			return false;
		}

		private boolean WaterWalking()
		{
			// Only use OnWaterMove Spell, if Navi target is not in water. Cancel buff if we have to dive
			if (API.GetNaviTarget() != Vector3.Zero && HasSpell("Water Walking"))
			{
				if (!API.IsNaviTargetInWater())
				{
					if (CastSelf("Water Walking", () => Me.IsSwimming && !HasAura("Water Walking"))) return true;
				}
				else if (HasAura("Water Walking"))
					CancelAura("Water Walking");
			}
		
			return false;
		}
		private boolean Buffs()
		{		
			if (	!Me.HasWeaponBuff(WeaponEnchantType.MainHand) == true )
			{
				if (	CastSelf("Windfury Weapon",
						() => Me.Level >= 30
						&& HasSpell("Windfury Weapon") ) == true )
					return true;
				if (	CastSelf("Flametongue Weapon",
						() => HasSpell("Windfury Weapon") ) == true )
					return true;
			}
			if (	!Me.HasWeaponBuff(WeaponEnchantType.OffHand) == true 
					&& Equip.OffHand.Exists == true )
			{
				if (	CastSelf("Windfury Weapon",
						() => Me.Level >= 30
						&& HasSpell("Windfury Weapon") ) == true )
					return true;
				if (	CastSelf("Flametongue Weapon",
						() => HasSpell("Windfury Weapon") ) == true )
					return true;
			}			
			
			if (	HasAura("Lightning Shield") == false )
				if (	CastSelf("Lightning Shield",
						() => HasSpell("Lightning Shield") ) == true )
					return true;
			
			if (	CastSelf("Thunderstorm",
					() => Me.ManaFraction < 0.8 ) )
				return true;
					
			return false;
		}
		
		private boolean Heal()
		{
			if (	Target.IsInCombatRangeAndLoS == true )
				if (	CastSelf("Healing Stream Totem",
						() => Me.HealthFraction < 0.5
						&& HasSpell("Healing Stream Totem") ) == true )
					return true;
			if (	Me.HealthFraction < 0.5 )
				if ( CastSelf("Healing Surge",
				() => HasSpell("Healing Surge") ) == true ) 
				return true;
			if (	Me.HealthFraction < 0.55 )
				if ( CastSelf("Astral Shift",
				() => HasSpell("Astral Shift") ) == true ) 
				return true;
			if (	Me.HealthFraction < 0.7 )
				if ( CastSelf("Shamanistic Rage",
				() => HasSpell("Shamanistic Rage") ) == true )
				return true;
			if (	Me.HealthFraction < 0.8 )
				if ( CastSelf("Ancestral Guidance",
				() => HasSpell("Ancestral Guidance") ) == true )
				return true;
			return false;
		}
		
		private boolean OffensiveCooldowns()
		{
			if (	!Target.IsInCombatRangeAndLoS == true )
				if (	CastSelf("Spiritwalker's Grace",
						() => HasSpell("Spiritwalker's Grace") ) ==  true )
						return true;
			if (	Target.IsInCombatRangeAndLoS == true 
					&& HasAura("Ascendance") == false )
				if (	CastSelf("Ascendance",
						() => HasSpell("Ascendance") ) ==  true )
						return true;
			if (	Target.IsElite() == true
					&& Target.HealthFraction > 0.6 )
				if ( CastSelf("Bloodlust",
					() => HasSpell("Bloodlust") ) == true )
					return true;
			return false;
		}
		private boolean Totems()
		{
            if (	CastSelf("Searing Totem",
					() => Adds.Count <= 1 
					&& (Target.MaxHealth > Me.MaxHealth 
					|| Target.CombatRange > 10) 
					&& Target.CombatRange <= 22 
					&& !Me.TotemExist(TotemType.Fire_M1_DeathKnightGhoul) ) == true ) 
				return true;
			if (	CastSelf("Magma Totem",
					() => Adds.Count >= 2
					&& !Me.TotemExist(TotemType.Fire_M1_DeathKnightGhoul) ) == true )
				return true;
			
			return false;
		}
		private boolean DamageRoutine()
		{
            if (	CastPreventDouble("Chain Lightning", 
					() => Adds.Count >= 4 
					&& HasAura("Maelstrom Weapon", false, 5)
					&& HasSpell("Chain Lightning") ) == true )
				return true;
            if (	CastPreventDouble("Lightning Bolt",
					() => HasAura("Maelstrom Weapon", false, 5)
					&& HasSpell("Lightning Bolt") ) == true )
				return true;

			if (	Cast("Stormstrike",
					() => HasSpell("Stormstrike") ) == true )
				return true;
				
            if (	Cast("Lava Lash",
					() => Equip.OffHand.Exists) == true )
				return true;
			if (	Cast("Primal Strike", 
					() => !HasSpell("Stormstrike") ) == true )
				return true;
            if (	Cast("Flame Shock", 
					() => Target.AuraTimeRemaining("Flame Shock") <= 5 
					&& Me.TotemExist(TotemType.Fire_M1_DeathKnightGhoul) ) == true ) 
				return true;
			
			if (	Cast("Unleash Elements") == true ) 
				return true;
			if (	Cast("Frost Shock") == true ) 
				return true;

            if (	CastPreventDouble("Lightning Bolt", 
					() => HasAura("Maelstrom Weapon", false, 1) ) == true )
			return true;
				
			return false;
		}
		
		private boolean Interrupt()
		{		
			if (	Target.IsInCombatRangeAndLoS == false 
					|| Target.RemainingCastTime < 100 )
				return false;
			if (	Target.IsCastingAndInterruptible() == true )	
				if (	CastSelfPreventDouble("Wind Shear",
						() => HasSpell("Wind Shear") == true,
						1500 ) == true )
					return true;
			if (	CastSelfPreventDouble("Earthbind Totem",
					() => HasSpell("Earthbind Totem") == true,
					1500 ) == true )
				return true;	
			return false;		
		}
		
		public override void Combat()
		{
			//check for gcd
			if (	HasGlobalCooldown() == true	)
				return;
			//check if target is casting
			if (	Target.Target.IsCasting() == true )
			{
				if (	Interrupt() == true )
					return;
			}
			
			return;
		}

		public override bool AfterCombat()
		{
			if (	CastSelf("Totemic Recall",
					() => Me.TotemExist(TotemType.Air, TotemType.Earth_M2, TotemType.Fire_M1_DeathKnightGhoul, TotemType.Water_M3) ) == true )
				return true;
			return false;
		}
	}
}
