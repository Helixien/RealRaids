using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RealRaids
{
    public class JobGiver_EquipBestWeapon : ThinkNode_JobGiver
    {
        public int jobFrequency = 1500;

        public float itemRadiusToSearch = 50;

        public override Job TryGiveJob(Pawn pawn)
        {
            var store = pawn.GetDataStore();
            if (GenTicks.TicksGame - store.lastTick_EquipBestWeapon < jobFrequency)
            {
                return null;
            }
            if (TryFindBestWeapon(pawn, out Thing weapon))
            {
                store.lastTick_EquipBestWeapon = GenTicks.TicksGame;
                return JobMaker.MakeJob(JobDefOf.Equip, weapon);
            }
            return null;
        }

        private bool TryFindBestWeapon(Pawn pawn, out Thing weapon)
        {
            float equipedValue = pawn.equipment?.Primary.GetStatValue(StatDefOf.MarketValue, true) ?? 0;
            Predicate<Thing> validator = delegate (Thing t)
           {
               if (pawn != null && !pawn.CanReserve(t))
               {
                   return false;
               }
               if (t.IsBurning())
               {
                   return false;
               }
               if (t.def == pawn.equipment?.Primary?.def)
               {
                   return false;
               }
               if (t.GetStatValue(StatDefOf.MarketValue, true) <= equipedValue * 1.1f)
               {
                   return false;
               }
               return true;
           };
            var comp = pawn.Map.GetRealRaidsComp();
            if (comp.equipement.Count > 0 &&
                comp.equipement.Where(e => e.Spawned
            && !e.Destroyed
            && e.Position.DistanceTo(pawn.Position) < itemRadiusToSearch
            && validator.Invoke(e)).TryRandomElement(out weapon))
            {
                return true;
            }
            weapon = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Weapon),
                PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), itemRadiusToSearch, validator);
            if (weapon != null)
            {
                return true;
            }
            return false;
        }
    }
}
