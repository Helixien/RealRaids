using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using HarmonyLib;
using HugsLib;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI;

namespace RealRaids
{
    [HarmonyPatch]
    public static class PathFinder_FindPath_Patch
    {
        static readonly MethodInfo mFindPath = AccessTools.Method("PathFinder:FindPath", parameters: new[] {
            typeof(IntVec3),
            typeof(LocalTargetInfo),
            typeof(TraverseParms),
            typeof(PathEndMode) });

        static readonly MethodInfo mGetCellCost = AccessTools.Method("PathFinder_FindPath_Patch:GetCellCost");
        static readonly MethodInfo mHostile = AccessTools.Method("PathFinder_FindPath_Patch:IsHostile");
        static readonly MethodInfo mProfile = AccessTools.Method("PathFinder:PfProfilerBeginSample");

        static readonly FieldInfo fMap = AccessTools.Field(typeof(PathFinder), "map");
        static MapComponent_RealRaids current;

        public static MethodBase TargetMethod()
        {
            return mFindPath;
        }

        public static bool IsHostile(TraverseParms param, Map map)
        {
            if (param.pawn.HostileTo(Faction.OfPlayer))
            {
                return true;
            }
            return false;
        }

        public static int GetCellCost(int index)
        {
            return current.tacticalGrid[index];
        }

        public static void Prefix(PathFinder __instance)
        {
            current = __instance.map.GetRealRaidsComp();
        }

        public static void Postfix(PathFinder __instance)
        {
            current = null;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var codes = instructions.ToList();
            var local1 = ilGen.DeclareLocal(typeof(bool));

            bool added1 = false;
            bool finished = false;
            for (int i = 0; i < codes.Count; i++)
            {
                if (!finished)
                {
                    if (!added1 && codes[i].opcode == OpCodes.Call && codes[i].OperandIs(mProfile))
                    {
                        added1 = true;
                        yield return codes[i];
                        yield return new CodeInstruction(OpCodes.Ldarg_3);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, fMap);
                        yield return new CodeInstruction(OpCodes.Call, mHostile);
                        yield return new CodeInstruction(OpCodes.Stloc_S, local1.LocalIndex);
                        continue;
                    }

                    if (codes[i].opcode == OpCodes.Mul &&
                        codes[i + 1].opcode == OpCodes.Add &&
                        codes[i + 2].opcode == OpCodes.Stloc_S)
                    {
                        finished = true;
                        yield return codes[i + 1];
                        yield return codes[i + 2];

                        var l1 = ilGen.DefineLabel();
                        var code1 = new CodeInstruction(OpCodes.Ldloc_S, local1.LocalIndex);
                        code1.labels.AddRange(codes[i + 3].labels);
                        yield return code1;
                        yield return new CodeInstruction(OpCodes.Brfalse_S, l1);

                        yield return new CodeInstruction(OpCodes.Ldloc_S, 38);
                        yield return new CodeInstruction(OpCodes.Call, mGetCellCost);
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 41);
                        yield return new CodeInstruction(OpCodes.Add);
                        yield return new CodeInstruction(OpCodes.Stloc_S, 41);

                        codes[i + 3].labels.Clear();
                        codes[i + 3].labels.Add(l1);
                        yield return codes[i + 3];
                        i = i + 3; continue;
                    }
                }
                yield return codes[i];
            }
        }
    }
}