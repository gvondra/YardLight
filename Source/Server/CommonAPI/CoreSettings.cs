using YardLight.CommonCore;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.CommonAPI
{
    public class CoreSettings : ISettings
    {
        private static Policy _cache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new RelativeTtl(TimeSpan.FromMinutes(6)));
        public string ConnectionString { get; set; }
        public bool EnableDatabaseAccessToken { get; set; }
        public string ConnectionStringUser { get; set; }
        public string KeyVaultAddress { get; set; }

        public bool UseDefaultAzureSqlToken => EnableDatabaseAccessToken && string.IsNullOrEmpty(ConnectionStringUser);

        public async Task<string> GetConnetionString()
        {
            string result = ConnectionString;
            if (!string.IsNullOrEmpty(KeyVaultAddress) && !string.IsNullOrEmpty(ConnectionStringUser))
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                builder.UserID = ConnectionStringUser;
                builder.Password = await _cache.Execute<Task<string>>(async context =>
                {
                    SecretClientOptions options = new SecretClientOptions()
                    {
                        Retry =
                        {
                            Delay= TimeSpan.FromSeconds(2),
                            MaxDelay = TimeSpan.FromSeconds(16),
                            MaxRetries = 4,
                            Mode = RetryMode.Exponential
                         }
                    };
                    SecretClient client = new SecretClient(
                        new Uri(KeyVaultAddress),
                        new DefaultAzureCredential(
                            new DefaultAzureCredentialOptions()
                            {
                                ExcludeAzureCliCredential = false,
                                ExcludeAzurePowerShellCredential = false,
                                ExcludeSharedTokenCacheCredential = false,
                                ExcludeEnvironmentCredential = false,
                                ExcludeManagedIdentityCredential = false,
                                ExcludeVisualStudioCodeCredential = false,
                                ExcludeVisualStudioCredential = false
                            })
                        , options)
                    ;
                    KeyVaultSecret secret = await client.GetSecretAsync(ConnectionStringUser);
                    return secret.Value;
                },
                new Context(ConnectionString.ToLower().Trim().Replace(" ", string.Empty)));
                result = builder.ConnectionString;
            }
            return result;
        }

        public Func<Task<string>> GetDatabaseAccessToken()
        {
            return null;
        }
    }
}
