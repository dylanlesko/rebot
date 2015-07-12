using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ReBot.API;
using System.ComponentModel;
using Geometry;

    public enum WarlockPet
    {
        AutoSelect = 0,
        SoulImp,
        Voidwalker,
        Succubus,
        Felhunter,
        Felguard,
        Infernal,
        Doomguard,
    }

    //Display IDs
    public enum WlPetDisplayId
    {
        SoulImp = 4449,
        Voidwalker = 1132,
        Felhunter = 850,
        Succubus = 4162,
        Felguard = 61493,
        Infernal = 169, // Infernal
        Doomguard = 1912, //Doomguard
        ImpSoulImp = 44152,
        ImpVoidwalker = 44542,
        ImpFelhunter = 44153,
        ImpSuccubus = 44610,
        ImpFelguard = 44609,
        ImpInfernal = 51650,
        ImpDoomguard = 22809,
    }



namespace ReBot
{
	[Rotation("Demonology Warlock [Savataged]", "Savataged", "v0.1", WoWClass.Warlock, Specialization.WarlockDemonology, 40)]
	public class WarlockDemonologySav : CombatRotation
	{
		public WarlockDemonologySav()
		{
			OverrideCombatModus = CombatModus.Fighter;
			OverrideCombatRole = CombatRole.DPS;
			
			PullSpells = new string[]
			{
				"Shadow Bolt"
			};			
			
			GroupBuffs = new string[]
			{
				"Dark Intent",
				(CurrentBotName == "PvP" ? "Create Soulwell" : null)
			};
		}	

		[JsonProperty("SelectedPet"), JsonConverter(typeof(StringEnumConverter))]
        public WarlockPet SelectedPet = WarlockPet.AutoSelect;
	    [JsonProperty("UsePet")] 
        public bool UsePet = true;
		
		public bool Buffs()
		{
			if (	HasSpell("Unending Breath") == true 
					&& Me.IsSwimming == true )
				if (	CastSelf("Unending Breath", 
						() => HasAura("Unending Breath") ) == true )
					return true;				
			if (	HasSpell("Fire and Brimstone") == true )
				if (	CastSelf("Fire and Brimstone", 
						() => HasAura("Fire and Brimstone") ) == true )
					return true;
					
			if (	HasSpell("Dark Intent") == true )
				if (	CastSelf("Dark Intent", 
						() => HasAura("Dark Intent") ) == true )
					return true;
		}
		
		public bool PetManagement()
		{
			if (	HasSpell("Grimoire of Sacrifice") == true )
			{
					if (	Me.HasAlivePet == true )
						if (	CastSelf("Grimoire of Sacrifice" ) == true )
							return true;
						else
							if (	SummonPets() == true )
								if (	CastSelf("Grimoire of Sacrifice" ) == true )
									return true;
					return false;
			}
			else
				if (	SummonPets() == true )
					return true;
			return false;
		}
		
		public bool SummonPets( )
		{
		
			if ( Me.HasAlivePet == true )
				return true;
				
			string spell = null;

            if (pet == WarlockPet.AutoSelect)
				if ( HasSpell("Demonic Servitude") == true )
				{
                    pet = WarlockPet.Infernal;
					spell = "Summon Doomguard";
				}
				else if ( HasSpell("Summon Felguard") == true )
				{
                    pet = WarlockPet.Felguard;
					spell = "Summon Felguard";
				}
                else if ( HasSpell("Summon Felhunter") == true )
				{
                    pet = WarlockPet.Felhunter;
					spell = "Summon Felhunter";
                }
                else if (  HasSpell("Summon Voidwalker") == true )
				{
                    pet = WarlockPet.Voidwalker;
					spell = "Summon Voidwalker";
				}
                else if ( HasSpell("Summon Imp") == true )
				{
                    pet = WarlockPet.SoulImp;
					spell = "Summon Imp";
				}
                else
                    return false; // we can not summon a pet
					
			if ( spell != null )
				if (	CastSelfPreventDouble(spell) == true )
					return true;
			
			return false;
		}

		public override bool OutOfCombat()
		{
			//check for gcd
			if (	HasGlobalCooldown() == true	)
				return true;
			
			if (	PetManagement() == true )
				return true;
			
			if (	HasSpell("Create Helathstone" ) == true )
				if ( 	CastSelfPreventDouble("Create Healthstone", 
						() => Inventory.Healthstone == null,
						10000 ) == true )
					return true;
						
			//end of OutOfCombat
			return false;
		}
		
		public override void Combat()
		{
			//check for gcd
			if (	HasGlobalCooldown() == true	)
				return;
			if (	DefensiveCoolDowns() == true )
				return;
			if ( 	PetManagement == true )
				return;
			if (	DamageRoutine() == true )
				return;
				
			
			//end of Combat
			return;	
		}
		
		public override bool AfterCombat()
		{
			if (	CastSelf("Metamorphosis", 
					() => HasAura("Metamorphosis") == true ) == true )
				return true;
			return false;
		}
		
		private bool DefensiveCoolDowns()
		{
			if (	HasSpell("Unending Resolve") == true )
				if (	CastSelf("Unending Resolve",
						() => Me.HealthFraction <= 0.5 ) == true )
					return true;
			if (	HasSpell("Mortal Coil") == true )
				if (	Cast("Mortal Coil",
						() => Me.HealthFraction <= 0.5 ) == true )
					return true;
			if (	HasSpell("Dark Regeneration") == true )
				if (	CastSelf("Dark Regeneration",
						() => Me.HealthFraction <= 0.6 ) == true )
					return true;
			if (	HasSpell("Drain Life") == true )
				if (	CastPreventDouble("Drain Life",
						() => Me.HealthFraction <= 0.5,
						1000 ) == true )
					return true;	

			//end of OffensiveCoolDowns
			return false;
		}
		
		private bool OffensiveCoolDowns()
		{
			
			//end of OffensiveCoolDowns
			return false;
		}
		
		private bool Metamorphosis()
		{
			if (	HasSpell("Metamorphosis") == true )
				if (	HasAura("Metamorphosis") == true )
					if (	Me.GetPower(WoWPowerType.WarlockDemonicFury) < 750 )
						if (	HasSpell("Dark Soul: Knowledge") == true )
							if (	CastSelf("Metamorphosis",
									() => HasAura("Dark Soul: Knowledge") == false ) == true )
								return true;
							else
								return false;
						else
							if (	CastSelf("Metamorphosis") == true )
								return true;			
				else
				    if (	CastSelf("Metamorphosis", 
							() => Me.GetPower(WoWPowerType.WarlockDemonicFury) >= 900)) 
					return true;
			return false;
		}
		
		private bool DamageRoutine()
		{
			if (	Metamorphosis() == true )
				return true;
			if (	HasAura("Metamorphosis") == true )
			{
				if (	HasSpell("Doom") == true )
					if (	Cast("Doom",
							() => Target.AuraTimeRemaining("Doom") <= 18 ) == true )
						return true;
	
				if (	HasSpell("Touch of Chaos") == true )
					if (	Cast("Touch of Chaos",
							() => Target.AuraTimeRemaining("Corruption") <= 5 ) == true )
						return true;				
				if (	HasSpell("Soulfire") == true )
					if (	Cast("Soulfire",
							() => Target.HealthFraction < 0.25 
							|| Me.HasAura("Molten Core") == true ) == true )
						return true;				
				if (	HasSpell("Touch of Chaos") == true )
					if (	Cast("Touch of Chaos") == true )
						return true;				
			}
			else
			{
				if (	HasSpell("Corruption") == true )
					if (	Cast("Corruption",
							() => Target.HasAura("Corruption") == false ) == true )
						return true;
				if (	HasSpell("Hand of Gul'dan") == true )
						if (	Cast("Hand of Gul'dan") == true )
							return true;				
				if (	Cast("Soul Fire",
						() => Target.HealthFraction < 0.25 
						|| Me.Auras["Molten Core"].StackCount > 4) == true ) 
					return true;
				if (	HasSpell("Cataclysm") == true )
					if (	CastOnTerrainPreventDouble("Cataclysm", Target.Position) == true ) 
					return true;
				if (	HasSpell("Shadow Bolt") == true )
					if (	Cast("Shadow Bolt") == true )
						return true;	
			}
	
			//end of Damageroutine
			return false;

		}
	}
}
