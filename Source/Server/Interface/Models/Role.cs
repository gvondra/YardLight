using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Models
{
    public class Role
    {
        public Guid? RoleId { get; set; }
        public string Name { get; set; }
        public string PolicyName { get; set; }
        public bool? IsActive { get; set; }
        public string Comment { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
