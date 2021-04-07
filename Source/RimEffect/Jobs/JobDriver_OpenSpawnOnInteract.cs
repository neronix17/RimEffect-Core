using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimEffect
{
    public class JobDriver_OpenSpawnOnInteract : JobDriver
    {
        private Thing Openable
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(this.Openable, job, errorOnFailed: errorOnFailed);
        } 

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_General.Wait(400, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f); ;
            Toil finalize = new Toil();
            finalize.initAction = delegate ()
            {
                if (this.Openable.TryGetComp<CompSpawnOnInteract>() is CompSpawnOnInteract comp && comp != null)
                {
                    comp.ShouldSpawn = true;
                }
            };
            finalize.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return finalize;
            yield break;
        }
    }
}