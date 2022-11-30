using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IWorkItemDataFactory
    {
        Task<WorkItemData> Get(ISettings settings, Guid id);
        Task<IEnumerable<WorkItemData>> GetByProjectId(ISettings settings, Guid projectId);
    }
}
