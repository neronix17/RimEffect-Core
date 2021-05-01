namespace RimEffect
{
    using HarmonyLib;
    using RimWorld;
    using System.Collections.Generic;
    using Verse;

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.CanInteractNowWith))]
    public static class CanInteractNowWith_Patch
    {
        public static bool Prefix(ref bool __result, Pawn recipient, InteractionDef interactionDef = null)
        {
            if (Building_VIInterface.terminals.TryGetValue(recipient, out Building_VIInterface vIInterface) && vIInterface.Spawned)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
