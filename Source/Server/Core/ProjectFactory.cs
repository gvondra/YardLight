using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Data.Framework;
using YardLight.Data.Models;
using YardLight.Framework;

namespace YardLight.Core
{
    public class ProjectFactory : IProjectFactory
    {
        private static Policy _projectUserIsActiveCache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(3));
        private readonly IProjectDataFactory _dataFactory;
        private readonly IProjectUserDataFactory _projectUserDataFactory;
        private readonly IProjectDataSaver _dataSaver;

        public ProjectFactory(IProjectDataFactory dataFactory, 
            IProjectUserDataFactory projectUserDataFactory,
            IProjectDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _projectUserDataFactory = projectUserDataFactory;
            _dataSaver = dataSaver;
        }

        private Project Create(ProjectData data) => new Project(data, _dataSaver, this);

        public IProject Create(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));
            Project project = Create(new ProjectData());
            project.Title = title;
            return project;
        }

        public async Task<IProject> Get(ISettings settings, string emailAddress, Guid projectId)
        {
            IProject project = null;
            if (await GetProjectUserIsActive(settings, emailAddress, projectId))
            {
                ProjectData data = await _dataFactory.Get(new DataSettings(settings), projectId);
                if (data != null)
                    project = Create(data);
            }
            return project;
        }

        public async Task<IEnumerable<IProject>> GetByEmailAddress(ISettings settings, string emailAddress)
        {
            return (await _dataFactory.GetByEmailAddress(new DataSettings(settings), emailAddress))
                .Select<ProjectData, IProject>(Create);
        }

        public Task<bool> GetProjectUserIsActive(ISettings settings, string emailAddress, Guid projectId)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));
            return _projectUserIsActiveCache.Execute(async (context) =>
            {
                ProjectUserData data = await _projectUserDataFactory.Get(new DataSettings(settings), projectId, emailAddress);
                return data?.IsActive ?? false;
            },
            new Context($"{projectId.ToString("N")}:{emailAddress.ToLower()}"));            
        }

        public async Task<IEnumerable<string>> GetUsers(ISettings settings, Guid projectId)
        {
            return (await _projectUserDataFactory.GetByProjectId(new DataSettings(settings), projectId))
                .Select(u => u.EmailAddress)
                .OrderBy(u => u?.ToLower() ?? string.Empty);
        }
    }
}
