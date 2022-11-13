using BrassLoon.DataClient;
using System;

namespace YardLight.Data.Models
{
    public class ProjectUserData : DataManagedStateBase
    {
        [ColumnMapping("ProjectId", IsPrimaryKey = true)] public Guid ProjectId { get; set; }
        [ColumnMapping("UserId", IsPrimaryKey = true)] public Guid UserId { get; set; }
        [ColumnMapping("IsActive")] public bool IsActive { get; set; } = true;
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
