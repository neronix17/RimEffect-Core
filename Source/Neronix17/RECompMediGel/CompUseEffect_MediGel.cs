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

            MediGelUtility.TrySealWounds(usedBy);
        }
    }
}
