using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using YardLight.Authorization.Core.Framework;
using YardLight.CommonAPI;
using YardLight.CommonCore;
using YardLight.Interface.Authorization.Models;
using LogAPI = BrassLoon.Interface.Log;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : AuthorizationControllerBase
    {
        private readonly IClientFactory _clientFactory;
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly IEmailAddressSaver _emailAddressSaver;

        public TokenController(IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            LogAPI.IMetricService metricService,
            LogAPI.IExceptionService exceptionService,
            IClientFactory clientFactory,
            IUserFactory userFactory,
            IUserSaver userSaver,
            IEmailAddressFactory emailAddressFactory,
            IEmailAddressSaver emailAddressSaver)
            : base(settings, settingsFactory, metricService, exceptionService)
        {
            _clientFactory = clientFactory;
            _userFactory = userFactory;
            _userSaver = userSaver;
            _emailAddressFactory = emailAddressFactory;
            _emailAddressSaver = emailAddressSaver;
        }

        [HttpPost]
        [Authorize(Constants.POLICY_TOKEN_CREATE)]
        public async Task<IActionResult> Create()
        {
            try
            {
                IUser user = await GetUser();
                return Ok(await CreateToken(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        
        [HttpPost("ClientCredential")]
        public async Task<IActionResult> CreateClientCredential([FromBody] ClientCredential clientCredential)
        {
            IActionResult result = null;
            try
            {
                if (result == null && clientCredential == null)
                    result = BadRequest("Missing request data");
                if (result == null && (!clientCredential.ClientId.HasValue || clientCredential.ClientId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing client id value");
                if (result == null && string.IsNullOrEmpty(clientCredential.Secret))
                    result = BadRequest("Missing secret value");
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient client = await _clientFactory.Get(settings, clientCredential.ClientId.Value);
                    if (client == null)
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                    {
                        if (await client.VerifySecret(settings, clientCredential.Secret) == false)
                            result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        result = Content(await CreateToken(client), "text/plain");
                    }
                }
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }
        
        [NonAction]
        private async Task<IUser> GetUser()
        {
            IUser user;
            IEmailAddress emailAddress = null;
            string subscriber = GetCurrentUserReferenceId();
            ISettings coreSettings = _settingsFactory.CreateCore(_settings.Value);
            user = await _userFactory.GetByReferenceId(coreSettings, subscriber);
            if (user == null)
            {
                string email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
                emailAddress = await _emailAddressFactory.GetByAddress(coreSettings, email);
                if (emailAddress == null)
                {
                    emailAddress = _emailAddressFactory.Create(email);
                    await _emailAddressSaver.Create(coreSettings, emailAddress);
                }
                user = _userFactory.Create(subscriber, emailAddress);
                user.Name = GetUserNameClaim().Value;
                await _userSaver.Create(coreSettings, user);
            }
            else
            {
                user.Name = GetUserNameClaim().Value;
                await _userSaver.Update(coreSettings, user);
            }
            return user;
        }

        [NonAction]
        private Claim GetUserNameClaim() => User.Claims.First(c => string.Equals(c.Type, "name", StringComparison.OrdinalIgnoreCase) || string.Equals(c.Type, ClaimTypes.Name, StringComparison.OrdinalIgnoreCase));

        [NonAction]
        private bool IsSuperUser(string email)
        {
            return (!string.IsNullOrEmpty(_settings.Value.SuperUser) 
                && !string.IsNullOrEmpty(email) 
                && string.Equals(email, _settings.Value.SuperUser, StringComparison.OrdinalIgnoreCase));
        }

        [NonAction]
        private async Task<List<string>> GetUserRoles(ISettings settings, IUser user)
        {
            List<string> roles = (await user.GetRoles(settings)).Keys.ToList();
            if (IsSuperUser((await user.GetEmailAddress(settings)).Address))
            {
                List<string> superUserRoles = new List<string>
                {
                    Constants.POLICY_LOG_WRITE,
                    Constants.POLICY_ROLE_EDIT,
                    Constants.POLICY_USER_EDIT,
                    Constants.POLICY_USER_READ
                };
                roles = roles.Union(superUserRoles).ToList();
            }
            return roles;
        }

        [NonAction]
        private async Task<string> CreateToken(IUser user)
        {
            ISettings settings = _settingsFactory.CreateCore(_settings.Value);
            RsaSecurityKey securityKey = RsaSecurityKeySerializer.GetSecurityKey(_settings.Value.TknCsp, true);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512);
            List<Claim> claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
                };
            Claim claim = User.Claims.FirstOrDefault(c => string.Equals(_settings.Value.ExternalIdIssuer, c.Issuer, StringComparison.OrdinalIgnoreCase) && string.Equals(ClaimTypes.NameIdentifier, c.Type, StringComparison.OrdinalIgnoreCase));
            if (claim != null)
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, claim.Value));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, (await user.GetEmailAddress(_settingsFactory.CreateCore(_settings.Value))).Address));
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.Name));
            await AddRoleClaims(settings, claims, user);
            JwtSecurityToken token = new JwtSecurityToken(
                _settings.Value.InternalIdIssuer,
                _settings.Value.InternalIdIssuer,
                claims,
                expires: DateTime.Now.AddHours(6),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [NonAction]
        private async Task AddRoleClaims(ISettings settings, List<Claim> claims, IUser user)
        {
            foreach (string role in await GetUserRoles(settings, user))
            {
                claims.Add(new Claim("role", role));
            }
        }

        [NonAction]
        private Task<string> CreateToken(IClient client)
        {
            RsaSecurityKey securityKey = RsaSecurityKeySerializer.GetSecurityKey(_settings.Value.TknCsp, true);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512);
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            };
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString("N")));
            JwtSecurityToken token = new JwtSecurityToken(
                _settings.Value.InternalIdIssuer,
                _settings.Value.InternalIdIssuer,
                claims,
                expires: DateTime.Now.AddHours(6),
                signingCredentials: credentials
                );
            return Task.FromResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
