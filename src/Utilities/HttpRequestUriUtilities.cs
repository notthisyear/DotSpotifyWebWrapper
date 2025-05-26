using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotSpotifyWebWrapper.Utilities
{
    internal static partial class HttpRequestUriUtilities
    {
        [GeneratedRegex("(?<=((\\?)|(\\&)))((?<key>\\w+)\\=(?<value>[a-zA-Z0-9-_]+))", RegexOptions.Compiled)]
        private static partial Regex GetParseQueryRegexUri();

        private static readonly Regex s_parseQueryUri = GetParseQueryRegexUri();

        public static string GetQueryStringOrEmpty(string key, string value, bool prefixQuestionMark = true)
            => string.IsNullOrEmpty(value) ? string.Empty : $"{(prefixQuestionMark ? "?" : "")}{key}={value}";

        public static string GetQueryString(Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return string.Empty;

            if (parameters.Count == 1)
                return GetQueryStringOrEmpty(parameters.First().Key, parameters.First().Value);

            return "?" +
                string.Join("&", parameters
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .Select(p => GetQueryStringOrEmpty(p.Key, p.Value, false)));
        }

        public static Dictionary<string, string> ParseQueryUri(string uri)
        {
            var result = new Dictionary<string, string>();
            if (!uri.Contains('?'))
                return result;

            var matches = s_parseQueryUri.Matches(uri);
            if (matches == default || matches.Count == 0)
                return result;

            foreach (var m in matches.Cast<Match>())
            {
                var key = string.Empty;
                var value = string.Empty;
                foreach (var g in m.Groups.Cast<Group>())
                {
                    if (g.Name == "key")
                        key = g.Value;
                    else if (g.Name == "value")
                        value = g.Value;
                }

                if (!string.IsNullOrEmpty(key) || !string.IsNullOrEmpty(value))
                    result.Add(key, value);
            }
            return result;
        }
    }
}
