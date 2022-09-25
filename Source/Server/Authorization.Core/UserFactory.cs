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
    public class UserFactory : IUserFactory
    {
        private readonly IUserDataFactory _dataFactory;
        private readonly IUserDataSaver _dataSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly IRoleDataFactory _roleDataFactory;

        public UserFactory(IUserDataFactory dataFactory,
            IUserDataSaver dataSaver,
            IEmailAddressFactory emailAddressFactory,
            IRoleDataFactory roleDataFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _emailAddressFactory = emailAddressFactory;
            _roleDataFactory = roleDataFactory;
        }

        private User Create(UserData data) => new User(data, _roleDataFactory, _dataSaver, _emailAddressFactory);

        public IUser Create(string referenceId, IEmailAddress emailAddress)
        {
            if (string.IsNullOrEmpty(referenceId))
                throw new ArgumentNullException(nameof(referenceId));
            if (emailAddress == null)
                throw new ArgumentNullException(nameof(emailAddress));
            User user = Create(new UserData() { ReferenceId = referenceId });
            user.SetEmailAddress(emailAddress);
            return user;
        }

        public async Task<IUser> Get(ISettings settings, Guid id)
        {
            User user = null;
            UserData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                user = Create(data);
            return user;
        }

        public async Task<IEnumerable<IUser>> GetByEmailAddress(ISettings settings, string emailAddress)
        {
            return (await _dataFactory.GetByEmailAddress(new DataSettings(settings), emailAddress))
                .Select<UserData, IUser>(u => Create(u))
                ;
        }

        public async Task<IUser> GetByReferenceId(ISettings settings, string referenceId)
        {
            User user = null;
            UserData data = await _dataFactory.GetByReferenceId(new DataSettings(settings), referenceId);
            if (data != null)
                user = Create(data);
            return user;
        }
    }
}
