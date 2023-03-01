using System.Text.RegularExpressions;

namespace PassageIdentity;

public static class StringUtils
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "<Pending>")]
    public static string ToSnakeCase(this string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLowerInvariant();
    }

    public static bool IsValidE164(this string phoneNumber)
    {
        var regex = new Regex(PassageConsts.E164RegexPattern);
        return regex.IsMatch(phoneNumber);
    }
}
