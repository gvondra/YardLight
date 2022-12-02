using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Framework
{
    public interface IComment
    {
        Guid CommentId { get;}
        string Text { get; }
        DateTime CreateTimestamp { get; }
        Guid CreateUserId { get; }
    }
}
