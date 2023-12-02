using System.Text.RegularExpressions;
using OpenRGB.NET.Models;

public static partial class StringExtensions {

    public static bool TryParseFromHexString(this string s, out Color? color) {
        color = null;
        if (!ColourHexString().IsMatch(s)) {
            return false;
        }

        var str = s.ToLower().TrimStart('#');
        if (str.Length == 6) {
            color = new Color(Convert.ToByte(str.Substring(0, 2), 16),
            Convert.ToByte(str.Substring(2, 2), 16),
            Convert.ToByte(str.Substring(4, 2), 16)
            );
            return true;
        }
        
        if (str.Length == 3) {
        color = new Color(
            Convert.ToByte(Convert.ToByte(str.Substring(0, 1), 16) << 4),
            Convert.ToByte(Convert.ToByte(str.Substring(1, 1), 16) << 4), 
            Convert.ToByte(Convert.ToByte(str.Substring(2, 1), 16) << 4));
            return true;
        }

        return false;
    }

    public static int LevensteinDistance(this string s, string t) {
        // https://stackoverflow.com/questions/13793560/find-closest-match-to-input-string-in-a-list-of-strings

        var n = s.Length;
        var m = t.Length;
        var d = new int[n + 1, m + 1];

        // Verify arguments.
        if (n == 0) {
            return m;
        }

        if (m == 0) {
            return n;
        }

        // Initialize arrays.
        for (var i = 0; i <= n; d[i, 0] = i++) { }

        for (var j = 0; j <= m; d[0, j] = j++) { }

        // Begin looping.
        for (var i = 1; i <= n; i++) {
            for (var j = 1; j <= m; j++) {
                // Compute cost.
                var cost = t[j - 1] == s[i - 1] ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                d[i - 1, j - 1] + cost);
            }
        }
        // Return cost.
        return d[n, m];
    }

    [GeneratedRegex("#?(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})")]
    private static partial Regex ColourHexString();
}