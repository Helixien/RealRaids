using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RealRaids.Patches
{
    [HarmonyPatch]
    public static class LordJob_AssaultColony_Patch
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(LordJob_AssaultColony), nameof(LordJob_AssaultColony.CreateGraph));
        }

        public static bool Prefix(LordJob_AssaultColony __instance, ref StateGraph __result)
        {
            bool useAvoidGridSmart = __instance.useAvoidGridSmart;
            bool canTimeoutOrFlee = __instance.canTimeoutOrFlee;
            bool canKidnap = __instance.canKidnap;
            bool canSteal = __instance.canSteal;
            IntRange SapTimeBeforeGiveUp = LordJob_AssaultColony.SapTimeBeforeGiveUp;
            IntRange AssaultTimeBeforeGiveUp = LordJob_AssaultColony.AssaultTimeBeforeGiveUp;
            Faction assaulterFaction = __instance.assaulterFaction;
            StateGraph stateGraph = new StateGraph();
            LordToil assaultToil = new LordToil_AssaultColony();
            if (useAvoidGridSmart)
            {
                assaultToil.useAvoidGrid = true;
            }
            stateGraph.AddToil(assaultToil);
            LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, canDig: false, interruptCurrentJob: true);
            lordToil_ExitMap.useAvoidGrid = true;
            stateGraph.AddToil(lordToil_ExitMap);
            if (assaulterFaction.def.humanlikeFaction)
            {
                if (canTimeoutOrFlee)
                {
                    Transition assaultToExitTransition = new Transition(assaultToil, lordToil_ExitMap);
                    assaultToExitTransition.AddTrigger(new Trigger_TicksPassed(AssaultTimeBeforeGiveUp.RandomInRange));
                    assaultToExitTransition.AddPreAction(new TransitionAction_Message("MessageRaidersGivenUpLeaving".Translate(assaulterFaction.def.pawnsPlural.CapitalizeFirst(), assaulterFaction.Name)));
                    stateGraph.AddTransition(assaultToExitTransition);
                    Transition assaultToExitVictoryTransition = new Transition(assaultToil, lordToil_ExitMap);
                    float randomInRange = new FloatRange(0.25f, 0.35f).RandomInRange;
                    assaultToExitVictoryTransition.AddTrigger(new Trigger_FractionColonyDamageTaken(randomInRange, 900f));
                    assaultToExitVictoryTransition.AddPreAction(new TransitionAction_Message("MessageRaidersSatisfiedLeaving".Translate(assaulterFaction.def.pawnsPlural.CapitalizeFirst(), assaulterFaction.Name)));
                    stateGraph.AddTransition(assaultToExitVictoryTransition);
                }
                if (canKidnap)
                {
                    LordToil KidnapJob = stateGraph.AttachSubgraph(new LordJob_Kidnap().CreateGraph()).StartingToil;
                    Transition assaultToKidnapTransition = new Transition(assaultToil, KidnapJob);
                    assaultToKidnapTransition.AddPreAction(new TransitionAction_Message("MessageRaidersKidnapping".Translate(assaulterFaction.def.pawnsPlural.CapitalizeFirst(), assaulterFaction.Name)));
                    assaultToKidnapTransition.AddTrigger(new Trigger_KidnapVictimPresent());
                    stateGraph.AddTransition(assaultToKidnapTransition);
                }
                if (canSteal)
                {
                    LordToil stealJob = stateGraph.AttachSubgraph(new LordJob_Steal().CreateGraph()).StartingToil;
                    Transition assaultToStealTransition = new Transition(assaultToil, stealJob);
                    assaultToStealTransition.AddPreAction(new TransitionAction_Message("MessageRaidersStealing".Translate(assaulterFaction.def.pawnsPlural.CapitalizeFirst(), assaulterFaction.Name)));
                    assaultToStealTransition.AddTrigger(new Trigger_HighValueThingsAround());
                    stateGraph.AddTransition(assaultToStealTransition);
                }
            }
            Transition assaultToExit = new Transition(assaultToil, lordToil_ExitMap);
            assaultToExit.AddTrigger(new Trigger_BecameNonHostileToPlayer());
            assaultToExit.AddPreAction(new TransitionAction_Message("MessageRaidersLeaving".Translate(assaulterFaction.def.pawnsPlural.CapitalizeFirst(), assaulterFaction.Name)));
            stateGraph.AddTransition(assaultToExit);
            __result = stateGraph;
            return false;
        }
    }
}
