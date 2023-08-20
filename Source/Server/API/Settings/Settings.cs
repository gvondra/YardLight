using System;

namespace API
{
    public class Settings
    {
        public string ConnectionString { get; set; }
        public bool EnableDatabaseAccessToken { get; set; } = false;
        public string BrassLoonAccountApiBaseAddress { get; set; }
        public string BrassLoonLogApiBaseAddress { get; set; }
        public string BrassLoonLogRpcAddress { get; set; }
        public Guid? BrassLoonLogClientId { get; set; }
        public string BrassLoonLogClientSecret { get; set; }
        public Guid? AuthorizationDomainId { get; set; }
        public Guid? LogDomainId { get; set; }
        public string AuthorizationApiBaseAddress { get; set; }
    }
}
