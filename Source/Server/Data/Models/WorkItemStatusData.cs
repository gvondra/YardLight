﻿using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Data.Models
{
    public class WorkItemStatusData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkItemStatusId { get; set; }
        [ColumnMapping()] public Guid WorkItemTypeId { get; set; }
        [ColumnMapping()] public Guid ProjectId { get; set; }
        [ColumnMapping()] public string Title { get; set; }
        [ColumnMapping()] public string ColorCode { get; set; } = "black";
        [ColumnMapping()] public short Order { get; set; } = 0;
        [ColumnMapping()] public bool IsActive { get; set; } = true;
        [ColumnMapping()] public bool IsDefaultHidden { get; set; } = false;
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping()] public Guid CreateUserId { get; set; }
        [ColumnMapping()] public Guid UpdateUserId { get; set; }
    }
}
