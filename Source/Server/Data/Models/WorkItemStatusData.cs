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
        [ColumnMapping("WorkItemStatusId")] public Guid WorkItemStatusId { get; set; }
        [ColumnMapping("ProjectId")] public Guid ProjectId { get; set; }
        [ColumnMapping("Title")] public string Title { get; set; }
        [ColumnMapping("ColorCode")] public string ColorCode { get; set; } = "black";
        [ColumnMapping("Order")] public short Order { get; set; } = 0;
        [ColumnMapping("IsActive")] public bool IsActive { get; set; } = true;
	    [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping("CreateUserId")] public Guid CreateUserId { get; set; }
        [ColumnMapping("UpdateUserId")] public Guid UpdateUserId { get; set; }
    }
}