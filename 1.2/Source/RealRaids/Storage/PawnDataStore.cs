using System;
using System.Collections.Generic;
using Verse;

namespace RealRaids.Storage
{
    public class PawnDataStore : BaseDataStore<Pawn>
    {
        public int lastTick_EquipBestWeapon = 0;
        public int lastTick_RescueComrade = 0;

        public HashSet<ThingDef> alreadyHadAsWeapon = new HashSet<ThingDef>();

        public PawnDataStore()
        {
        }

        public PawnDataStore(Pawn pawn) : base(pawn)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            #region UserData
            Scribe_Values.Look(ref lastTick_EquipBestWeapon, "lastTick_EquipBestWeapon");
            Scribe_Values.Look(ref lastTick_RescueComrade, "lastTick_RescueComrade");
            #endregion
        }
    }
}
