namespace RimEffect
{
    using System.Collections.Generic;
    using RimWorld;
    using RimWorld.Planet;
    using UnityEngine;
    using Verse;
    using VFECore.Abilities;
    using Ability = VFECore.Abilities.Ability;
    using AbilityDef = VFECore.Abilities.AbilityDef;

    public class Hediff_BioticAmp : Hediff_Abilities
    {
        public float bioticEnergy;

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            this.bioticEnergy = this.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyMax);
        }

        public void UseEnergy(float energyUsed)
        {
            this.bioticEnergy -= energyUsed;
            this.bioticEnergy =  Mathf.Min(this.bioticEnergy, this.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyMax));
        }

        public bool SufficientEnergyPresent(float energyWanted) => 
            this.bioticEnergy > energyWanted;

        public override void Tick()
        {
            base.Tick();
            this.bioticEnergy += this.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyRecoveryRate) / GenTicks.TicksPerRealSecond;
            this.bioticEnergy =  Mathf.Min(this.bioticEnergy, this.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyMax));
        }

        public override IEnumerable<Gizmo> DrawGizmos()
        {
            Gizmo_BioticEnergyStatus gizmoBioticEnergy = new Gizmo_BioticEnergyStatus {bioticHediff = this};
            yield return gizmoBioticEnergy;
        }

        public override bool SatisfiesConditionForAbility(AbilityDef abilityDef) => 
            base.SatisfiesConditionForAbility(abilityDef) || 
            (abilityDef.requiredHediff?.minimumLevel == 1 && this.level == 0 && this.pawn.health.hediffSet.HasHediff(RE_DefOf.RE_BioticNatural));

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.bioticEnergy, nameof(this.bioticEnergy));
        }
    }

    public class AbilityExtension_Biotic : AbilityExtension_AbilityMod
    {
        public float GetEnergyUsedByPawn(Pawn pawn) => this.energyUsed * pawn.GetStatValue(RE_DefOf.RE_BioticAbilityCostMultiplier);

        public float energyUsed = 0f;

        public override bool IsEnabledForPawn(Ability ability, out string reason)
        {
            reason = "RE.AbilityDisableReasonBioticEnergyLack".Translate();

            return ability.Hediff != null && ((Hediff_BioticAmp) ability.Hediff).SufficientEnergyPresent(this.GetEnergyUsedByPawn(ability.pawn));
        }

        public override void Cast(GlobalTargetInfo[] targets, Ability ability)
        {
            base.Cast(targets, ability);

            Hediff_BioticAmp bioticAmp = (Hediff_BioticAmp) ability.pawn.health.hediffSet.GetFirstHediffOfDef(RE_DefOf.RE_BioticAmpHediff);
            bioticAmp.UseEnergy(this.GetEnergyUsedByPawn(ability.pawn));
        }

        public override string GetDescription(Ability ability) => 
            $"{"RE.AbilityStatsBioticEnergy".Translate()}: {this.GetEnergyUsedByPawn(ability.pawn)}".Colorize(Color.cyan);
    }

    [StaticConstructorOnStartup]
    public class Gizmo_BioticEnergyStatus : Gizmo
    {
        public Hediff_BioticAmp bioticHediff;

        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public override float Order => -1500f;
        
        public override float GetWidth(float maxWidth) => 
            140f;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect  = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font    = GameFont.Tiny;
            Widgets.Label(rect3, this.bioticHediff.LabelCap);
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = this.bioticHediff.bioticEnergy / this.bioticHediff.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyMax);
            Widgets.FillableBar(rect4, fillPercent, FullBarTex, EmptyBarTex, doBorder: false);
            Text.Font   = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, (this.bioticHediff.bioticEnergy).ToString("F0") + " / " + this.bioticHediff.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyMax).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
