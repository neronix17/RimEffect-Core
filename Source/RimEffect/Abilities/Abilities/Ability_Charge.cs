namespace RimEffect
{
    using System.Linq;
    using RimWorld;
    using RimWorld.Planet;
    using Verse;
    using VFECore.Abilities;
    using Ability = VFECore.Abilities.Ability;

    public class Ability_Charge : Ability
    {

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);

            LongEventHandler.QueueLongEvent(() =>
                                            {
                                                GlobalTargetInfo target = targets.First();

                                                IntVec3          destination = target.Cell + ((this.pawn.Position - target.Cell).ToVector3().normalized * 2).ToIntVec3();
                                                Map              map     = this.pawn.Map;

                                                AbilityPawnFlyer flyer = (AbilityPawnFlyer) PawnFlyer.MakeFlyer(VFE_DefOf_Abilities.VFEA_AbilityFlyer_Charge, this.pawn, destination, null, null);
                                                flyer.ability = this;
                                                flyer.target  = destination.ToVector3();
                                                GenSpawn.Spawn(flyer, target.Cell, map);
                                                target.Thing.TakeDamage(new DamageInfo(DamageDefOf.Blunt, this.GetPowerForPawn(), float.MaxValue, instigator: this.pawn));

                                            }, "chargeAbility", false, null);
        }
    }
}