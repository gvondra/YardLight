using System;

namespace YardLight.Interface.Models
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public DateTime UpdateTimestamp { get; set; }
    }
}
