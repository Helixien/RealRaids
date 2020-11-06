using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RealRaids
{
	public class JobGiver_EquipBestWeapon : ThinkNode_JobGiver
	{
		public float itemRadiusToSearch = 50;
		public override Job TryGiveJob(Pawn pawn)
		{
			Log.Message(pawn + " starting weapon search");
			if (TryFindBestWeapon(pawn, out Thing weapon))
			{
				Log.Message(pawn + " found a weapon: " + weapon);
				return JobMaker.MakeJob(JobDefOf.Equip, weapon);
			}
			Log.Message(pawn + " didn't found a weapon");
			return null;
		}

		private bool TryFindBestWeapon(Pawn pawn, out Thing weapon)
        {
			Predicate<Thing> validator = delegate (Thing t)
			{
				if (pawn != null && !pawn.CanReserve(t))
				{
					Log.Message(t + ": 1");
					return false;
				}
				if (t.IsBurning())
                {
					Log.Message(t + ": 2");
					return false;
                }
				if (t.def == pawn.equipment?.Primary?.def)
                {
					Log.Message(t + ": 3");
					return false;
                }
				return true;
			};
			Log.Message("itemRadiusToSearch: " + itemRadiusToSearch);
			weapon = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Weapon),
				PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), itemRadiusToSearch, validator);
			if (weapon != null)
            {
				Log.Message("Weapon is not null");
				return true;
            }
			Log.Message("Weapon is null");
			return false;
		}
	}
}
