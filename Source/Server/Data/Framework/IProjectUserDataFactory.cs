using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IProjectUserDataFactory
    {
        Task<ProjectUserData> Get(ISettings settings, Guid projectId, string emailAddress);
        Task<IEnumerable<ProjectUserData>> GetByProjectId(ISettings settings, Guid projectId);
    }
}
