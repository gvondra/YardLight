using BrassLoon.DataClient;
using System;

namespace YardLight.Data.Models
{
    public class ItterationData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid? ItterationId { get; set; }
		[ColumnMapping()] public Guid ProjectId { get; set; }
        [ColumnMapping()] public string Name { get; set; }
        [ColumnMapping()] public DateTime? Start { get; set; }
        [ColumnMapping()] public DateTime? End { get; set; }
        [ColumnMapping()] public bool Hidden { get; set; }
        [ColumnMapping()] public bool Virtual { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping()] public Guid? CreateUserId { get; set; }
        [ColumnMapping()] public Guid? UpdateUserId { get; set; }
    }
}
