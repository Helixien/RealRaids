using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using HarmonyLib;
using RealRaids.LordToils;
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
            LordToil lordToil_assaultColony = new LordToil_AssaultColony();
            stateGraph.AddToil(lordToil_assaultColony);

            LordToil lordToil_regroup = new LordToil_Regroup();

            Transition assaultToRegroup = new Transition(lordToil_assaultColony, lordToil_regroup);
            assaultToRegroup.AddTrigger(new Trigger_PawnHarmed(0.2f));
            stateGraph.AddTransition(assaultToRegroup);

            Transition regroupToAssault = new Transition(lordToil_assaultColony, lordToil_regroup);
            regroupToAssault.AddTrigger(new Trigger_TicksPassed(1250));
            stateGraph.AddTransition(regroupToAssault);

            LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, canDig: false, interruptCurrentJob: true);
            lordToil_ExitMap.useAvoidGrid = true;
            stateGraph.AddToil(lordToil_ExitMap);


            if (__instance.assaulterFaction.def.humanlikeFaction)
            {
                if (__instance.canTimeoutOrFlee)
                {
                    Transition assaultToGiveUp = new Transition(lordToil_assaultColony, lordToil_ExitMap);
                    assaultToGiveUp.AddTrigger(new Trigger_TicksPassed(500));
                    assaultToGiveUp.AddPreAction(new TransitionAction_Message("MessageRaidersGivenUpLeaving".Translate(__instance.assaulterFaction.def.pawnsPlural.CapitalizeFirst()
                        , __instance.assaulterFaction.Name)));
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
