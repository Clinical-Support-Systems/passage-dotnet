using System.Net.Mail;
using System.Text.RegularExpressions;

namespace PassageIdentity;

public static class StringUtils
{
    public static bool IsValidE164(this string phoneNumber)
    {
        var regex = new Regex(PassageConsts.E164RegexPattern);
        return regex.IsMatch(phoneNumber);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
    public static bool IsValidEmail(this string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "<Pending>")]
    public static string ToSnakeCase(this string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLowerInvariant();
    }
}
