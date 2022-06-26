namespace RimEffect
{
    using RimWorld.Planet;
    using UnityEngine;
    using Verse;
    using Ability = VFECore.Abilities.Ability;

    public class Ability_Sabotage : Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);

            foreach (GlobalTargetInfo target in targets)
            {
                if (target.Thing is Pawn targetPawn)
                {
                    if (targetPawn.RaceProps.IsFlesh)
                        targetPawn.stances.StaggerFor(Mathf.RoundToInt(this.GetPowerForPawn() * GenTicks.TicksPerRealSecond));
                    else if (targetPawn.RaceProps.IsMechanoid)
                        targetPawn.mindState.mentalStateHandler.TryStartMentalState(RE_DefOf.RE_SabotageBerserk, null, forceWake: true);
                }
            }
        }

        public override int GetDurationForPawn() => RE_DefOf.RE_SabotageBerserk.minTicksBeforeRecovery + RE_DefOf.RE_SabotageBerserk.maxTicksBeforeRecovery - RE_DefOf.RE_SabotageBerserk.minTicksBeforeRecovery;
    }
}
