using System;
using System.Threading.Tasks;
using YardLight.CommonCore;

namespace YardLight.Framework
{
    public interface IProjectSaver
    {
        Task Create(ISettings settings, IProject project, Guid userId, string userEmailAddress);
        Task Update(ISettings settings, IProject project, Guid userId, string userEmailAddress);
    }
}
