using System;

namespace AuthorizationAPI
{
    public class Settings
    {
        public string InternalIdIssuer { get; set; }
        public string ExternalIdIssuer { get; set; }
        public string LogApiBaseAddress { get; set; }
        public Guid LogDomainId { get; set; }
        public string SuperUser { get; set; }
        public string TknCsp { get; set; }
    }
}
