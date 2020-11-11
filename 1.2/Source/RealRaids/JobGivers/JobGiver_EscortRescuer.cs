using System;
using System.Runtime.InteropServices;
using RimWorld;
using Verse;
using Verse.AI;

namespace RealRaids
{
    public class JobGiver_EscortRescuer : JobGiver_AIDefendPawn
    {
        public float searchRadius = 50f;
        public float targetKeepRadius = 15f;

        private Pawn defendee = null;

        public override Job TryGiveJob(Pawn pawn)
        {
            defendee = GetCandidate(pawn);
            if (defendee == null)
            {
                return null;
            }
            return base.TryGiveJob(pawn);
        }

        public override float GetFlagRadius(Pawn pawn)
        {
            return targetKeepRadius;
        }

        public override Pawn GetDefendee(Pawn pawn)
        {
            return defendee;
        }

        private Pawn GetCandidate(Pawn pawn)
        {
            var map = pawn.Map;
            var closest = (int)1e5;

            Pawn other = null;
            foreach (Pawn p in map.mapPawns.PawnsInFaction(pawn.factionInt))
            {
                if (p?.jobs?.curDriver?.GetType() != typeof(JobDriver_CarrayPawn)) continue;
                if (!pawn.CanReach(p, PathEndMode.ClosestTouch, maxDanger: Danger.Unspecified)) continue;
                var distance = (int)p.positionInt.DistanceTo(pawn.positionInt);
                if (distance <= searchRadius && distance < closest)
                {
                    closest = distance;
                    other = p;
                }
            }
            return other;
        }
    }
}
