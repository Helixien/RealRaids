using System;
using Verse;

namespace RealRaids.Storage
{
    public class PawnDataStore : BaseDataStore<Pawn>
    {
        public PawnDataStore()
        {
        }

        public PawnDataStore(Pawn pawn) : base(pawn)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            #region UserData

            #endregion
        }
    }
}
