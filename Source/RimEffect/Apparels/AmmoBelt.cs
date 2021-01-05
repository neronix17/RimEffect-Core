using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimEffect
{
    public class AmmoBelt : Apparel
    {
        public static Dictionary<Thing, AmmoBelt> pawnsWithAmmobelts = new Dictionary<Thing, AmmoBelt>();
        
        private Pawn wearer;
        public override void Tick()
        {
            if (!this.Destroyed && this.HitPoints > 0 && this.Wearer != wearer)
            {
                var keysToRemove = new List<Thing>();
                foreach (var ammoBelts in pawnsWithAmmobelts)
                {
                    if (ammoBelts.Value == this)
                    {
                        keysToRemove.Add(ammoBelts.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    pawnsWithAmmobelts.Remove(key);
                }

                pawnsWithAmmobelts[this.Wearer] = this;
                wearer = this.Wearer;
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.Wearer != null)
            {
                pawnsWithAmmobelts.Remove(this.Wearer);
            }
            base.Destroy(mode);
        }
    }


    [HarmonyPatch(typeof(Projectile), nameof(Projectile.Launch), new Type[] 
    {
        typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(Thing), typeof(ThingDef)
    })]
    internal static class Launch_Patch
    {
        private static void Prefix(Projectile __instance, Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget,
            ProjectileHitFlags hitFlags, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            if (launcher != null && AmmoBelt.pawnsWithAmmobelts.TryGetValue(launcher, out AmmoBelt ammoBelt))
            {
                ammoBelt.HitPoints -= 1;
                if (ammoBelt.HitPoints <= 0)
                {
                    AmmoBelt.pawnsWithAmmobelts.Remove(launcher);
                    ammoBelt.Destroy();
                }
            }
        }
    }
    [HarmonyPatch(typeof(Projectile), "Impact")]
    internal static class Impact_Patch
    {
        private static void Prefix(Projectile __instance, Thing hitThing)
        {
            if (__instance.Launcher != null && AmmoBelt.pawnsWithAmmobelts.TryGetValue(__instance.Launcher, out AmmoBelt ammoBelt))
            {
                if (ammoBelt.def == RE_DefOf.RE_AmmoExplosiveBelt)
                {
                    GenExplosion.DoExplosion(__instance.Position, __instance.Map, 1.9f, RE_DefOf.RE_BombNoShake, __instance.Launcher, 5, -1f, null, (__instance.Launcher as Pawn)?.equipment?.Primary?.def);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
    internal static class TakeDamage_Patch
    {
        private static bool recursionTrap = false;
        private static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (!recursionTrap)
            {
                if (dinfo.Instigator != null && (dinfo.Weapon?.IsRangedWeapon ?? false) 
                    && AmmoBelt.pawnsWithAmmobelts.TryGetValue(dinfo.Instigator, out AmmoBelt ammoBelt) && !ammoBelt.DestroyedOrNull())
                {
                    if (ammoBelt.def == RE_DefOf.RE_AmmoPiercingBelt)
                    {
                        AccessTools.Field(typeof(DamageInfo), "armorPenetrationInt").SetValueDirect(__makeref(dinfo), dinfo.ArmorPenetrationInt * 1.5f);
                    }
                    else if (ammoBelt.def == RE_DefOf.RE_AmmoCryoBelt && __instance is Pawn victim)
                    {
                        HealthUtility.AdjustSeverity(victim, RE_DefOf.Hypothermia, 0.05f);
                    }
                    else if (ammoBelt.def == RE_DefOf.RE_AmmoToxicBelt && __instance is Pawn victim2)
                    {
                        HealthUtility.AdjustSeverity(victim2, HediffDefOf.ToxicBuildup, 0.05f);
                    }
                    else if (ammoBelt.def == RE_DefOf.RE_AmmoDisruptorBelt)
                    {
                        recursionTrap = true;
                        __instance.TakeDamage(new DamageInfo(DamageDefOf.EMP, 5f, 0, -1, dinfo.Instigator, null, dinfo.Weapon));
                    }
                    else if (ammoBelt.def == RE_DefOf.RE_AmmoIncendiaryBelt)
                    {
                        __instance.TryAttachFire(0.5f);
                    }
                }
            }
            else
            {
                recursionTrap = false;
            }
        }
    }
}
