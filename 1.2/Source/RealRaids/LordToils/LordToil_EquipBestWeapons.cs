using System;
using HarmonyLib;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RealRaids
{
	public class LordToil_EquipBestWeapons : LordToil
	{
		public override bool AllowSatisfyLongNeeds => false;

		public override bool AllowSelfTend => false;

		public DutyDef EquipWeaponDuty => RealRaidsDefOf.RR_EquipBestWeapons;

		public LordToil_EquipBestWeapons()
		{

		}

		public override void UpdateAllDuties()
		{
			for (int i = 0; i < lord.ownedPawns.Count; i++)
			{
				PawnDuty pawnDuty = new PawnDuty(EquipWeaponDuty);
				pawnDuty.locomotion = LocomotionUrgency.Jog;
				Pawn pawn = lord.ownedPawns[i];
				pawn.mindState.duty = pawnDuty;
				Log.Message(pawn + " gets duty: " + EquipWeaponDuty);
				if (pawn.jobs.curJob != null)
				{
					pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
				}
			}
		}
	}
}
