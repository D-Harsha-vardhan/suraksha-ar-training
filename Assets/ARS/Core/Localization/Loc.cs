using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ARS.Core.Localization
{
    public static class Loc
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Tables = new();
        public static string Language { get; private set; } = "en";
        public static void SetLanguage(string language) => Language = string.IsNullOrWhiteSpace(language) ? "en" : language;
        public static void LoadJson(string language, string json)
        {
            var table = new Dictionary<string, string>();
            foreach (Match match in Regex.Matches(json ?? string.Empty, "\\\"(?<key>[^\\\"]+)\\\"\\s*:\\s*\\\"(?<value>(?:\\\\.|[^\\\"])*)\\\"")) table[match.Groups["key"].Value] = Regex.Unescape(match.Groups["value"].Value);
            Tables[language] = table;
        }
        public static string T(string key)
        {
            if (Tables.TryGetValue(Language, out var selected) && selected.TryGetValue(key, out var value)) return value;
            if (Tables.TryGetValue("en", out var fallback) && fallback.TryGetValue(key, out value)) return value;
            return $"[{key}]";
        }
    }
}
