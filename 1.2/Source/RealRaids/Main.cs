using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib;
using Verse;

namespace RealRaids
{
    [StaticConstructorOnStartup]
    public class Main : ModBase
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class OnInitialization : Attribute
        {

        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class OnDefsLoaded : Attribute
        {

        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class OnMapLoaded : Attribute
        {

        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class OnMapDiscarded : Attribute
        {

        }

        IEnumerable<Action> onDefsLoaded = GetAction<OnDefsLoaded>();
        IEnumerable<Action> onMapLoaded = GetAction<OnMapLoaded>();
        IEnumerable<Action> onMapDiscarded = GetAction<OnMapDiscarded>();

        static Main()
        {
#if DEBUG
            Log.Message("RR: Starting up...");
#endif
            IEnumerable<Action> initializationAction = GetAction<OnInitialization>();
            foreach (var action in initializationAction)
            {
                action.Invoke();
            }

            Finder.harmony.PatchAll();
        }

        static IEnumerable<Action> GetAction<T>() where T : Attribute
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .SelectMany(m => m.GetMethods())
                .Where(m => m.HasAttribute<T>());
            foreach (var method in methods)
            {
                yield return () => { method.Invoke(null, null); };
            }
        }

        public override void DefsLoaded()
        {
            base.DefsLoaded();
            foreach (var action in onDefsLoaded)
            {
                action.Invoke();
            }
        }

        public override void MapLoaded(Map map)
        {
            base.MapLoaded(map);
            foreach (var action in onMapLoaded)
            {
                action.Invoke();
            }
        }

        public override void MapDiscarded(Map map)
        {
            base.MapDiscarded(map);
            foreach (var action in onMapDiscarded)
            {
                action.Invoke();
            }
        }
    }
}
