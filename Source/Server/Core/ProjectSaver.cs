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
        private readonly IProjectUserDataFactory _projectUserDataFactory;
        private readonly IProjectUserDataSaver _projectUserDataSaver;

        public ProjectSaver(IProjectUserDataFactory projectUserDataFactory, IProjectUserDataSaver projectUerDataSaver)
        {
            _projectUserDataFactory = projectUserDataFactory;
            _projectUserDataSaver = projectUerDataSaver;
        }

        public Task Create(ISettings settings, IProject project, Guid userId, string userEmailAddress)
        {
            Saver saver = new Saver();
            return saver.Save(new TransactionHandler(settings), th => project.Create(th, userId, userEmailAddress));
        }

        public async Task Update(ISettings settings, IProject project, Guid userId, string userEmailAddress)
        {
            Saver saver = new Saver();
            ProjectUserData projectUserData = await _projectUserDataFactory.Get(new DataSettings(settings), project.ProjectId, userId);
            await saver.Save(new TransactionHandler(settings), 
                async (th) =>
                {
                    if (projectUserData != null)
                    {
                        projectUserData.EmailAddress = userEmailAddress;
                        await _projectUserDataSaver.Update(th, projectUserData);
                    }
                    await project.Update(th);
                });
        }
    }
}
