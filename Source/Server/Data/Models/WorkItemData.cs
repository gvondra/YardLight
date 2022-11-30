using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Data.Models
{
    public class WorkItemData : DataManagedStateBase
    {
        [ColumnMapping("WorkItemId", IsPrimaryKey = true)] public Guid WorkItemId { get; set; }
        [ColumnMapping("ProjectId")] public Guid ProjectId { get; set; }
        [ColumnMapping("Title")] public string Title { get; set; }
        [ColumnMapping("TypeId")] public Guid TypeId { get; set; }
        [ColumnMapping("StatusId")] public Guid StatusId { get; set; }
        [ColumnMapping("Team")] public string Team { get; set; }
        [ColumnMapping("Itteration")] public string Itteration { get; set; }
        [ColumnMapping("StartDate")] public DateTime? StartDate { get; set; }
        [ColumnMapping("TargetDate")] public DateTime? TargetDate { get; set; }
        [ColumnMapping("CloseDate")] public DateTime? CloseDate { get; set; }
        [ColumnMapping("Priority")] public string Priority { get; set; }
        [ColumnMapping("Effort")] public string Effort { get; set; }
        [ColumnMapping("Value")] public string Value { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping("CreateUserId")] public Guid CreateUserId { get; set; }
        [ColumnMapping("UpdateUserId")] public Guid UpdateUserId { get; set; }
    }
}
