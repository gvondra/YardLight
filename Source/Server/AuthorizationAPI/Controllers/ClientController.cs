using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ClientController : AuthorizationControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IClientFactory _clientFactory;
        private readonly IClientSaver _clientSaver;
        private readonly IClientSecretProcessor _clientSecretProcessor;

        public ClientController(IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            LogAPI.IMetricService metricService,
            LogAPI.IExceptionService exceptionService,
            IMapper mapper,
            IClientFactory clientFactory,
            IClientSaver clientSaver,
            IClientSecretProcessor clientSecretProcessor)
            : base(settings, settingsFactory, metricService, exceptionService)
        {
            _mapper = mapper;
            _clientFactory = clientFactory;
            _clientSaver = clientSaver;
            _clientSecretProcessor = clientSecretProcessor;
        }

        [NonAction]
        private async Task<Client> MapClient(ISettings settings, IMapper mapper, IClient innerClient)
        {
            Client client = _mapper.Map<Client>(innerClient);
            client.Roles = await innerClient.GetRoles(settings);
            return client;
        }

        [HttpGet()]
        [Authorize(Constants.POLICY_CLIENT_READ)]
        [ProducesResponseType(typeof(Client), 200)]
        public async Task<IActionResult> GetAll()
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                IEnumerable<IClient> innerClients = await _clientFactory.GetAll(settings);
                result = Ok(await Task.WhenAll(innerClients.Select<IClient, Task<Client>>(async c => await MapClient(settings, _mapper, c))));
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _ = WriteMetrics("get-client-all", DateTime.UtcNow.Subtract(start).TotalSeconds);
            }
            return result;
        }

        [HttpGet("{id}")]
        [Authorize(Constants.POLICY_CLIENT_READ)]
        [ProducesResponseType(typeof(Client), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient innerClient = await _clientFactory.Get(settings, id.Value);
                    if (innerClient == null)
                        result = NotFound();
                    if (result == null)
                    {
                        result = Ok(await MapClient(settings, _mapper, innerClient));
                    }
                }
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _ = WriteMetrics("get-client", DateTime.UtcNow.Subtract(start).TotalSeconds, new Dictionary<string, string> { { nameof(id), id.ToString() } });
            }
            return result;
        }

        [HttpGet("/api/ClientCredentialSecret")]
        [Authorize(Constants.POLICY_CLIENT_EDIT)]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetClientCredentialSecret()
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result;
            try
            {
                result = Ok(_clientSecretProcessor.Create());
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _ = WriteMetrics("get-client-credential-secret", DateTime.UtcNow.Subtract(start).TotalSeconds);
            }
            return result;
        }

        [NonAction]
        private async Task SetRoles(ISettings settings, Client client, IClient innerClient)
        {
            if (client.Roles != null)
            {
                foreach (string key in client.Roles.Keys)
                {
                    await innerClient.AddRole(settings, key);
                }
            }
            foreach (string key in (await innerClient.GetRoles(settings)).Keys)
            {
                if (client.Roles == null || !client.Roles.ContainsKey(key))
                    await innerClient.RemoveRole(settings, key);
            }
        }

        [HttpPost()]
        [Authorize(Constants.POLICY_CLIENT_EDIT)]
        [ProducesResponseType(typeof(Client), 200)]
        public async Task<IActionResult> Create(ClientSaveRequest request)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && request.Client == null)
                    result = BadRequest("Missing client data");
                if (result == null && string.IsNullOrEmpty(request.Client?.Name))
                    result = BadRequest("Missing client name value");
                if (result == null && string.IsNullOrEmpty(request.Secret))
                    result = BadRequest("Missing secret value");
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient innerClient = _clientFactory.Create(request.Secret);
                    _mapper.Map<Client, IClient>(request.Client, innerClient);
                    await SetRoles(settings, request.Client, innerClient);
                    await _clientSaver.Create(settings, innerClient);
                    result = Ok(await MapClient(settings, _mapper, innerClient));
                }
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _ = WriteMetrics("create-client", DateTime.UtcNow.Subtract(start).TotalSeconds);
            }
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Constants.POLICY_CLIENT_EDIT)]
        [ProducesResponseType(typeof(Client), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? id, ClientSaveRequest request)
        {
            DateTime start = DateTime.UtcNow;
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing client id value");
                if (result == null && request.Client == null)
                    result = BadRequest("Missing client data");
                if (result == null && string.IsNullOrEmpty(request.Client?.Name))
                    result = BadRequest("Missing client name value");
                if (result == null)
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient innerClient = await _clientFactory.Get(settings, id.Value);
                    if (innerClient == null)
                        result = NotFound();
                    if (result == null)
                    {
                        _mapper.Map<Client, IClient>(request.Client, innerClient);
                        await SetRoles(settings, request.Client, innerClient);
                        if (!string.IsNullOrEmpty(request.Secret))
                            innerClient.SetSecret(request.Secret);
                        await _clientSaver.Update(_settingsFactory.CreateCore(_settings.Value), innerClient);
                        result = Ok(await MapClient(settings, _mapper, innerClient));
                    }
                }
            }
            catch (Exception ex)
            {
                await WriteException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _ = WriteMetrics("update-client", DateTime.UtcNow.Subtract(start).TotalSeconds, new Dictionary<string, string> { { nameof(id), id.ToString() } });
            }
            return result;
        }

    }
}
