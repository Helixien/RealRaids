using System;
using HarmonyLib;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RealRaids
{
    public class LordToil_AssaultColony : LordToil
    {
        public override bool AllowSatisfyLongNeeds => false;

        public override bool AllowSelfTend => false;

        public DutyDef AssaultColonyDuty => RealRaidsDefOf.RR_AssaultColony;

        public LordToil_AssaultColony()
        {

        }

        public override void UpdateAllDuties()
        {
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                PawnDuty pawnDuty = new PawnDuty(AssaultColonyDuty);
                Pawn pawn = lord.ownedPawns[i];

                pawnDuty.locomotion = LocomotionUrgency.Jog;
                pawn.mindState.duty = pawnDuty;

                if (pawn.jobs.curJob != null)
                {
                    pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                }
            }
        }
    }
}
