﻿using System;

namespace API
{
    public class Settings
    {
        public string BrassLoonAccountApiBaseAddress { get; set; }
        public string BrassLoonLogApiBaseAddress { get; set; }
        public Guid? BrassLoonLogClientId { get; set; }
        public string BrassLoonLogClientSecret { get; set; }
        public Guid? LogDomainId { get; set; }
    }
}
