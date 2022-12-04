using BrassLoon.DataClient;
using System;

namespace YardLight.Data.Models
{
    public class ProjectUserData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid ProjectId { get; set; }
        [ColumnMapping(IsPrimaryKey = true)] public Guid UserId { get; set; }
        [ColumnMapping()] public bool IsActive { get; set; } = true;
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
