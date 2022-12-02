using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IWorkItemCommentDataFactory
    {
        Task<IEnumerable<WorkItemCommentData>> GetByWorkItemId(ISettings settings, Guid workItemId);
    }
}
