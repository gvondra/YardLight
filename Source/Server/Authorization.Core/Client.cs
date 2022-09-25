using YardLight.Authorization.Data.Framework;
using YardLight.Authorization.Data.Models;
using YardLight.Authorization.Core.Framework;
using YardLight.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core
{
    public class Client : IClient
    {
        private readonly ClientData _data;
        private readonly IClientDataFactory _dataFactory;
        private readonly IClientDataSaver _dataSaver;
        private readonly IRoleDataFactory _roleDataFactory;
        private string _newSecret;
        private List<RoleData> _currentRoles;
        private List<RoleData> _newRoles;
        private List<RoleData> _removeRoles;

        public Client(ClientData data,
            IClientDataFactory dataFactory,
            IClientDataSaver dataSaver,
            IRoleDataFactory roleDataFactory)
        {
            _data = data;
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _roleDataFactory = roleDataFactory;
        }

        public Guid ClientId => _data.ClientId;

        public string Name { get => _data.Name; set => _data.Name = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        private async Task LoadCurrentRoles(ISettings settings)
        {
            if (_currentRoles == null && !ClientId.Equals(Guid.Empty))
            {
                _currentRoles = (await _roleDataFactory.GetByClientId(new DataSettings(settings), ClientId)).ToList();
            }
        }

        public async Task AddRole(ISettings settings, string policyName)
        {
            await LoadCurrentRoles(settings);
            if (_currentRoles == null)
                _currentRoles = new List<RoleData>();
            if (_newRoles == null)
                _newRoles = new List<RoleData>();
            if (!_currentRoles.Concat(_newRoles).Any(d => string.Equals(d.PolicyName, policyName, StringComparison.OrdinalIgnoreCase)))
            {
                RoleData data = (await _roleDataFactory.GetAll(new DataSettings(settings)))
                    .FirstOrDefault(d => string.Equals(d.PolicyName, policyName, StringComparison.OrdinalIgnoreCase));
                if (data == null)
                    throw new ApplicationException($"Role with policy \"{policyName}\" not found");
                _newRoles.Add(data);
            }
        }

        public async Task Create(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Create(transactionHandler, _data);
            await SaveNewSecret(transactionHandler);
            await SaveRoles(transactionHandler);
        }

        private async Task SaveRoles(ITransactionHandler transactionHandler)
        {
            if (_newRoles != null)
                foreach (RoleData data in _newRoles)
                {
                    await _dataSaver.AddRole(transactionHandler, ClientId, data.RoleId);
                }
            if (_removeRoles != null)
                foreach (RoleData data in _removeRoles)
                {
                    await _dataSaver.RemoveRole(transactionHandler, ClientId, data.RoleId);
                }
            _currentRoles = null;
            _newRoles = null;
            _removeRoles = null;
        }

        public async Task<Dictionary<string, string>> GetRoles(ISettings settings)
        {
            await LoadCurrentRoles(settings);
            return (_currentRoles ?? new List<RoleData>()).Concat(_newRoles ?? new List<RoleData>())
                .ToDictionary<RoleData, string, string>(d => d.PolicyName, d => d.Name);
        }

        public async Task RemoveRole(ISettings settings, string policyName)
        {
            await LoadCurrentRoles(settings);
            if (_currentRoles == null)
                _currentRoles = new List<RoleData>();
            if (_removeRoles == null)
                _removeRoles = new List<RoleData>();
            int index;
            index = _currentRoles.FindIndex(d => string.Equals(d.PolicyName, policyName, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                _removeRoles.Add(_currentRoles[index]);
                _currentRoles.RemoveAt(index);
            }
            if (_newRoles != null)
            {
                index = _newRoles.FindIndex(d => string.Equals(d.PolicyName, policyName, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    _newRoles.RemoveAt(index);
                }
            }
        }

        public void SetSecret(string secret)
        {
            _newSecret = secret;
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Update(transactionHandler, _data);
            await SaveNewSecret(transactionHandler);
            await SaveRoles(transactionHandler);
        }

        public async Task<bool> VerifySecret(ISettings settings, string secret)
        {
            return ClientSecretProcessor.Verify(secret,
                await GetSecretHash(settings)
                );
        }

        private async Task<byte[]> GetSecretHash(ISettings settings)
        {
            return (await _dataFactory.GetClientCredentials(new DataSettings(settings), ClientId))
                .OrderByDescending(c => c.CreateTimestamp)
                .Select<ClientCredentialData, byte[]>(c => c.Secret)
                .FirstOrDefault();              
        }

        private async Task SaveNewSecret(ITransactionHandler transactionHandler)
        {
            if (!string.IsNullOrEmpty(_newSecret))
            {
                ClientCredentialData data = new ClientCredentialData
                { 
                    ClientId = ClientId,
                    Secret = ClientSecretProcessor.Hash(_newSecret)
                };
                await _dataSaver.Create(transactionHandler, data);
            }            
        }
    }
}
