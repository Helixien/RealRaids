using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RealRaids.LordToils
{
    public class LordToil_Regroup : LordToil
    {
        public override bool AllowSatisfyLongNeeds => false;
        public override bool AllowSelfTend => false;

        public DutyDef Assault => DutyDefOf.AssaultColony;
        public DutyDef DefendComrade => DutyDefOf.Escort;
        public DutyDef RescueComrade => RealRaidsDefOf.RR_HelpInjuredComrades;

        public override void UpdateAllDuties()
        {
            HashSet<Pawn> rescued = new HashSet<Pawn>();
            HashSet<Pawn> rescuee = new HashSet<Pawn>();
            List<Pawn> downedPawns = lord.Map.mapPawns.PawnsInFaction(lord.faction)
                .Where(p => p.Downed).ToList();
            Pawn pawn;
            Pawn other;
            PawnDuty pawnDuty;
            if (downedPawns.Count == 0)
            {
                throw new NotImplementedException("REALRADIS: Regrouping with no pawns downed");
            }
            for (int i = 0; i < downedPawns.Count; i++)
            {
                other = downedPawns[i];
                if (lord.ownedPawns.Where(p => p.Position.DistanceTo(other.positionInt) < 30 && p.CanReach(other, PathEndMode.InteractionCell, Danger.Unspecified)).TryRandomElement(out pawn))
                {
                    if (pawn.Downed) continue;
                    if (rescuee.Contains(pawn) || rescued.Contains(other)) continue;
                    pawnDuty = new PawnDuty(RescueComrade, new LocalTargetInfo(other));
                    pawnDuty.locomotion = LocomotionUrgency.Sprint;
                    pawn.mindState.duty = pawnDuty;

                    Log.Message(pawn + " gets duty: " + RescueComrade);
                    if (pawn.jobs.curJob != null)
                    {
                        pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                }
            }
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                pawn = lord.ownedPawns[i];
                if (pawn.Downed) continue;
                if (rescuee.Contains(pawn)) continue;
                Pawn closestPawn = null;
                float minDistance = 1e5f;
                foreach (var rescuer in rescuee)
                {
                    float distance = pawn.positionInt.DistanceTo(rescuer.positionInt);
                    if (distance < minDistance && pawn.CanReach(rescuer, PathEndMode.InteractionCell, Danger.Unspecified))
                    {
                        minDistance = distance;
                        closestPawn = rescuer;
                    }
                }
                if (closestPawn != null)
                {
                    pawnDuty = new PawnDuty(DefendComrade, closestPawn, 15);
                    pawnDuty.locomotion = LocomotionUrgency.Jog;
                    pawn.mindState.duty = pawnDuty;

                    Log.Message(pawn + " gets duty: " + DefendComrade);
                    if (pawn.jobs.curJob != null)
                    {
                        pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                }
                else
                {
                    pawnDuty = new PawnDuty(Assault);
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
}
