using System;

namespace YardLight.Interface.Models
{
    public class Itteration
    {
        public Guid? ItterationId { get; set; }
        public Guid? ProjectId { get; set; }
        public string Name { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public bool? Hidden { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public Guid? CreateUserId { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
