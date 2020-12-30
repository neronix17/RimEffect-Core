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
    public class CompUseEffect_MediGel : CompUseEffect
	{
		public override void DoEffect(Pawn usedBy)
		{
			base.DoEffect(usedBy);

            TrySealWounds(usedBy);
        }

        public static void TrySealWounds(Pawn pawn)
        {
            IEnumerable<Hediff> enumerable = from hd in pawn.health.hediffSet.hediffs
                                             where hd.TendableNow()
                                             select hd;
            if (enumerable != null)
            {
                foreach (Hediff hediff in enumerable)
                {
                    if (hediff != null)
                    {
                        HediffWithComps hediffWithComps = hediff as HediffWithComps;
                        if (hediffWithComps != null)
                        {
                            HediffComp_TendDuration hediffComp_TendDuration = HediffUtility.TryGetComp<HediffComp_TendDuration>(hediffWithComps);
                            if (hediffComp_TendDuration != null)
                            {
                                hediffComp_TendDuration.tendQuality = 1.0f;
                                hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                            }
                            pawn.health.Notify_HediffChanged(hediff);
                        }
                    }
                }
            }
        }
    }
}
