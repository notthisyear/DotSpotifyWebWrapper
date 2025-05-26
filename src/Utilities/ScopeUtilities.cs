using System;
using System.Linq;
using DotSpotifyWebWrapper.Types;

namespace DotSpotifyWebWrapper.Utilities
{
    internal static class ScopeUtilities
    {
        public static string ScopeName(this AccessScopeType type)
        {
            var t = typeof(AccessScopeType);
            if (Attribute.GetCustomAttribute(t.GetMember(type.ToString()).First(), typeof(AccessScopeAttribute)) is AccessScopeAttribute attr)
                return attr.ScopeName;
            else
                return string.Empty;
        }
    }
}
