using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.CommonAPI
{
    public static class Constants
    {
        public const string AUTH_SCHEME_GOOGLE = "GoogleAuthentication";
        public const string AUTH_SCHEMA_YARD_LIGHT = "YardLightAuthentication";
        public const string POLICY_CLIENT_EDIT = "CLIENT:EDIT";
        public const string POLICY_CLIENT_READ = "CLIENT:READ";
        public const string POLICY_TOKEN_CREATE = "TOKEN:CREATE";
        public const string POLICY_USER_EDIT = "USER:EDIT";
        public const string POLICY_USER_READ = "USER:READ";
        public const string POLICY_ROLE_EDIT = "ROLE:EDIT";
        public const string POLICY_LOG_READ = "LOG:READ";
        public const string POLICY_LOG_WRITE = "LOG:WRITE";
    }
}
