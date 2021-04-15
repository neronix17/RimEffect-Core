using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using VFEMech;

namespace RimEffect
{
    public class SpectreArmor : Apparel
    {
        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            var shield = this.GetComp<ShieldMechBubble>();
            if (shield != null)
            {
                return shield.AbsorbingDamage(dinfo, out bool absorbed);
            }
            else
            {
                return base.CheckPreAbsorbDamage(dinfo);
            }
        }
        public override void DrawWornExtras()
        {
            base.DrawWornExtras();
            Comps_PostDraw();
        }
    }
}
