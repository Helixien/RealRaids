using System;
using RimWorld;
using Verse;

namespace RealRaids.Storage
{
    public abstract class BaseDataStore<T> : IExposable, ILoadReferenceable where T : ILoadReferenceable
    {
        private T owner;
        private int hash;
        private string loadID;

        private const string loadIDPrefix = "DataStore_";

        public T Owner => owner;

        public BaseDataStore()
        {
        }

        public BaseDataStore(T owner)
        {
            this.owner = owner;
            this.hash = GetUniqueLoadID().GetHashCode();
        }

        public virtual void ExposeData()
        {
            #region SystemData
            // Scribe system data here
            Scribe_References.Look<T>(ref owner, "owner", saveDestroyedThings: false);
            Scribe_Values.Look<string>(ref loadID, loadIDPrefix + "loadID");
            // Post exposing data update the hash code
            this.hash = GetUniqueLoadID().GetHashCode();
            #endregion
        }

        public string GetUniqueLoadID()
        {
            if (loadID == null)
            {
                return loadID = loadIDPrefix + owner.GetUniqueLoadID();
            }
            return loadID;
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public override bool Equals(object obj)
        {
            return hash == obj.GetHashCode();
        }
    }
}
