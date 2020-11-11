using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RealRaids
{
    public static class Pawn_Patch
    {
        [HarmonyPatch(typeof(Pawn), nameof(Pawn.PostApplyDamage))]
        public static class Pawn_PostApplyDamage_Patch
        {
            private static readonly IntVec3[] offsets = new IntVec3[]
            {
                new IntVec3(1,0,0),
                new IntVec3(1,0,1),
                new IntVec3(1,0,-1),
                new IntVec3(0,0,0),
                new IntVec3(0,0,1),
                new IntVec3(0,0,-1),
                new IntVec3(-1,0,0),
                new IntVec3(-1,0,1),
                new IntVec3(-1,0,-1)
            };

            public static void Prefix(Pawn __instance, DamageInfo dinfo, float totalDamageDealt)
            {
                if (dinfo.Category != DamageInfo.SourceCategory.ThingOrUnknown)
                {
                    return;
                }
                if (!__instance.factionInt.HostileTo(Faction.OfPlayer))
                {
                    return;
                }
                var map = __instance.Map;
                var store = map.GetRealRaidsComp();
                for (int i = 0; i < 9; i++)
                {
                    var cell = offsets[i] + __instance.positionInt;
                    if (cell.InBounds(map))
                    {
                        store.tacticalGrid[map.cellIndices.CellToIndex(cell)] += (int)totalDamageDealt * 15;
                    }
                }
            }
        }
    }
}
