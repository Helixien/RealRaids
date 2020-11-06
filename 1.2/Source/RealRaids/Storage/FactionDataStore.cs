using System;
using RimWorld;

namespace RealRaids.Storage
{
    public class FactionDataStore : BaseDataStore<Faction>
    {
        public FactionDataStore()
        {
        }

        public FactionDataStore(Faction owner) : base(owner)
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
