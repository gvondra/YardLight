using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.Behaviors
{
    public class BacklogVMLoader
    {
        private readonly BacklogVM _backlogVM;

        public BacklogVMLoader(BacklogVM backlogVM)
        {
            _backlogVM = backlogVM;
        }

        public void Load()
        {
            UserSession userSession = UserSessionLoader.GetUserSession();
            Task.Run(() => LoadProject(userSession.OpenProjectId.Value))
                .ContinueWith(LoadProjectCallback, userSession.OpenProjectId.Value, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<Project> LoadProject(Guid projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IProjectService projectService = scope.Resolve<IProjectService>();
                return projectService.Get(settingsFactory.CreateApi(), projectId);
            }
        }

        private async Task LoadProjectCallback(Task<Project> loadProject, object state)
        {
            try
            {
                Project project = await loadProject;
                if (state == null)
                    state = Guid.Empty;
                if (((Guid)state).Equals(project.ProjectId))
                {
                    _backlogVM.Project = project;
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }
    }
}
