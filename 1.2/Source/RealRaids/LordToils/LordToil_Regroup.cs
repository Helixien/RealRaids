using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RealRaids.LordToils
{
    public class LordToil_Regroup : LordToil
    {
        public override bool AllowSatisfyLongNeeds => false;

        public override bool AllowSelfTend => false;

        public DutyDef AssaultColonyDuty => RealRaidsDefOf.RR_AssaultColony;
        public DutyDef RescueComrade => RealRaidsDefOf.RR_HelpInjuredComrades;

        public LordToil_Regroup()
        {

        }

        public override void UpdateAllDuties()
        {
            HashSet<Pawn> rescued = new HashSet<Pawn>();
            Pawn pawn;
            PawnDuty pawnDuty;
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                pawn = lord.ownedPawns[i];
                if (pawn.Map.mapPawns.PawnsInFaction(pawn.Faction).Where(p => p.Downed && !rescued.Contains(p) && p.Position.DistanceTo(pawn.positionInt) < 10).TryRandomElement(out Pawn other))
                {
                    pawnDuty = new PawnDuty(RescueComrade, other);
                    pawnDuty.locomotion = LocomotionUrgency.Sprint;
                    pawn.mindState.duty = pawnDuty;

                    rescued.Add(other);
                    Log.Message(pawn + " gets duty: " + RescueComrade);
                    if (pawn.jobs.curJob != null && !pawn.Downed)
                    {
                        pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                }
                else
                {
                    pawnDuty = new PawnDuty(AssaultColonyDuty);
                    pawnDuty.locomotion = LocomotionUrgency.Jog;
                    pawn.mindState.duty = pawnDuty;

                    Log.Message(pawn + " gets duty: " + AssaultColonyDuty);
                    if (pawn.jobs.curJob != null && !pawn.Downed)
                    {
                        pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                }
            }
        }
    }
}
