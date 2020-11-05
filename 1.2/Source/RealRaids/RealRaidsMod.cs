using System;
using UnityEngine;
using Verse;

namespace RealRaids
{
    public class RealRaidsMod : Mod
    {
        private RealRaidsSettings settings;

        public override string SettingsCategory()
        {
            return Finder.modName;
        }

        public RealRaidsMod(ModContentPack modContent) : base(modContent)
        {
            Finder.settings = settings = GetSettings<RealRaidsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();

            settings.Write();
        }
    }
}
