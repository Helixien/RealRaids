using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RealRaids.Patches
{
    [HarmonyPatch(typeof(LordJob_AssaultColony), "CreateGraph")]
    public static class LordJob_AssaultColony_CreateGraph_Patch
    {
        public static bool Prefix(LordJob_AssaultColony __instance, ref StateGraph __result)
        {
            StateGraph stateGraph = new StateGraph();
            LordToil lordToil_assaultColony = new RealRaids.LordToil_AssaultColony();
            stateGraph.AddToil(lordToil_assaultColony);

            LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, canDig: false, interruptCurrentJob: true);
            lordToil_ExitMap.useAvoidGrid = true;
            stateGraph.AddToil(lordToil_ExitMap);

            if (__instance.assaulterFaction.def.humanlikeFaction)
            {
                if (__instance.canTimeoutOrFlee)
                {
                    Transition assaultToGiveUp = new Transition(lordToil_assaultColony, lordToil_ExitMap);
                    assaultToGiveUp.AddTrigger(new Trigger_TicksPassed(50000));
                    assaultToGiveUp.AddPreAction(new TransitionAction_Message("MessageRaidersGivenUpLeaving".Translate(__instance.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), __instance.assaulterFaction.Name)));
                    stateGraph.AddTransition(assaultToGiveUp);
                }
            }

            Transition assaultToExit = new Transition(lordToil_assaultColony, lordToil_ExitMap);
            assaultToExit.AddTrigger(new Trigger_BecameNonHostileToPlayer());
            assaultToExit.AddPreAction(new TransitionAction_Message("MessageRaidersLeaving".Translate(__instance.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), __instance.assaulterFaction.Name)));

            stateGraph.AddTransition(assaultToExit);
            __result = stateGraph;
            return false;
        }
    }
}
