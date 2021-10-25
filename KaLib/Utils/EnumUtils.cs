using System;

namespace KaLib.Utils
{
    public static class EnumUtils
    {
        public static T Parse<T>(string value) where T : struct
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            => Enum.Parse<T>(value);
#else
            => (T)Enum.Parse(typeof(T), value);
#endif
    }
}