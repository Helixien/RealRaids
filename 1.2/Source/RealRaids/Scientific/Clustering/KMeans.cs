using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using Verse;

namespace RealRaids.Scientific.Clustering
{
    public class KMeans
    {
        public readonly int k = 8;

        public readonly float timeout = 0.005f;

        public readonly Vector3[] data;
        public readonly Vector3[] centroids;

        private bool running = false;

        private HashSet<Vector3> openset = new HashSet<Vector3>();
        private List<Vector3>[] classification;

        public KMeans(Vector3[] data, int k = 8, float timeout = -1)
        {
            this.data = data;
            this.k = k > 0 ? k : 8;
            this.k = data.Length >= this.k ? this.k : (int)data.Length / 4;
            this.timeout = timeout > 0 ? timeout : 0.005f;

            centroids = new Vector3[this.k];
            classification = new List<Vector3>[this.k];
            for (int i = 0; i < this.classification.Length; i++)
            {
                classification[i] = new List<Vector3>();
            }
        }

        public void DebugDraw(Map map)
        {
            foreach (var centroid in centroids)
            {
                map.debugDrawer.FlashCell(centroid.ToIntVec3());
            }
            foreach (var point in data)
            {
                var centroidIndex = Predict(point);
                map.debugDrawer.FlashLine(point.ToIntVec3(), centroids[centroidIndex].ToIntVec3(), duration: 1);
            }
        }

        public void Start()
        {
            this.SelectCentroids();
            this.running = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (running && stopwatch.ElapsedTicks * Stopwatch.Frequency < timeout && Iterate())
            {

            }
            stopwatch.Stop();
            this.running = false;
        }

        public int Predict(Vector3 point)
        {
            float min = 1e5f;
            float dist = -1;
            int centroidIndex = -1;
            for (int i = 0; i < this.k; i++)
            {
                dist = Vector3.Distance(point, centroids[i]);
                if (dist < min || (dist == min && Rand.Chance(0.5f)))
                {
                    centroidIndex = i;
                    min = dist;
                }
            }
            return centroidIndex;
        }

        public bool Iterate()
        {
            for (int i = 0; i < classification.Length; i++)
            {
                classification[i].Clear();
            }
            foreach (var x in data)
            {
                float min = 1e5f;
                float dist = -1;
                int centroidIndex = -1;
                for (int i = 0; i < this.k; i++)
                {
                    dist = Vector3.Distance(x, centroids[i]);
                    if (dist < min || (dist == min && Rand.Chance(0.5f)))
                    {
                        centroidIndex = i;
                        min = dist;
                    }
                }
                classification[centroidIndex].Add(x);
            }
            bool changed = false;
            for (int i = 0; i < this.k; i++)
            {
                var mean = Vector3.zero;
                foreach (var x in classification[i])
                {
                    mean += x;
                }
                mean = mean / (classification[i].Count > 0 ? classification[i].Count : 1);
                if (classification[i].Count > 0 && Vector3.Distance(centroids[i], mean) > 1f)
                {
                    changed = true;
                }
                centroids[i] = classification[i].Count > 0 ? mean : centroids[i];
            }
            return changed;
        }

        private void SelectCentroids()
        {
            openset.Clear();
            for (int i = 0; i < centroids.Length; i++)
            {
                var c = this.data.RandomElement();
                if (!openset.Contains(c))
                {
                    centroids[i] = c;
                    openset.Add(c);
                }
            }
        }
    }
}
