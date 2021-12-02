using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
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
			if (ShouldWorkOn(req))
            {
				return "RE.AllyRelationsDiscount".Translate();
			}
			return null;
		}
		public override void TransformValue(StatRequest req, ref float val)
		{
			if (ShouldWorkOn(req))
            {
				val *= 0.75f;
			}
		}

		private bool ShouldWorkOn(StatRequest req)
        {
			if (req.HasThing)
			{
				if (req.Thing.ParentHolder is Pawn_InventoryTracker inventoryTracker && inventoryTracker.pawn.GetLord()?.LordJob is LordJob_TradeWithColony lordJob
					&& FactionHasDiscount(lordJob.lord.faction))
				{
					return true;
				}
				else if (req.Thing.ParentHolder is Settlement_TraderTracker settlement_TraderTracker && FactionHasDiscount(settlement_TraderTracker.settlement.Faction))
				{
					return true;
				}
				else if (req.Thing.ParentHolder is Caravan_TraderTracker caravan_TraderTracker)
				{
					var caravan = Traverse.Create(caravan_TraderTracker).Field("caravan").GetValue<Caravan>();
					if (FactionHasDiscount(caravan.Faction))
					{
						return true;
					}
				}
			}
			return false;
		}
		private bool FactionHasDiscount(Faction faction)
        {
			return faction?.def == RE_DefOf.RE_SystemsAlliance && faction.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Ally;
		}
	}
}
 