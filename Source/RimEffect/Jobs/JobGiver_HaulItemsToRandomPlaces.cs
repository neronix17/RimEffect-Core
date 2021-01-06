using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimEffect
{
	public class JobGiver_HaulItemsToRandomPlaces : ThinkNode_JobGiver
	{
		private const float MinDistance = 40f;
		private const float MaxDistance = 80f;
		protected override Job TryGiveJob(Pawn pawn)
		{
			Log.Message(pawn + " is trying to steal items");
			if (!(from t in pawn.Map.listerThings.AllThings.Where(x => x.def.EverHaulable && x.Map.areaManager.Home[x.Position] && x.IsInAnyStorage() 
				  && pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.Deadly))
				  select t).TryRandomElement(out Thing result))
			{
				return null;
			}
			Log.Message(pawn + " item is found: " + result);
			var cells = pawn.Map.AllCells.Where(x => x.DistanceTo(result.Position) > MinDistance && x.DistanceTo(result.Position) < MaxDistance && x.Walkable(pawn.Map) && !x.IsValidStorageFor(pawn.Map, result) 
			&& pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.Deadly));
			if (cells.Any())
			{
				Job job = JobMaker.MakeJob(JobDefOf.HaulToCell, result, cells.RandomElement());
				job.locomotionUrgency = LocomotionUrgency.Walk;
				job.count = Mathf.Min(result.stackCount, (int)(pawn.GetStatValue(StatDefOf.CarryingCapacity) / result.def.VolumePerUnit));
				Log.Message(pawn + " - get job " + job); ;
				return job;
			}
			return null;
		}
	}
}
