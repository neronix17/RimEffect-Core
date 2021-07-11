using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimEffect
{
	internal class DamageWorker_BombNoShake : DamageWorker_AddInjury
	{
		public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
		{
			if (def.explosionHeatEnergyPerCell > float.Epsilon)
			{
				GenTemperature.PushHeat(explosion.Position, explosion.Map, def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
			}
			FleckMaker.Static(explosion.Position, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
			ExplosionVisualEffectCenter(explosion);
		}
	}
}
