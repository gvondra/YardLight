using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Models
{
    public class Comment
    {
        public Guid? CommentId { get; set; }
        public string Text { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public Guid? CreateUserId { get; set; }
    }
}
