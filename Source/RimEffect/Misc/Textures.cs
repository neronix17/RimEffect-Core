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
    public class ScenPart_StartRandomIncidentOfCategory : ScenPart
    {
        private IncidentCategoryDef incidentCategory;

        private IncidentDef incidentDef;


        public override void Randomize()
        {
            incidentDef = DefDatabase<IncidentDef>.AllDefs.Where(x => x.category == incidentCategory).RandomElement();
        }
        public override string Label => this.def.LabelCap;
        public override void PostMapGenerate(Map map)
        {
            if (Find.GameInitData != null)
            {
                if (incidentCategory is null)
                {
                    incidentCategory = IncidentCategoryDefOf.ThreatBig;
                }
                if (incidentDef is null)
                {
                    Randomize();
                }
                var parms = StorytellerUtility.DefaultParmsNow(incidentCategory, map);
                incidentDef.Worker.TryExecute(parms);
            }
        }
    }
}