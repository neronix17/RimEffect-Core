using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimEffect
{
	public class StatPart_ModifyMarketValue : StatPart
	{
		public override string ExplanationPart(StatRequest req)
		{
			if (req.HasThing)
			{

			}
			return null;
		}

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (req.HasThing && req.Thing.ParentHolder is Pawn_InventoryTracker inventoryTracker 
				&& inventoryTracker.pawn.GetLord()?.LordJob is LordJob_TradeWithColony lordJob 
				&& lordJob.lord.faction?.def == RE_DefOf.RE_SystemsAlliance && lordJob.lord.faction.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Ally)
			{
				val *= 0.75f;
			}
		}
	}
}
 