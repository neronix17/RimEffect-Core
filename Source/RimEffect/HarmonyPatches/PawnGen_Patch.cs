namespace RimEffect
{
    using HarmonyLib;
    using RimWorld;
    using Verse;

    [HarmonyPatch(typeof(MechClusterGenerator), nameof(MechClusterGenerator.MechKindSuitableForCluster))]
    public class MechSpawn_Patch
    {

        [HarmonyPostfix]
        public static void Postfix(PawnKindDef __0, ref bool __result) =>
            __result &= !__0.race.defName.StartsWith("RE_Mechanoids_");
    }
}