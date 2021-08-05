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
                    if (list[i] != null && !list[i].def.makesSickThought && !list[i].def.chronic)
                    {
                        if (list[i].def.hediffClass == typeof(Hediff_MissingPart))
                        {
                            list[i].Tended(1.0f, 1.0f);
                        }
                        else if (!list[i].IsPermanent() && list[i].def.everCurableByItem)
                        {
                            HealthUtility.Cure(list[i]);
                        }
                    }
                }
            }
        }
        public static bool CanSealWounds(Pawn pawn)
        {
            IEnumerable<Hediff> enumerable = from hd in pawn.health.hediffSet.hediffs
                                             where hd.TendableNow()
                                             select hd;
            if (enumerable != null)
            {
                List<Hediff> list = enumerable.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null && !list[i].def.makesSickThought && !list[i].def.chronic)
                    {
                        if (list[i].def.hediffClass == typeof(Hediff_MissingPart))
                        {
                            return false;
                        }
                        else if (!list[i].IsPermanent() && list[i].def.everCurableByItem)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static void TendAdditional(Pawn doctor, Pawn patient)
        {
            if (doctor != null && doctor.Faction == Faction.OfPlayer && patient.Faction != doctor.Faction && !patient.IsPrisoner && patient.Faction != null)
            {
                patient.mindState.timesGuestTendedToByPlayer++;
            }
            if (doctor != null && doctor.Faction == Faction.OfPlayer && patient.Faction != doctor.Faction && !patient.IsPrisoner && patient.Faction != null)
            {
                patient.mindState.timesGuestTendedToByPlayer++;
            }
            if (doctor != null && doctor.RaceProps.Humanlike && patient.RaceProps.Animal && patient.RaceProps.playerCanChangeMaster && RelationsUtility.TryDevelopBondRelation(doctor, patient, 0.004f) && doctor.Faction != null && doctor.Faction != patient.Faction)
            {
                InteractionWorker_RecruitAttempt.DoRecruit(doctor, patient, false);
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
            if (ModsConfig.IdeologyActive && doctor != null && doctor.Ideo != null)
            {
                Precept_Role role = doctor.Ideo.GetRole(doctor);
                if (role != null && role.def.roleEffects != null)
                {
                    foreach (RoleEffect roleEffect in role.def.roleEffects)
                    {
                        roleEffect.Notify_Tended(doctor, patient);
                    }
                }
            }
            if (doctor != null && doctor.Faction == Faction.OfPlayer && doctor != patient)
            {
                QuestUtility.SendQuestTargetSignals(patient.questTags, "PlayerTended", patient.Named("SUBJECT"));
            }
        }
    }
}
