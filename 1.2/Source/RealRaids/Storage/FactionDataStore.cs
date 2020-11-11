using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RealRaids.Storage
{
    public class FactionDataStore : BaseDataStore<Faction>
    {
        public HashSet<PawnTimeSignature> alreadyRescuedPawns = new HashSet<PawnTimeSignature>();

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
            alreadyRescuedPawns = alreadyRescuedPawns.Where(p => p.IsValid).ToHashSet();
            Scribe_Collections.Look(ref alreadyRescuedPawns, "alreadyRescuedPawns", LookMode.Deep);
            #endregion
        }
    }
}
