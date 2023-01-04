using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimEffect
{
	public class IncidentWorker_SpectreJoin : IncidentWorker_WandererJoin
	{
        public override Pawn GeneratePawn()
        {
			Gender? fixedGender = null;
			if (def.pawnFixedGender != 0)
			{
				fixedGender = def.pawnFixedGender;
			}
			Pawn spectre = PawnGenerator.GeneratePawn(new PawnGenerationRequest(this.def.pawnKind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, 
                                                                                forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: this.def.pawnMustBeCapableOfViolence, 
                                                                                colonistRelationChanceFactor: 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowFood: true, allowAddictions: true, inhabitant: false, 
                                                                                certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: 1f, 
                                                                                biocodeApparelChance: 0f, extraPawnForExtraRelationChance: null, relationWithExtraPawnChanceFactor: 1f, validatorPreGear: null, allowPregnant: false, 
                                                                                validatorPostGear: null, forcedTraits: null, prohibitedTraits: null, minChanceToRedressWorldPawn: null, fixedBiologicalAge: null, fixedChronologicalAge: null, fixedGender: fixedGender));
			spectre.apparel.LockAll();
			var spectreHediff = HediffMaker.MakeHediff(RE_DefOf.RE_Spectre, spectre) as Hediff_Spectre;
			spectreHediff.tickShouldLeave = (int)(Find.TickManager.TicksAbs + (GenDate.TicksPerDay * Rand.Range(3f, 6f)));
			spectreHediff.tickShouldLeaveMax = (int)(Find.TickManager.TicksAbs + (GenDate.TicksPerDay * Rand.Range(9f, 12f)));
			spectre.health.AddHediff(spectreHediff);
			return spectre;
		}
    }
}
