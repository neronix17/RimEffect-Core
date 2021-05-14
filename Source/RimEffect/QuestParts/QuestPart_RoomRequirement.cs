using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimEffect
{
	public class RoomRequirement : IExposable
    {
		public RoomRoleDef roomRoleDef;
		public int currentCellCount;
		public int minimumCellRequirement;

		public RoomRequirement()
        {

        }

        public void ExposeData()
        {
			Scribe_Defs.Look(ref roomRoleDef, "roomRoleDef");
			Scribe_Values.Look(ref currentCellCount, "currentCellCount");
			Scribe_Values.Look(ref minimumCellRequirement, "minimumCellRequirement");
		}
    }
    public class QuestPart_RoomRequirement : QuestPart
	{
		public List<RoomRequirement> oldRooms = new List<RoomRequirement>();
		public List<RoomRequirement> roomsToBeBuilt = new List<RoomRequirement>();

		public Map map;
		public string inSignal;
		public override void Notify_QuestSignalReceived(Signal signal)
		{
			base.Notify_QuestSignalReceived(signal);
			if (!(signal.tag == inSignal) || this.quest.State != QuestState.Ongoing)
			{
				return;
			}

			var allRooms = map.regionGrid.allRooms.ListFullCopy();
			foreach (var room in oldRooms)
            {
				var first = allRooms.Where(x => x.Role == room.roomRoleDef).FirstOrDefault();
				if (first != null && first.CellCount >= room.minimumCellRequirement)
                {
					allRooms.Remove(first);
                }
            }
			string progressReport = "RE.CurrentProgress".Translate().RawText;
			int match = this.quest.description.RawText.IndexOf(progressReport);

			if (match >= 0)
				this.quest.description = this.quest.description.RawText.Substring(0, match);

			Dictionary<RoomRoleDef, int> roomCount = new Dictionary<RoomRoleDef, int>();
			foreach (var room in roomsToBeBuilt)
			{
				var rooms = allRooms.Where(x => x.Role == room.roomRoleDef && x.CellCount >= room.currentCellCount);
				if (rooms.Any())
                {
					roomCount[room.roomRoleDef] = rooms.Count();
				}
			}

			if (roomCount.Any())
            {
				progressReport += "\n";
				foreach (var roomProgress in roomCount)
                {
					progressReport += roomProgress.Value + "x " + roomProgress.Key.LabelCap + "\n";
				}
				this.quest.description += progressReport;
			}

			bool allDone = true;
			foreach (var room in roomsToBeBuilt)
            {
				var first = allRooms.Where(x => x.Role == room.roomRoleDef && x.CellCount >= room.currentCellCount).FirstOrDefault();
				if (first == null)
				{
					allDone = false;
				}
				else
                {
					allRooms.Remove(first);
				}
			}

			if (allDone)
            {
				Find.SignalManager.SendSignal(new Signal("Quest" + quest.id + ".AllRoomsAreBuilt"));
            }
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref oldRooms, "oldRooms", LookMode.Deep);
			Scribe_Collections.Look(ref roomsToBeBuilt, "roomsToBeBuilt", LookMode.Deep);
			Scribe_References.Look(ref map, "map");
			Scribe_Values.Look(ref inSignal, "inSignal");
		}
	}
}
