using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.Interface.Models;
namespace YardLight.Interface
{
    public interface IProjectService
    {
        Task<Project> Get(ISettings settings, Guid id);
        Task<string[]> GetUsers(ISettings settings, Guid id);
        Task<List<Project>> Get(ISettings settings);
        Task<Project> Create(ISettings settings, Project project);
        Task<Project> Update(ISettings settings, Project project);
        Task<Project> Update(ISettings settings, Guid id, Project project);
        Task AddUser(ISettings settings, Guid id, string emailAddress);
        Task RemoveUser(ISettings settings, Guid id, string emailAddress);
    }
}
