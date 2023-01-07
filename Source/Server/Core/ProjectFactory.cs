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

        private Project Create(ProjectData data) => new Project(data, _dataSaver);

        public IProject Create(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));
            Project project = Create(new ProjectData());
            project.Title = title;
            return project;
        }

        public async Task<IProject> Get(ISettings settings, Guid userId, Guid projectId)
        {
            IProject project = null;
            if (await GetProjectUserIsActive(settings, userId, projectId))
            {
                ProjectData data = await _dataFactory.Get(new DataSettings(settings), projectId);
                if (data != null)
                    project = Create(data);
            }
            return project;
        }

        public async Task<IEnumerable<IProject>> GetByUserId(ISettings settings, Guid userId)
        {
            return (await _dataFactory.GetByUserId(new DataSettings(settings), userId))
                .Select<ProjectData, IProject>(Create);
        }

        public Task<bool> GetProjectUserIsActive(ISettings settings, Guid userId, Guid projectId)
        {
            return _projectUserIsActiveCache.Execute(async (context) =>
            {
                ProjectUserData data = await _projectUserDataFactory.Get(new DataSettings(settings), projectId, userId);
                return data?.IsActive ?? false;
            },
            new Context($"{projectId.ToString("N")}:{userId.ToString("N")}"));            
        }
    }
}
