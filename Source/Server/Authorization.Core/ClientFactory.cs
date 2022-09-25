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
    public class ClientFactory : IClientFactory
    {
        private readonly IClientDataFactory _dataFactory;
        private readonly IClientDataSaver _dataSaver;
        private readonly IRoleDataFactory _roleDataFactory;

        public ClientFactory(IClientDataFactory dataFactory,
            IClientDataSaver dataSaver,
            IRoleDataFactory roleDataFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _roleDataFactory = roleDataFactory;
        }

        private Client Create(ClientData data) => new Client(data, _dataFactory, _dataSaver, _roleDataFactory);

        public IClient Create(string secret)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException(nameof(secret));
            Client client = Create(new ClientData());
            client.SetSecret(secret);
            return client;
        }

        public async Task<IClient> Get(ISettings settings, Guid id)
        {
            Client client = null;
            ClientData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                client = Create(data);
            return client;
        }

        public async Task<IEnumerable<IClient>> GetAll(ISettings settings)
        {
            return (await _dataFactory.GetAll(new DataSettings(settings)))
                .Select<ClientData, IClient>(d => Create(d));
        }
    }
}
