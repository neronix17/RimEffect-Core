using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimEffect.GenSteps
{
    public class JobGiver_WanderHostage : JobGiver_Wander
    {
        public JobGiver_WanderHostage()
        {
            wanderRadius = 7f;
            ticksBetweenWandersRange = new IntRange(125, 200);
            locomotionUrgency = LocomotionUrgency.Amble;
            wanderDestValidator = (Pawn pawn, IntVec3 loc, IntVec3 root) => WanderRoomUtility.IsValidWanderDest(pawn, loc, root) && loc.Roofed(pawn.Map);
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            return pawn.Position;
        }
    }

    [HarmonyPatch(typeof(JobGiver_Wander), "TryGiveJob")]
    public static class JobGiver_Wander_TryGiveJob
    {
        public static bool Prefix(JobGiver_Wander __instance, Pawn pawn, ref Job __result)
        {
            if (__instance is JobGiver_WanderColony && pawn.health.hediffSet.HasHediff(RE_DefOf.RE_TurnBackToFormerFaction))
            {
                var jbg = new JobGiver_WanderHostage();
                jbg.ResolveReferences();
                __result = jbg.TryIssueJobPackage(pawn, default).Job;
                return false;
            }
            return true;
        }
    }
}