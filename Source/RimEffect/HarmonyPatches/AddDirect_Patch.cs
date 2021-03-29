namespace RimEffect
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using Verse;

    [HarmonyPatch(typeof(HediffSet), nameof(HediffSet.AddDirect))]
    public static class AddDirect_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions    = instructions.ToList();
            MethodInfo            notMissingPartsInfo = AccessTools.Method(typeof(HediffSet), nameof(HediffSet.GetNotMissingParts));

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                CodeInstruction instruction = codeInstructions[i];
                yield return instruction;

                if (instruction.opcode == OpCodes.Brtrue_S && i>4 && codeInstructions[i-4].Calls(notMissingPartsInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Hediff), nameof(Hediff.def)));
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(RE_DefOf), nameof(RE_DefOf.RE_OmniToolHediff)));
                    yield return new CodeInstruction(OpCodes.Beq_S, instruction.operand);
                }
            }
        }
    }
}
