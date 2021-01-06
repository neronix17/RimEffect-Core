using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimEffect
{
	public class InteractionWorker_PyjakNuzzle : InteractionWorker
	{
		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			AddNuzzledThought(initiator, recipient);
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
		}

		private void AddNuzzledThought(Pawn initiator, Pawn recipient)
		{
			Thought_Memory newThought = (Thought_Memory)ThoughtMaker.MakeThought(RE_DefOf.RE_Thought_PyjakNuzzle);
			if (recipient.needs.mood != null)
			{
				recipient.needs.mood.thoughts.memories.TryGainMemory(newThought);
			}
		}
	}
}
