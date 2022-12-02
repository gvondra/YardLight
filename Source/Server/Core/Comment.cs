using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;
using YardLight.Framework;

namespace YardLight.Core
{
    public abstract class Comment : IComment
    {
        private readonly CommentData _data;

        public Comment(CommentData data)
        {
            _data = data;
        }

        public Guid CommentId => _data.CommentId;

        public string Text => _data.Text;

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public Guid CreateUserId => _data.CreateUserId;
    }
}
