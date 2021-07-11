namespace RimEffect
{
    using System.Reflection;
    using HarmonyLib;
    using RimWorld;
    using Verse;

    [HarmonyPatch]
    public static class CanEquip_Patch
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => 
            AccessTools.Method(typeof(EquipmentUtility), nameof(EquipmentUtility.CanEquip), new[] {typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool)});


        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
        {
            if (__result)
            {
                if (thing.TryGetComp<CompEquippable>() != null)
                {
                    if (thing.GetStatValue(StatDefOf.Mass) > 2.5f * pawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation) && pawn.health.hediffSet.HasHediff(RE_DefOf.RE_BioticAmpHediff))
                    {
                        __result   = false;
                        cantReason = "RE.BioticAmpCantEquipWeaponWeightReason".Translate();
                    }
                }
            }    
        }
    }
}
