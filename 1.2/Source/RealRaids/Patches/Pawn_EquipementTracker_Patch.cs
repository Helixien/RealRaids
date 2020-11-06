using System;
using HarmonyLib;
using Verse;

namespace RealRaids.Patches
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.DropAllEquipment))]
    public class Pawn_EquipementTracker_Patch
    {
        public static void Prefix(Pawn_EquipmentTracker __instance)
        {
            var pawn = __instance.pawn;
            var map = __instance.pawn.Map;
            if ((pawn.health.State == PawnHealthState.Dead || pawn.health.State == PawnHealthState.Down) && pawn?.equipment?.PrimaryEq != null)
            {
                var comp = map.GetRealRaidsComp();
                comp.Notify_EquipementDroped(pawn.equipment.Primary);
            }
        }
    }
}
