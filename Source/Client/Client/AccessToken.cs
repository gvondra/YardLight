﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client
{
    public static class AccessToken
    {
        private const string Issuer = "urn:yardlight";
        private static string _token;
        private static JwtSecurityToken _jwtSecurityToken;
        public static Dictionary<string, string> GoogleToken { get; set; }
        public static string Token 
        { 
            get => _token; 
            set
            {
                _token = value;
                if (!string.IsNullOrEmpty(value))
                    _jwtSecurityToken = new JwtSecurityToken(value);
            } 
        }

        public static string GetGoogleIdToken() => GoogleToken?["id_token"];

        public static bool UserHasUserAdminRoleAccess()
        {
            return _jwtSecurityToken != null 
                && _jwtSecurityToken.Claims.Any(
                    clm => string.Equals(clm.Issuer, Issuer, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(clm.Type, "role", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(clm.Value, "ROLE:EDIT", StringComparison.OrdinalIgnoreCase)
                    );
        }
    }
}
