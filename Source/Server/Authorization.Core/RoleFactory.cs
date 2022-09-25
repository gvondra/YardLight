using YardLight.Authorization.Data.Framework;
using YardLight.Authorization.Data.Models;
using YardLight.Authorization.Core.Framework;
using YardLight.CommonCore;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Core
{
    public class RoleFactory : IRoleFactory
    {
        private const string CACHE_KEY = "all-roles";
        private static MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        private static readonly CachePolicy _cache = Policy.Cache(new MemoryCacheProvider(_memoryCache), new SlidingTtl(TimeSpan.FromMinutes(5)));
        private readonly IRoleDataFactory _dataFactory;
        private readonly IRoleDataSaver _dataSaver;

        public RoleFactory(IRoleDataFactory dataFactory,
            IRoleDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        internal static void ClearCache()
        {
            _memoryCache.Remove(CACHE_KEY);
        }

        private Role Create(RoleData data) => new Role(data, _dataSaver);

        public async Task<IRole> Get(ISettings settings, int id)
        {
            Role role = null;
            RoleData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                role = Create(data);
            return role;
        }

        public async Task<IEnumerable<IRole>> GetAll(ISettings settings)
        {            
            
            return await _cache.Execute<Task<IEnumerable<IRole>>>(async (context) =>
            {
                return (await _dataFactory.GetAll(new DataSettings(settings)))
                .Select<RoleData, IRole>(d => Create(d));
            },
            new Context(CACHE_KEY));            
        }

        public async Task<IEnumerable<IRole>> GetByUserId(ISettings settings, Guid userId)
        {
            return (await _dataFactory.GetByUserId(new DataSettings(settings), userId))
                .Select<RoleData, IRole>(d => Create(d));
        }

        public IRole Create(string policyName)
        {
            if (string.IsNullOrEmpty(policyName))
                throw new ArgumentNullException(nameof(policyName));
            return Create(new RoleData { PolicyName = policyName });
        }

        public async Task<IEnumerable<IRole>> GetByClientId(ISettings settings, Guid clientId)
        {
            return (await _dataFactory.GetByClientId(new DataSettings(settings), clientId))
                .Select<RoleData, IRole>(d => Create(d));
        }
    }
}
