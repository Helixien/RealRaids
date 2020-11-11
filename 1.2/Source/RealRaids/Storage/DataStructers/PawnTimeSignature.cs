using System;
using Verse;

namespace RealRaids
{
    public class PawnTimeSignature : IExposable
    {
        public Pawn pawn;
        public int tick;

        public bool IsValid => pawn != null && tick < 5000 && pawn.Spawned && !pawn.Destroyed;

        public PawnTimeSignature(Pawn pawn)
        {
            this.pawn = pawn;
            this.tick = GenTicks.TicksGame;
        }

        public void ExposeData()
        {
            if (!pawn.Destroyed && pawn.Spawned)
            {
                Scribe_Values.Look(ref tick, "tick");
                Scribe_References.Look(ref pawn, "pawn", false);
            }
        }
    }
}
