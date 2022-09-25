using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data.Models
{
    public class ClientData : DataManagedStateBase
    {
        [ColumnMapping("ClientId", IsPrimaryKey = true)] public Guid ClientId { get; set; }
        [ColumnMapping("Name")] public string Name { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
