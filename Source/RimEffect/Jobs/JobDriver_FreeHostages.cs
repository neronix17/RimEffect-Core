using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimEffect
{
    public class JobDriver_FreeHostages : JobDriver
    {
        public Pawn OtherPawn
        {
            get
            {
                return (Pawn)this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOn(() => !this.OtherPawn.health.hediffSet.HasHediff(RE_DefOf.RE_TurnBackToFormerFaction));

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil finalize = new Toil();
            finalize.initAction = delegate ()
            {
                IntVec3 leavePoint;
                CellFinder.TryFindRandomPawnExitCell(OtherPawn, out leavePoint);
                // Turn back to former faction and villager + exit the map

                foreach (Pawn pawn in Map.mapPawns.AllPawnsSpawned.FindAll(p => p.Position.GetRoom(Map) == this.OtherPawn.Position.GetRoom(Map) 
                && p.health.hediffSet.HasHediff(RE_DefOf.RE_TurnBackToFormerFaction)))
                {
                    pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs.Find(h => h.def == RE_DefOf.RE_TurnBackToFormerFaction));
                    pawn.guest.SetGuestStatus(null);
                    pawn.jobs.TryTakeOrderedJob(new Job(RE_DefOf.RE_LeaveMap, leavePoint));
                }
            };
            finalize.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return finalize;
            yield break;
        }
    }
}