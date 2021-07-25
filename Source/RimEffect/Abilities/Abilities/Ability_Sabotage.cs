﻿namespace RimEffect
{
    using UnityEngine;
    using Verse;
    using Ability = VFECore.Abilities.Ability;

    public class Ability_Sabotage : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            if (target.Pawn != null)
            {
                if (target.Pawn.RaceProps.IsFlesh)
                    target.Pawn.stances.StaggerFor(Mathf.RoundToInt(this.GetPowerForPawn() * GenTicks.TicksPerRealSecond));
                else if (target.Pawn.RaceProps.IsMechanoid)
                    target.Pawn.mindState.mentalStateHandler.TryStartMentalState(RE_DefOf.RE_SabotageBerserk, null, forceWake: true);
            }
        }

        public override int GetDurationForPawn() => RE_DefOf.RE_SabotageBerserk.minTicksBeforeRecovery + RE_DefOf.RE_SabotageBerserk.maxTicksBeforeRecovery - RE_DefOf.RE_SabotageBerserk.minTicksBeforeRecovery;
    }
}
