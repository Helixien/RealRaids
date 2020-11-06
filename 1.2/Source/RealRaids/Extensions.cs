using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using RealRaids.Storage;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RealRaids
{
    public static class Extensions
    {
        private static Map[] maps = new Map[20];
        private static MapComponent_RealRaids[] mapComponent_RealRaids = new MapComponent_RealRaids[20];

        private static World world;
        private static WorldComponent_RealRaids worldComponent_RealRaids;

        /// <summary>
        /// Used to initialize the cache for a map.
        /// </summary>
        /// <param name="map"></param>
        private static void InitializeMapCache(Map map)
        {
            int index = map.Index;
            maps[index] = map;
            mapComponent_RealRaids[index] = map.GetComponent<MapComponent_RealRaids>();
        }

        /// <summary>
        /// Used to initialize the worldcomp cache.
        /// </summary>
        private static void InitializeWorldCache()
        {
            world = Find.World;
            worldComponent_RealRaids = world.GetComponent<WorldComponent_RealRaids>();
        }

        /// <summary>
        /// Return the provided map RealRaids MapComponent.
        /// </summary>
        /// <param name="map">Map</param>
        /// <returns>Current map MapComponent_RaiderMemory</returns>
        public static MapComponent_RealRaids GetRealRaidsComp(this Map map)
        {
            int index = map.Index;
            if (maps[index] != map)
            {
                InitializeMapCache(map);
            }
            return mapComponent_RealRaids[index];
        }


        /// <summary>
        /// Get the current WorldComponent_RealRaids.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public static WorldComponent_RealRaids GetRealRaidsWorldComp(this World world)
        {
            if (world != Extensions.world)
            {
                InitializeWorldCache();
            }
            return worldComponent_RealRaids;
        }

        /// <summary>
        /// Return the provided pawn data storage unit.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static PawnDataStore GetDataStore([NotNull] this Pawn pawn)
        {
            if (Find.World != world)
            {
                InitializeWorldCache();
            }
            return worldComponent_RealRaids.GetPawnStore(pawn);
        }

        /// <summary>
        /// Return the provided pawn data storage unit.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static FactionDataStore GetDataStore([NotNull] this Faction faction)
        {
            if (Find.World != world)
            {
                InitializeWorldCache();
            }
            return worldComponent_RealRaids.GetFactionStore(faction);
        }
    }
}
