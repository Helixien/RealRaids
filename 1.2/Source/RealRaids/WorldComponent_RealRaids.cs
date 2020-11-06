using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using RealRaids.Storage;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RealRaids
{
    public class WorldComponent_RealRaids : WorldComponent
    {
        private List<PawnDataStore> pawnDataStorage = new List<PawnDataStore>();
        private List<FactionDataStore> factionDataStorage = new List<FactionDataStore>();

        private Dictionary<int, PawnDataStore> pawnCache = new Dictionary<int, PawnDataStore>();
        private Dictionary<Faction, FactionDataStore> factionCache = new Dictionary<Faction, FactionDataStore>();

        public WorldComponent_RealRaids(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            this.CollectionsCleanup();
            base.ExposeData();
            Scribe_Collections.Look(ref pawnDataStorage, "pawnDataStorage", LookMode.Deep);
            Scribe_Collections.Look(ref factionDataStorage, "factionDataStorage", LookMode.Deep);
            this.RebuildCache();
        }

        /// <summary>
        /// Used to access the pawn data store
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public PawnDataStore GetPawnStore([NotNull] Pawn pawn)
        {
            if (pawnCache.TryGetValue(pawn.thingIDNumber, out var store))
                return store;
            store = new PawnDataStore(pawn);
            pawnDataStorage.Add(store);
            pawnCache.Add(pawn.thingIDNumber, store);
            return store;
        }

        /// <summary>
        /// Used to access the faction data store
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        public FactionDataStore GetFactionStore([NotNull] Faction faction)
        {
            if (factionCache.TryGetValue(faction, out var store))
                return store;
            store = new FactionDataStore(faction);
            factionDataStorage.Add(store);
            factionCache.Add(faction, store);
            return store;
        }

        /// <summary>
        /// Used to clean up collections before scribing
        /// </summary>
        private void CollectionsCleanup()
        {
            pawnDataStorage = pawnDataStorage.Where(p => p.Owner != null
            && !p.Owner.Destroyed
            && !p.Owner.Dead).ToList();
            factionDataStorage = factionDataStorage.Where(f => f.Owner != null
            && !f.Owner.defeated).ToList();
        }

        /// <summary>
        /// Used to rebuild cache after changes to the pawn data storage.
        /// </summary>
        private void RebuildCache()
        {
            pawnCache.Clear();
            foreach (var store in pawnDataStorage)
                if (store.Owner != null && !store.Owner.Destroyed)
                {
                    pawnCache.Add(store.Owner.thingIDNumber, store);
                }
            factionCache.Clear();
            foreach (var store in factionDataStorage)
                if (store.Owner != null)
                {
                    factionCache.Add(store.Owner, store);
                }
        }
    }
}
