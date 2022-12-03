using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IWorkItemCommentSaver
    {
        Task Create(ISettings settings, IWorkItemComment workItemComment, Guid userId);
    }
}
