using System;
using System.Collections.Generic;
using System.Text;

namespace PassageIdentity
{
    public static class PassageConsts
    {
        public const string NamedClient = "Passage";
        public const string E164RegexPattern = @"^\+(?:[0-9] ?){6,14}[0-9]$";
        public const string CookieName = "psg_auth_token";
    }
}
