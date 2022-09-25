using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data.Models
{
    public class UserData : DataManagedStateBase
    {
        [ColumnMapping("UserId", IsPrimaryKey = true)] public Guid UserId { get; set; }
        [ColumnMapping("ReferenceId")] public string ReferenceId { get; set; }
        [ColumnMapping("EmailAddressId")] public Guid EmailAddressId { get; set; }
        [ColumnMapping("Name")] public string Name { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc =true)] public DateTime UpdateTimestamp { get; set; }
    }
}
