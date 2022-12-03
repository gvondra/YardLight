using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public interface IWorkItemCommentService
    {
        Task<List<Comment>> GetByWorkItemId(ISettings settings, Guid projectId, Guid workItemId);
        Task<Comment> Create(ISettings settings, Guid projectId, Guid workItemId, Comment comment);
    }
}
