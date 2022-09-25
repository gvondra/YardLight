using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data.Models
{
    public class EmailAddressData : DataManagedStateBase
    {
        [ColumnMapping("EmailAddressId", IsPrimaryKey = true)] public Guid EmailAddressId { get; set; }
        [ColumnMapping("Address")] public string Address { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
