using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IProjectFactory
    {
        IProject Create(string title);
        Task<IProject> Get(ISettings settings, Guid userId, Guid projectId);
        Task<IEnumerable<IProject>> GetByUserId(ISettings settings, Guid userId);
    }
}
