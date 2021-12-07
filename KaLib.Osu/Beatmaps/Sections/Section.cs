using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using KaLib.Utils;

namespace KaLib.Osu.Beatmaps.Sections
{
    public abstract class Section
    {
        public string Name { get; }

        private static Dictionary<string, Type> _typeMap = new();

        static Section()
        {
            foreach (var t in typeof(Section).Assembly.GetTypes()
                         .Where(x => typeof(Section).IsAssignableFrom(x) && x != typeof(Section)))
            {
                RuntimeHelpers.RunClassConstructor(t.TypeHandle);
            }
        }

        internal static void RegisterSectionType(string name, Type type)
        {
            if (!typeof(Section).IsAssignableFrom(type))
            {
                throw new ArgumentException("The given type is not a subclass of Section.");
            }

            _typeMap.Add(name, type);
        }

        public static Section CreateByName(string name)
        {
            if (!_typeMap.TryGetValue(name, out Type t))
            {
                return null;
            }

            return (Section)t.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
        }

        protected Section(string name)
        {
            Name = name;
        }

        public abstract void ReadFrom(BeatmapReader reader);
    }
}