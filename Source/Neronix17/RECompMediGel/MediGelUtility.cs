using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace RECompMediGel
{
    public static class MediGelUtility
    {
        public static void TrySealWounds(Pawn pawn)
        {
            IEnumerable<Hediff> enumerable = from hd in pawn.health.hediffSet.hediffs
                                             where hd.TendableNow()
                                             select hd;
            if (enumerable != null)
            {
                List<Hediff> list = enumerable.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        if (list[i].def.hediffClass == typeof(Hediff_MissingPart))
                        {
                            list[i].Tended_NewTemp(1.0f, 1.0f);
                        }
                        //else if(list[i].def.tendable)
                        //{
                        //    HediffWithComps hediffWithComps = list[i] as HediffWithComps;
                        //    if (hediffWithComps != null)
                        //    {
                        //        HediffComp_TendDuration hediffComp_TendDuration = HediffUtility.TryGetComp<HediffComp_TendDuration>(hediffWithComps);
                        //        if (hediffComp_TendDuration != null)
                        //        {
                        //            hediffComp_TendDuration.tendQuality = 1.0f;
                        //            hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                        //        }
                        //        pawn.health.Notify_HediffChanged(list[i]);
                        //    }
                        //}
                        else if (!list[i].IsPermanent() && list[i].def.everCurableByItem)
                        {
                            HealthUtility.CureHediff(list[i]);
                        }
                    }
                }
            }
        }

        public static void TendAdditional(Pawn doctor, Pawn patient)
        {
            if (doctor != null && doctor.Faction == Faction.OfPlayer && patient.Faction != doctor.Faction && !patient.IsPrisoner && patient.Faction != null)
            {
                patient.mindState.timesGuestTendedToByPlayer++;
            }
            if (doctor != null && doctor.IsColonistPlayerControlled)
            {
                patient.records.AccumulateStoryEvent(StoryEventDefOf.TendedByPlayer);
            }
            if (doctor != null && doctor.RaceProps.Humanlike && patient.RaceProps.Animal && RelationsUtility.TryDevelopBondRelation(doctor, patient, 0.004f) && doctor.Faction != null && doctor.Faction != patient.Faction)
            {
                InteractionWorker_RecruitAttempt.DoRecruit(doctor, patient, 1f, false);
            }
            patient.records.Increment(RecordDefOf.TimesTendedTo);
            if (doctor != null)
            {
                doctor.records.Increment(RecordDefOf.TimesTendedOther);
            }
            if (doctor == patient && !doctor.Dead)
            {
                doctor.mindState.Notify_SelfTended();
            }
        }
    }
}
