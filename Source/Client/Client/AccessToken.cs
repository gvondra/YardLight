using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client
{
    public static class AccessToken
    {
        public static Dictionary<string, string> GoogleToken { get; set; }
        public static string Token { get; set; }
        public static string GetGoogleIdToken() => GoogleToken?["id_token"];
    }
}
