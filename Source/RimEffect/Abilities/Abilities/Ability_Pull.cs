namespace RimEffect
{
    using RimWorld;
    using RimWorld.Planet;
    using Verse;
    using VFECore.Abilities;
    using Ability = VFECore.Abilities.Ability;

    public class Ability_Pull : Ability
    {

        public override void Cast(params GlobalTargetInfo[] targets)
        {

            foreach (GlobalTargetInfo target in targets)
            {


                IntVec3 destination = this.pawn.Position + ((target.Cell - this.pawn.Position).ToVector3().normalized * 2).ToIntVec3();

                /*
                if (!parent.def.HasAreaOfEffect)
                {
                    parent.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(target.Thing, pawn.Map), target.Thing.Position, 60);
                }
    
                if (Props.destination == AbilityEffectDestination.Selected)
                {
                    parent.AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(target.Cell, pawn.Map), destination, 60);
                }
                */

                target.Thing.TryGetComp<CompCanBeDormant>()?.WakeUp();

                if (target.Thing is Pawn)
                {
                    AbilityPawnFlyer flyer = (AbilityPawnFlyer)PawnFlyer.MakeFlyer(VFE_DefOf_Abilities.VFEA_AbilityFlyer, target.Thing as Pawn, destination, null, null);
                    flyer.ability = this;
                    flyer.target  = destination.ToVector3();
                    GenSpawn.Spawn(flyer, target.Cell, this.pawn.Map);
                }
                else
                {
                    target.Thing.Position = destination;
                }
            }

            base.Cast(targets);
        }

        public override void CheckCastEffects(GlobalTargetInfo[] targetInfos, out bool cast, out bool target, out bool hediffApply)
        {
            base.CheckCastEffects(targetInfos, out cast, out target, out _);
            hediffApply = false;
        }
    }
}