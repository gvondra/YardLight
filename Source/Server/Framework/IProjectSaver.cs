using System;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IProjectSaver
    {
        Task Create(ISettings settings, IProject project, string userEmailAddress);
        Task Update(ISettings settings, IProject project);
        Task UpdateProjectUser(ISettings settings, Guid projectId, string emailAddress, bool isActive);
    }
}
