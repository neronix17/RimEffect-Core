using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimEffect
{
    [StaticConstructorOnStartup]
    public static class Textures
    {
        public static Texture2D AmmoBeltToggle = ContentFinder<Texture2D>.Get("UI/AmmoBeltToggle");
    }
}
