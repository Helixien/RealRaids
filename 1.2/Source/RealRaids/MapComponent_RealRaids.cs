using System;
using System.Collections.Generic;
using System.Linq;
using RealRaids.Scientific.Clustering;
using RimWorld;
using Verse;

namespace RealRaids
{
    public class MapComponent_RealRaids : MapComponent
    {
        public List<ThingWithComps> equipement = new List<ThingWithComps>();

        public MapComponent_RealRaids(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (GenTicks.TicksGame % 750 == 0)
            {
                CleanUp();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            CleanUp();
            Scribe_Collections.Look(ref equipement, "equipement", LookMode.Reference);
        }

        public void Notify_EquipementDroped(ThingWithComps equipement)
        {
            this.equipement.Add(equipement);
        }

        private void CleanUp()
        {
            equipement = equipement.Where(e => !e.Destroyed
            && e.Spawned).ToList();
        }
    }
}
