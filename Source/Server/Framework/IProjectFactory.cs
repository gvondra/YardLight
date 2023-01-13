using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IProjectFactory
    {
        IProject Create(string title);
        Task<IProject> Get(ISettings settings, string emailAddress, Guid projectId);
        Task<IEnumerable<IProject>> GetByEmailAddress(ISettings settings, string emailAddress);
        Task<IEnumerable<string>> GetUsers(ISettings settings, Guid projectId);
    }
}
