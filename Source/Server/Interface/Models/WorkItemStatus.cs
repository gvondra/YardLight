using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Models
{
    public class WorkItemStatus
    {
        public Guid? WorkItemStatusId { get; set; }
        public Guid? WorkItemTypeId { get; set; }
        public Guid? ProjectId { get; set; }
        public string Title { get; set; }
        public string ColorCode { get; set; }
        public short? Order { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? IsDefaultHidden { get; set; } = false;
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public Guid? CreateUserId { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
