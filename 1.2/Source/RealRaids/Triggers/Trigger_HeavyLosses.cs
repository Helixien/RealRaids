using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace RealRaids
{
    public class Trigger_HeavyLosses : Trigger
    {
        public readonly float downedThreshold;

        public readonly float killedThreshold;

        public int pawnsCount = -1;

        private int lastDowning;
        private int lastKilling;

        private float downingMomentum = 0;
        private float killingMomentum = 0;

        private HashSet<Pawn> downedPawns = new HashSet<Pawn>();

        public Trigger_HeavyLosses(float downingThreshold = 0.5f, float killingThreshold = 0.15f) : base()
        {
            this.downedThreshold = downingThreshold;
            this.killedThreshold = killingThreshold;
        }

        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            var shouldCheck = false;
            if (SignalIsPawnKilled(signal))
            {
                if (lastKilling != 0)
                {
                    UpdateMomentum(downed: false);
                }
                lastKilling = GenTicks.TicksGame;
                shouldCheck = true;
            }
            if (SignalIsPawnDowned(signal))
            {
                if (lastDowning != 0)
                {
                    UpdateMomentum(downed: true);
                }
                lastDowning = GenTicks.TicksGame;
                shouldCheck = true;
            }
            if (shouldCheck && (downingMomentum > downedThreshold || killingMomentum > killedThreshold))
            {
#if TRACE
                Log.Message(string.Format("REALRAIDS: checks for faction {0}", lord.faction));
#if DEBUG
                Log.Message("REALRAIDS: Trigger_PawnsKilledOrHarmed on");
#endif
#endif
                return true;
            }
            return false;
        }

        public override void SourceToilBecameActive(Transition transition, LordToil previousToil)
        {
            if (!transition.sources.Contains(previousToil))
            {
                downingMomentum = 0;
                killingMomentum = 0;
            }
        }

        private void UpdateMomentum(bool downed = false)
        {
            int deltaT;
            if (downed)
            {
                deltaT = GenTicks.TicksGame - lastDowning;
                downingMomentum = downingMomentum * 0.4f + 600 / deltaT * 0.6f;
#if DEBUG
                Log.Message(string.Format("REALRAIDS: downing: heavylosses {0}", downingMomentum));
#endif
            }
            else
            {
                deltaT = GenTicks.TicksGame - lastKilling;
                killingMomentum = killingMomentum * 0.4f + 600 / deltaT * 0.6f;
#if DEBUG
                Log.Message(string.Format("REALRAIDS: killing: heavylosses {0}", killingMomentum));
#endif
            }
        }

        private bool SignalIsPawnDowned(TriggerSignal signal)
        {
            if (signal.type == TriggerSignalType.PawnDamaged && !downedPawns.Contains(signal.Pawn))
            {
                if (signal.dinfo.Def.ExternalViolenceFor(signal.Pawn) && signal.Pawn.Downed)
                {
                    downedPawns.Add(signal.Pawn);
                    return true;
                }
            }
            return false;
        }

        private bool SignalIsPawnKilled(TriggerSignal signal)
        {
            if (signal.type == TriggerSignalType.PawnLost)
            {
                return true;
            }
            return false;
        }
    }
}