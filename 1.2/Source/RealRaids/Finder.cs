using System;
using HarmonyLib;

namespace RealRaids
{
    public static class Finder
    {
        public const string modName = "Real Raids";

        public const string packageID = "Helixien.realraids";

        public static RealRaidsSettings settings;

        public static Harmony harmony = new Harmony(packageID);
    }
}
