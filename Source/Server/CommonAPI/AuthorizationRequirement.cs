using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.CommonAPI
{
    public class AuthorizationRequirement : IAuthorizationRequirement
    {
        public AuthorizationRequirement(string policyName, string issuer)
        {
            this.PolicyName = policyName;
            this.Issuer = issuer;
            this.Roles = new string[] { };
        }

        public AuthorizationRequirement(string policyName, string issuer, params string[] roles)
        {
            this.PolicyName = policyName;
            this.Issuer = issuer;
            this.Roles = roles;
        }

        public string PolicyName { get; set; }
        public string Issuer { get; set; }
        public string[] Roles { get; set; }
    }
}
