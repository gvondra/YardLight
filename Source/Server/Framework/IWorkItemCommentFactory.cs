using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework.Enumerations;

namespace YardLight.Framework
{
    public interface IWorkItemCommentFactory
    {
        IWorkItemComment Create(IWorkItem workItem, string text, WorkItemCommentType workItemCommentType);
        Task<IEnumerable<IWorkItemComment>> GetByWorkItem(ISettings settings, IWorkItem workItem);
    }
}
