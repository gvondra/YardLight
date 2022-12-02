using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Models
{
    public class WorkItem
    {
        public Guid? WorkItemId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? ParentWorkItemId { get; set; }
        public string Title { get; set; }
        public WorkItemType Type { get; set; }
        public WorkItemStatus Status { get; set; }
        public string Team { get; set; }
        public string Itteration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Priority { get; set; }
        public string Effort { get; set; }
        public string Value { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public Guid? CreateUserId { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }
    }
}
