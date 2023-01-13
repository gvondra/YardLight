using System;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Data.Framework;
using YardLight.Data.Models;
using YardLight.Framework;

namespace YardLight.Core
{
    public class ProjectSaver : IProjectSaver
    {
        private readonly IProjectUserDataSaver _projectUserDataSaver;

        public ProjectSaver(IProjectUserDataSaver projectUserDataSaver)
        {
            _projectUserDataSaver = projectUserDataSaver;
        }

        public Task Create(ISettings settings, IProject project, string userEmailAddress)
        {
            Saver saver = new Saver();
            return saver.Save(new TransactionHandler(settings), th => project.Create(th, userEmailAddress));
        }

        public async Task Update(ISettings settings, IProject project)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), project.Update);
        }

        public async Task UpdateProjectUser(ISettings settings, Guid projectId, string emailAddress, bool isActive)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), async th =>
            {
                ProjectUserData data = new ProjectUserData
                {
                    ProjectId = projectId,
                    EmailAddress = emailAddress,
                    IsActive = isActive
                };
                await _projectUserDataSaver.Update(th, data);
            });
        }
    }
}
