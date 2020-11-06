using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RealRaids
{
    public class JobGiver_CarryPawnToSafety : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            Pawn other = pawn.mindState.duty.focus.Pawn;
            LocalTargetInfo target = pawn.mindState.duty.focus;
            if (other.Destroyed || other.Dead || !target.IsValid)
            {
                return null;
            }
            if (!pawn.CanReach(pawn.mindState.duty.focus, PathEndMode.ClosestTouch, Danger.Deadly) || !pawn.CanReserve(target))
            {
                return null;
            }
            IntVec3 safeCell = FindSafeCell(pawn);
            return JobMaker.MakeJob(RealRaidsDefOf.RR_CarryPawn, new LocalTargetInfo(other), new LocalTargetInfo(safeCell));
        }

        private IntVec3 FindSafeCell(Pawn pawn)
        {
            List<Thing> threats = new List<Thing>();
            List<IAttackTarget> potentialTargetsFor = pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
            for (int i = 0; i < potentialTargetsFor.Count; i++)
            {
                Thing thing = potentialTargetsFor[i].Thing;
                if (SelfDefenseUtility.ShouldFleeFrom(thing, pawn, checkDistance: false, checkLOS: false))
                {
                    threats.Add(thing);
                }
            }
            List<Thing> thingsToFlee = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.AlwaysFlee);
            for (int j = 0; j < thingsToFlee.Count; j++)
            {
                Thing thing = thingsToFlee[j];
                if (SelfDefenseUtility.ShouldFleeFrom(thing, pawn, checkDistance: false, checkLOS: false))
                {
                    threats.Add(thing);
                }
            }
            return CellFinderLoose.GetFleeDest(pawn, threats);
        }
    }
}
