using BrassLoon.DataClient;
using System;

namespace YardLight.Data.Models
{
    public class ProjectData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid ProjectId { get; set; }
        [ColumnMapping()] public string Title { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
