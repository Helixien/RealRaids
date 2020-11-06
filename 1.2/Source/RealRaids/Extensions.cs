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

        private static Game game;
        private static GameComponent_RealRaids gameComponent_RealRaids;

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
        private static void InitializeGameCache()
        {
            game = Current.Game;
            gameComponent_RealRaids = game.GetComponent<GameComponent_RealRaids>();
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
        /// Get the current GameComponent_RealRaids.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public static GameComponent_RealRaids GetRealRaidsGameComp(this Game game)
        {
            if (game != Extensions.game)
            {
                InitializeGameCache();
            }
            return gameComponent_RealRaids;
        }

        /// <summary>
        /// Return the provided pawn data storage unit.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static PawnDataStore GetDataStore([NotNull] this Pawn pawn)
        {
            if (game != Current.Game)
            {
                InitializeGameCache();
            }
            return gameComponent_RealRaids.GetPawnStore(pawn);
        }

        /// <summary>
        /// Return the provided pawn data storage unit.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static FactionDataStore GetDataStore([NotNull] this Faction faction)
        {
            if (game != Current.Game)
            {
                InitializeGameCache();
            }
            return gameComponent_RealRaids.GetFactionStore(faction);
        }
    }
}
