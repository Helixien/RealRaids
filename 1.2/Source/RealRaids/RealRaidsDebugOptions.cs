using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RealRaids
{
    public static class RealRaidsDebugOptions
    {
        [DebugAction(category = Finder.modName, name = "Set LordJob", actionType = DebugActionType.ToolMapForPawns)]
        public static void SetLordJob(Pawn pawn)
        {
            var lordJobSelection = new List<DebugMenuOption>();
            var faction = pawn.factionInt;
            var pawns = Find.CurrentMap.mapPawns.PawnsInFaction(faction);
            foreach (var lordJobType in typeof(LordJob).AllSubclasses())
                lordJobSelection.Add(new DebugMenuOption(lordJobType.Name, DebugMenuOptionMode.Action, () =>
                {
                    LordMaker.MakeNewLord(faction, (LordJob)Activator.CreateInstance(lordJobType), Find.CurrentMap, pawns);
                }));
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(lordJobSelection));
        }
    }
}
