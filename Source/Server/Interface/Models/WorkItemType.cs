using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Models
{
    public class WorkItemType
    {
        public Guid? WorkItemTypeId { get; set; }
        public Guid? ProjectId { get; set; }
        public string Title { get; set; }
        public string ColorCode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public Guid? CreateUserId { get; set; }
        public Guid? UpdateUserId { get; set; }

        public List<WorkItemStatus> Statuses { get; set; }
    }
}
