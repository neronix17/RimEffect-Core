using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimEffect
{
	public class JobGiver_PyjakNuzzle : ThinkNode_JobGiver
	{
		private const float MaxNuzzleDistance = 40f;
		protected override Job TryGiveJob(Pawn pawn)
		{
			Log.Message(pawn + " - trying to nuzzle");
			if (pawn.RaceProps.nuzzleMtbHours <= 0f)
			{
				return null;
			}
			if (!(from p in pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
				  where !p.NonHumanlikeOrWildMan() && p != pawn && p.Position.InHorDistOf(pawn.Position, MaxNuzzleDistance) && pawn.GetRoom() == p.GetRoom() 
				  && !p.Position.IsForbidden(pawn) && p.CanCasuallyInteractNow()
				  select p).TryRandomElement(out Pawn result))
			{
				return null;
			}

			Job job = JobMaker.MakeJob(RE_DefOf.RE_PyjakNuzzle, result);
			job.locomotionUrgency = LocomotionUrgency.Walk;
			job.expiryInterval = 3000;
			Log.Message(pawn + " - get job " + job); ;
			return job;
		}
	}
}
