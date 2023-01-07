using System;
using System.Threading.Tasks;

namespace YardLight.CommonCore
{
    public class DataSettings : BrassLoon.DataClient.ISqlSettings
    {
        private readonly ISettings _settings;

        public DataSettings(ISettings settings)
        {
            _settings = settings;
        }

        public Func<Task<string>> GetAccessToken => _settings.GetDatabaseAccessToken();

        public bool UseDefaultAzureToken => _settings.UseDefaultAzureSqlToken;

        public Task<string> GetConnectionString() => _settings.GetConnetionString();
    }
}
