﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Models
{
    public class User
    {
        public Guid? UserId { get; set; }
        public string ReferenceId { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public List<AppliedRole> Roles { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
