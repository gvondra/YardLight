using BrassLoon.DataClient;
using System;

namespace YardLight.Data.Models
{
    public class ProjectData : DataManagedStateBase
    {
        [ColumnMapping("ProjectId", IsPrimaryKey = true)] public Guid ProjectId { get; set; }
        [ColumnMapping("Title")] public string Title { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
