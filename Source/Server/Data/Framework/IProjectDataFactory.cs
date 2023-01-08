using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IProjectDataFactory
    {
        Task<ProjectData> Get(ISettings settings, Guid projectId);
        Task<IEnumerable<ProjectData>> GetByEmailAddress(ISettings settings, string emailAddress);
    }
}
