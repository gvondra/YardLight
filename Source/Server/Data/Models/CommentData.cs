using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Data.Models
{
    public class CommentData : DataManagedStateBase
    {
        [ColumnMapping("CommentId", IsPrimaryKey = true)] public Guid CommentId { get; set; }
        [ColumnMapping("Text")] public string Text { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("CreateUserId")] public Guid CreateUserId { get; set; }
    }
}
