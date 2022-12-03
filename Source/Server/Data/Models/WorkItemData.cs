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
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkItemId { get; set; }
        [ColumnMapping()] public Guid? ParentWorkItemId { get; set; }
        [ColumnMapping()] public Guid ProjectId { get; set; }
        [ColumnMapping()] public string Title { get; set; }
        [ColumnMapping()] public Guid TypeId { get; set; }
        [ColumnMapping()] public Guid StatusId { get; set; }
        [ColumnMapping()] public string Team { get; set; }
        [ColumnMapping()] public string Itteration { get; set; }
        [ColumnMapping()] public DateTime? StartDate { get; set; }
        [ColumnMapping()] public DateTime? TargetDate { get; set; }
        [ColumnMapping()] public DateTime? CloseDate { get; set; }
        [ColumnMapping()] public string Priority { get; set; }
        [ColumnMapping()] public string Effort { get; set; }
        [ColumnMapping()] public string Value { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping()] public Guid CreateUserId { get; set; }
        [ColumnMapping()] public Guid UpdateUserId { get; set; }
    }
}
