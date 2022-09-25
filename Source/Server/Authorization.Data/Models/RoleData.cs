using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data.Models
{
    public class RoleData : DataManagedStateBase
    {
        [ColumnMapping("RoleId", IsPrimaryKey = true)] public int RoleId { get; set; }
        [ColumnMapping("Name")] public string Name { get; set; }
        [ColumnMapping("PolicyName")] public string PolicyName { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
