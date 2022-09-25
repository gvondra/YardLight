using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client
{
    public static class Settings
    {
        public static string GoogleAuthorizationEndpoint => ConfigurationManager.AppSettings["GoogleAuthorizationEndpoint"];
        public static string GoogleClientId => ConfigurationManager.AppSettings["GoogleClientId"];
        public static string GoogleClientSecret => ConfigurationManager.AppSettings["GoogleClientSecret"];
        public static string GoogleTokenEndpoint => ConfigurationManager.AppSettings["GoogleTokenEndpoint"];
    }
}
