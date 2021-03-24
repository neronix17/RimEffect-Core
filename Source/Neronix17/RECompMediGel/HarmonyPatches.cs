using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

using HarmonyLib;

namespace RECompMediGel
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("com.neronix17.rimeffect.medigel");

            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(TendUtility), "DoTend")]
    static class TendUtility_DoTend
    {
        [HarmonyPrefix]
        static bool Prefix(Pawn doctor, Pawn patient, Medicine medicine)
        {
            if (medicine != null && medicine.def.defName == "RE_MediGel")
            {
                MediGelUtility.TrySealWounds(patient);
                MediGelUtility.TendAdditional(doctor, patient);
                if (medicine != null)
                {
                    if (medicine.stackCount > 1)
                    {
                        medicine.stackCount--;
                    }
                    if (!medicine.Destroyed)
                    {
                        medicine.Destroy(DestroyMode.Vanish);
                    }
                }
                return false;
            }
            return true;
        }
    }
}
