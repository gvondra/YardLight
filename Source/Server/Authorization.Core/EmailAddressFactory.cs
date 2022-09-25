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
    public class EmailAddressFactory : IEmailAddressFactory
    {
        private readonly IEmailAddressDataFactory _dataFactory;
        private readonly IEmailAddressDataSaver _dataSaver;

        public EmailAddressFactory(IEmailAddressDataFactory dataFactory,
            IEmailAddressDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private EmailAddress Create(EmailAddressData data) => new EmailAddress(data, _dataSaver);

        public IEmailAddress Create(string address)
        {
            return Create(new EmailAddressData { Address = (address ?? string.Empty).Trim() });
        }

        public async Task<IEmailAddress> Get(ISettings settings, Guid id)
        {
            EmailAddress emailAddress = null;
            EmailAddressData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                emailAddress = Create(data);
            return emailAddress;
        }

        public async Task<IEmailAddress> GetByAddress(ISettings settings, string address)
        {
            EmailAddress emailAddress = null;
            EmailAddressData data = await _dataFactory.GetByAddress(new DataSettings(settings), address);
            if (data != null)
                emailAddress = Create(data);
            return emailAddress;
        }
    }
}
