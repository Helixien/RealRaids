using System;
using System.Linq;
using RealRaids.Scientific.Clustering;
using RimWorld;
using Verse;

namespace RealRaids
{
    public class MapComponent_RealRaids : MapComponent
    {
        public MapComponent_RealRaids(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
        }

        public void Notify_EquipementDroped(ThingWithComps equipement)
        {

        }
    }
}
