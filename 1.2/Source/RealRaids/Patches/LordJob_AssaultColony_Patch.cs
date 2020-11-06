using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RealRaids.Patches
{

    [HarmonyPatch(typeof(LordJob_AssaultColony), "CreateGraph")]
    public static class CreateGraph_Patch
    {
        public static bool Prefix(LordJob_AssaultColony __instance, ref StateGraph __result)
        {
			StateGraph stateGraph = new StateGraph();
			LordToil equipToil = new LordToil_EquipBestWeapons();
			stateGraph.AddToil(equipToil);

			LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, canDig: false, interruptCurrentJob: true);
			lordToil_ExitMap.useAvoidGrid = true;
			stateGraph.AddToil(lordToil_ExitMap);

			if (__instance.assaulterFaction.def.humanlikeFaction)
			{
				if (__instance.canTimeoutOrFlee)
				{
					Transition transition = new Transition(equipToil, lordToil_ExitMap);
					transition.AddTrigger(new Trigger_TicksPassed(500));
					transition.AddPreAction(new TransitionAction_Message("MessageRaidersGivenUpLeaving".Translate(__instance.assaulterFaction.def.pawnsPlural.CapitalizeFirst()
						, __instance.assaulterFaction.Name)));
					stateGraph.AddTransition(transition);
				}
			}
			Transition transition2 = new Transition(equipToil, lordToil_ExitMap);
			transition2.AddTrigger(new Trigger_BecameNonHostileToPlayer());
			transition2.AddPreAction(new TransitionAction_Message("MessageRaidersLeaving".Translate(__instance.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), __instance.assaulterFaction.Name)));
			stateGraph.AddTransition(transition2);

			__result = stateGraph;

            return false;
        }
    }
}
