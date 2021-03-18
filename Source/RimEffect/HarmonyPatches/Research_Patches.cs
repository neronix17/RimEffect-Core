using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimEffect
{
    [HarmonyPatch(typeof(ResearchProjectDef), "CanBeResearchedAt")]
    public static class Patch_CanBeResearchedAt_Postfix
    {
        public static void Postfix(ResearchProjectDef __instance, ref bool __result, Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus)
        {
            if (!__result)
            {
                if (bench.def == RE_DefOf.RE_SpacerTechResearchBench)
                {
                    __result = CanBeResearchedAt(__instance, bench, ignoreResearchBenchPowerStatus);
                }
            }
        }

        private static bool CanBeResearchedAt(ResearchProjectDef def, Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus)
		{
			if (!ignoreResearchBenchPowerStatus)
			{
				CompPowerTrader comp = bench.GetComp<CompPowerTrader>();
				if (comp != null && !comp.PowerOn)
				{
					return false;
				}
			}
			if (!def.requiredResearchFacilities.NullOrEmpty())
			{
				CompAffectedByFacilities affectedByFacilities = bench.TryGetComp<CompAffectedByFacilities>();
				if (affectedByFacilities == null)
				{
					return false;
				}
				List<Thing> linkedFacilitiesListForReading = affectedByFacilities.LinkedFacilitiesListForReading;
				int i;
				for (i = 0; i < def.requiredResearchFacilities.Count; i++)
				{
					if (linkedFacilitiesListForReading.Find((Thing x) => x.def == def.requiredResearchFacilities[i] && affectedByFacilities.IsFacilityActive(x)) == null)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

}
