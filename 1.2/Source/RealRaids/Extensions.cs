using System;
using Verse;

namespace RealRaids
{
    public static class Extensions
    {
        private static Map[] maps = new Map[20];
        private static MapComponent_RealRaids[] mapComponent_RealRaids = new MapComponent_RealRaids[20];

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
    }
}
