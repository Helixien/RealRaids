using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RealRaids
{
    public class JobDriver_CarrayPawn : JobDriver
    {
        public Pawn Comrade => (Pawn)TargetThingA;

        public IntVec3 Position => TargetB.Cell;

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => Comrade.Dead);
            this.FailOnNotDowned(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.A);

            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.GotoCell(Position, PathEndMode.OnCell);
            yield return Toils_Haul.PlaceCarriedThingInCellFacing(TargetIndex.B);
            yield return Toils_General.Do(() =>
            {
                var factionStore = pawn.factionInt.GetDataStore();
                factionStore.alreadyRescuedPawns.Add(new PawnTimeSignature(Comrade));
                pawn.GetLord().Notify_ReachedDutyLocation(pawn);
            });
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
        }
    }
}
