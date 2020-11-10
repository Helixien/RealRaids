using System;
using System.Collections.Generic;
using System.Linq;
using RealRaids.Scientific.Clustering;
using RimWorld;
using UnityEngine;
using Verse;

namespace RealRaids
{
    public class MapComponent_RealRaids : MapComponent
    {
        public int[] tacticalGrid;

        public List<ThingWithComps> equipement = new List<ThingWithComps>();

        private int currentIndex;
        private readonly int numGridCell;

        public MapComponent_RealRaids(Map map) : base(map)
        {
            tacticalGrid = new int[map.cellIndices.NumGridCells];
            numGridCell = map.cellIndices.NumGridCells;
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (GenTicks.TicksGame % 750 == 0)
            {
                CleanUp();
            }
            if (GenTicks.TicksGame % 2 == 0)
            {
                GridDecay();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            CleanUp();

            #region taticalGrid

            List<int> grid = tacticalGrid.ToList();
            Scribe_Collections.Look(ref grid, "tacticalGrid", LookMode.Value);
            tacticalGrid = grid?.ToArray() ?? new int[map.cellIndices.NumGridCells];
            #endregion

            #region other
            Scribe_Values.Look(ref currentIndex, "currentIndex");
            Scribe_Collections.Look(ref equipement, "equipement", LookMode.Reference);
            #endregion
        }

        public void Notify_EquipementDroped(ThingWithComps equipement)
        {
            this.equipement.Add(equipement);
        }

        private void GridDecay()
        {
            for (int i = 0; i < 100; i++)
            {
                tacticalGrid[currentIndex] = Mathf.Clamp(tacticalGrid[currentIndex] - 1, 0, 2500);
                currentIndex = (currentIndex + 1) % numGridCell;
            }
        }

        private void CleanUp()
        {
            equipement = equipement.Where(e => !e.Destroyed
            && e.Spawned).ToList();
        }
    }
}
