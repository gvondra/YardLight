using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Data.Models
{
    public class WorkItemTypeData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkItemTypeId { get; set; }
        [ColumnMapping()] public Guid ProjectId { get; set; }
        [ColumnMapping()] public string Title { get; set; }
        [ColumnMapping()] public string ColorCode { get; set; } = "black";
        [ColumnMapping()] public bool IsActive { get; set; } = true;
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping()] public Guid CreateUserId { get; set; }
        [ColumnMapping()] public Guid UpdateUserId { get; set; }
    }
}
