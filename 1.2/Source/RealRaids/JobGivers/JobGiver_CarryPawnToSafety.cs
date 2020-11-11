using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RealRaids
{
    public class JobGiver_CarryPawnToSafety : ThinkNode_JobGiver
    {
        public float chance = 0.25f;
        public float jobFrequency = 1250;

        public override Job TryGiveJob(Pawn pawn)
        {
            var store = pawn.GetDataStore();
            if (GenTicks.TicksGame - store.lastTick_RescueComrade <= 1250)
            {
                return null;
            }
            store.lastTick_RescueComrade = GenTicks.TicksGame;
            if (Rand.Chance(chance) && GetCandidate(pawn, out var other))
            {
                LocalTargetInfo target = new LocalTargetInfo(other);
                IntVec3 safeCell = FindSafeCell(pawn);
                return JobMaker.MakeJob(RealRaidsDefOf.RR_CarryPawn, target, new LocalTargetInfo(safeCell));
            }
            return null;
        }

        private bool GetCandidate(Pawn pawn, out Pawn other)
        {
            var map = pawn.Map;
            var closest = (int)1e5;
            other = null;
            var factionStore = pawn.factionInt.GetDataStore();
            foreach (Pawn p in map.mapPawns.PawnsInFaction(pawn.factionInt))
            {
                if (!p.Downed || p.Dead || factionStore.alreadyRescuedPawns.Count(s => s.pawn == p) != 0) continue;
                var distance = (int)p.positionInt.DistanceTo(pawn.positionInt);
                if (distance < closest && distance < 35 && p.CanReach(pawn, PathEndMode.ClosestTouch, Danger.Unspecified) && pawn.CanReserve(p))
                {
                    closest = distance;
                    other = p;
                }
            }
            if (other == null)
            {
                return false;
            }
            return true;
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
