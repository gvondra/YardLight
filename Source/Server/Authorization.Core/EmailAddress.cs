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
    public class EmailAddress : IEmailAddress
    {
        private readonly EmailAddressData _data;
        private readonly IEmailAddressDataSaver _dataSaver;

        public EmailAddress(EmailAddressData data,
            IEmailAddressDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid EmailAddressId { get => _data.EmailAddressId; }
        public string Address { get => _data.Address; }
        public DateTime CreateTimestamp { get => _data.CreateTimestamp; }

        public Task Create(ITransactionHandler transactionHandler) => _dataSaver.Create(transactionHandler, _data);
    }
}
