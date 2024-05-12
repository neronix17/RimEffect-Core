namespace RimEffect
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using RimWorld;
    using Verse;
    using Verse.AI;

    [HarmonyPatch]
    public static class TendPatient_Patch
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => 
            AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(JobDriver_TendPatient), "MakeNewToils"));

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (instruction.Is(OpCodes.Ldc_R4, 600))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), nameof(JobDriver.pawn)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TendPatient_Patch), nameof(TendMultiplier)));
                }
            }
        }

        public static float TendMultiplier(float mult, Pawn pawn)
        {
            ThingDef thingDef = pawn.CurJob.GetTarget(TargetIndex.B).Thing?.def;
            if (thingDef != null && thingDef == RE_DefOf.RE_MediGel)
                mult /= 2f;
            return mult;
        }
    }
}
