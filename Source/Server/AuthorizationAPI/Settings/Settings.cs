using System;

namespace AuthorizationAPI
{
    public class Settings
    {
        public string BrassLoonLogApiBaseAddress { get; set; }
        public string BrassLoonAccountApiBaseAddress { get; set; }
        public Guid BrassLoonLogClientId { get; set; }
        public string BrassLoonLogClientSecret { get; set; }
        public string ConnectionString { get; set; }
        public string IdIssuer { get; set; }
        public bool EnableDatabaseAccessToken { get; set; } = false;
        public string ExternalIdIssuer { get; set; }
        public Guid LogDomainId { get; set; }
        public string SuperUser { get; set; }
        public string TknCsp { get; set; }
    }
}
