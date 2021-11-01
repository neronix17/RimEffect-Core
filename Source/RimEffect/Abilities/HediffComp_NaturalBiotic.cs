namespace RimEffect
{
    using RimWorld;
    using Verse;
    using VFECore.Abilities;

    public class HediffComp_NaturalBiotic : HediffComp
    {
        public override bool CompDisallowVisible() => true;

        public override void CompPostMake()
        {
            base.CompPostMake();

            if (!this.Pawn.health.hediffSet.HasHediff(RE_DefOf.RE_BioticAmpHediff) && HediffMaker.MakeHediff(RE_DefOf.RE_BioticAmpHediff, this.Pawn, this.Pawn.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Brain).FirstOrFallback()) is Hediff_Abilities implant)
            {
                implant.Severity            = 0f;
                implant.giveRandomAbilities = true;
                this.Pawn.health.AddHediff(implant);
                implant.SetLevelTo(0);
            }
        }
    }
}
