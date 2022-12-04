using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Data.Models
{
    public class WorkItemCommentData : CommentData
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkItemId { get; set; }
        [ColumnMapping()] public short Type { get; set; }
    }
}
