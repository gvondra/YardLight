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
    public class User : IUser
    {
        private readonly UserData _data;
        private readonly IRoleDataFactory _roleDataFactory;
        private readonly IUserDataSaver _dataSaver;
        private readonly IEmailAddressFactory _emailFactory;
        private IEmailAddress _email;
        private List<RoleData> _currentRoles;
        private List<RoleData> _newRoles;
        private List<RoleData> _removeRoles;

        public User(UserData data,
            IRoleDataFactory roleDataFactory,
            IUserDataSaver dataSaver,
            IEmailAddressFactory emailAddressFactory)
        {
            _data = data;
            _roleDataFactory = roleDataFactory;
            _dataSaver = dataSaver;
            _emailFactory = emailAddressFactory;
        }

        public Guid UserId => _data.UserId;
        public string ReferenceId => _data.ReferenceId;
        internal Guid EmailAddressId { get => _data.EmailAddressId; set => _data.EmailAddressId = value; }
        public string Name { get => _data.Name; set => _data.Name = value; }
        public DateTime CreateTimestamp => _data.CreateTimestamp;
        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        private async Task SaveRoles(ITransactionHandler transactionHandler)
        {
            if (_newRoles != null)
            foreach (RoleData data in _newRoles)
            {
                    await _dataSaver.AddRole(transactionHandler, UserId, data.RoleId);
            }
            if (_removeRoles != null)
            foreach (RoleData data in _removeRoles)
            {
                    await _dataSaver.RemoveRole(transactionHandler, UserId, data.RoleId);
            }
            _currentRoles = null;
            _newRoles = null;
            _removeRoles = null;
        }

        public async Task Create(ITransactionHandler transactionHandler)
        {
            if (_email != null)
                EmailAddressId = _email.EmailAddressId;
            await _dataSaver.Create(transactionHandler, _data);
            await SaveRoles(transactionHandler);
        }

        public async Task<IEmailAddress> GetEmailAddress(ISettings settings)
        {
            if (_email == null && !EmailAddressId.Equals(Guid.Empty))
                _email = await _emailFactory.Get(settings, EmailAddressId);
            return _email;
        }

        public void SetEmailAddress(IEmailAddress emailAddress)
        {
            if (emailAddress == null)
                throw new ArgumentNullException(nameof(emailAddress));
            _email = emailAddress;
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            if (_email != null)
                EmailAddressId = _email.EmailAddressId;
            await _dataSaver.Update(transactionHandler, _data);
            await SaveRoles(transactionHandler);
        }

        private async Task LoadCurrentRoles(ISettings settings)
        {
            if (_currentRoles == null && !UserId.Equals(Guid.Empty))
            {
                _currentRoles = (await _roleDataFactory.GetByUserId(new DataSettings(settings), UserId)).ToList();
            }
        }

        public async Task<Dictionary<string, string>> GetRoles(ISettings settings)
        {
            await LoadCurrentRoles(settings);
            return (_currentRoles ?? new List<RoleData>()).Concat(_newRoles ?? new List<RoleData>())
                .ToDictionary<RoleData, string, string>(d => d.PolicyName, d => d.Name);
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
    }
}
