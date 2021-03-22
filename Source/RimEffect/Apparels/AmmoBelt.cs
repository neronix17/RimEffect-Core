using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using VFECore;

namespace RimEffect
{
    public class AmmoBelt : Apparel
    {
        public static Dictionary<Thing, AmmoBelt> pawnsWithAmmobelts = new Dictionary<Thing, AmmoBelt>();
        private bool inUse;
        public bool InUse => inUse;
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
                if (this.Wearer != null)
                {
                    pawnsWithAmmobelts[this.Wearer] = this;
                }
                wearer = this.Wearer;
            }
        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            if (this.Wearer?.Faction?.IsPlayer ?? false)
            {
                Command_Toggle command_Toggle = new Command_Toggle();
                command_Toggle.hotKey = KeyBindingDefOf.Misc12;
                command_Toggle.isActive = (() => InUse);
                command_Toggle.toggleAction = delegate
                {
                    this.inUse = !InUse;
                };
                command_Toggle.defaultLabel = (inUse ? "RE.CommandDisableAmmoBeltLabel".Translate(this.Label) : "RE.CommandEnableAmmoBeltLabel".Translate(this.Label));
                command_Toggle.defaultDesc = "RE.CommandToggleAmmoBeltDesc".Translate();
                command_Toggle.icon = this.def.uiIcon;
                command_Toggle.turnOnSound = RE_DefOf.RE_Ammo_Enable;
                command_Toggle.turnOffSound = RE_DefOf.RE_Ammo_Enable;
                command_Toggle.groupKey = 817296546;
                yield return command_Toggle;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref inUse, "inUse");
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
            if (launcher != null && AmmoBelt.pawnsWithAmmobelts.TryGetValue(launcher, out AmmoBelt ammoBelt) && ammoBelt.InUse)
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

    [HarmonyPatch(typeof(ExpandableProjectile), "Impact")]
    internal static class ExpandableProjectile_Impact_Patch
    {
        private static void Prefix(ExpandableProjectile __instance, Thing hitThing)
        {
            if ((__instance.hitThings?.Any() ?? false) && (__instance.hitThings.Where(x => x is Pawn).Count() > 0 && hitThing is Pawn || __instance.hitThings.Where(x => x is Pawn).Count() > 1))
            {
                TakeDamage_Patch.ignoreEffect = true;
            }
        }
        private static void Postfix(ExpandableProjectile __instance, Thing hitThing)
        {
            if ((__instance.hitThings?.Any() ?? false) && __instance.hitThings.Where(x => x is Pawn).Count() <= 1
                && __instance.Launcher != null && AmmoBelt.pawnsWithAmmobelts.TryGetValue(__instance.Launcher, out AmmoBelt ammoBelt) && ammoBelt.InUse && hitThing is Pawn)
            {
                if (ammoBelt.def == RE_DefOf.RE_AmmoExplosiveBelt)
                {
                    GenExplosion.DoExplosion(__instance.Position, __instance.Map, 1.9f, RE_DefOf.RE_BombNoShake, __instance.Launcher, 5, -1f, null, (__instance.Launcher as Pawn)?.equipment?.Primary?.def);
                }
            }
            Log.Message(__instance + " - " + hitThing + " - " + TakeDamage_Patch.ignoreEffect);
            TakeDamage_Patch.ignoreEffect = false;
        }
    }

    [HarmonyPatch(typeof(Projectile), "Impact")]
    internal static class Impact_Patch
    {
        private static void Prefix(Projectile __instance, Thing hitThing)
        {
            if (__instance.Launcher != null && AmmoBelt.pawnsWithAmmobelts.TryGetValue(__instance.Launcher, out AmmoBelt ammoBelt) && ammoBelt.InUse && hitThing is Pawn)
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

        public static bool ignoreEffect;
        private static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (!recursionTrap && !ignoreEffect)
            {
                if (dinfo.Instigator != null && (dinfo.Weapon?.IsRangedWeapon ?? false)
                    && AmmoBelt.pawnsWithAmmobelts.TryGetValue(dinfo.Instigator, out AmmoBelt ammoBelt) && !ammoBelt.DestroyedOrNull() && ammoBelt.InUse && __instance is Pawn victim)
                {
                    if (ammoBelt.def == RE_DefOf.RE_AmmoPiercingBelt)
                    {
                        Log.Message("TakeDamage_Patch - Prefix - AccessTools.Field(typeof(DamageInfo), \"armorPenetrationInt\").SetValueDirect(__makeref(dinfo), dinfo.ArmorPenetrationInt * 1.5f); - 8", true);
                        AccessTools.Field(typeof(DamageInfo), "armorPenetrationInt").SetValueDirect(__makeref(dinfo), dinfo.ArmorPenetrationInt * 1.5f);
                    }
                    else if (ammoBelt.def == RE_DefOf.RE_AmmoCryoBelt)
                    {
                        Log.Message("TakeDamage_Patch - Prefix - HealthUtility.AdjustSeverity(victim, RE_DefOf.Hypothermia, 0.20f); - 10", true);
                        HealthUtility.AdjustSeverity(victim, RE_DefOf.Hypothermia, 0.20f);
                        Log.Message("TakeDamage_Patch - Prefix - HealthUtility.AdjustSeverity(victim, RE_DefOf.RE_HypothermicSlowdown, 0.20f); - 11", true);
                        HealthUtility.AdjustSeverity(victim, RE_DefOf.RE_HypothermicSlowdown, 0.20f);
                    }
                    else if (ammoBelt.def == RE_DefOf.RE_AmmoToxicBelt)
                    {
                        Log.Message("TakeDamage_Patch - Prefix - HealthUtility.AdjustSeverity(victim2, HediffDefOf.ToxicBuildup, 0.05f); - 13", true);
                        HealthUtility.AdjustSeverity(victim, HediffDefOf.ToxicBuildup, 0.05f);
                    }
                    else if (ammoBelt.def == RE_DefOf.RE_AmmoDisruptorBelt)
                    {
                        Log.Message("TakeDamage_Patch - Prefix - recursionTrap = true; - 15", true);
                        recursionTrap = true;
                        Log.Message("TakeDamage_Patch - Prefix - __instance.TakeDamage(new DamageInfo(DamageDefOf.EMP, 20f, 0, -1, dinfo.Instigator, null, dinfo.Weapon)); - 16", true);
                        __instance.TakeDamage(new DamageInfo(DamageDefOf.EMP, 20f, 0, -1, dinfo.Instigator, null, dinfo.Weapon));
                    }
                    else if (ammoBelt.def == RE_DefOf.RE_AmmoIncendiaryBelt)
                    {
                        Log.Message("TakeDamage_Patch - Prefix - __instance.TryAttachFire(0.5f); - 18", true);
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
