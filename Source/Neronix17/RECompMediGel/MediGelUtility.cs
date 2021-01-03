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
                        if (list[i].IsPermanent())
                        {
                            HediffWithComps hediffWithComps = list[i] as HediffWithComps;
                            if (hediffWithComps != null)
                            {
                                HediffComp_TendDuration hediffComp_TendDuration = HediffUtility.TryGetComp<HediffComp_TendDuration>(hediffWithComps);
                                if (hediffComp_TendDuration != null)
                                {
                                    hediffComp_TendDuration.tendQuality = 1.0f;
                                    hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                                }
                                pawn.health.Notify_HediffChanged(list[i]);
                            }
                        }
                        else if (list[i].def.everCurableByItem)
                        {
                            HealthUtility.CureHediff(list[i]);
                        }
                    }
                }
            }
        }
    }
}
