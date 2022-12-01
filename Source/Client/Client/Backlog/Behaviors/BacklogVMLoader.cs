using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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
            Task.Run(() => LoadAvailableWorkItemTypes(userSession.OpenProjectId.Value))
                .ContinueWith(LoadAvailableWorkItemTypesCallback, userSession.OpenProjectId.Value, TaskScheduler.FromCurrentSynchronizationContext());
            _backlogVM.AddBehavior(new CreateWorkItemLoader(_backlogVM.CreateWorkItemVM));
        }

        private Task<List<WorkItemType>> LoadAvailableWorkItemTypes(Guid projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemTypeService workItemTypeService = scope.Resolve<IWorkItemTypeService>();
                return workItemTypeService.GetByProjectId(settingsFactory.CreateApi(), projectId, true);
            }
        }

        private async Task LoadAvailableWorkItemTypesCallback(Task<List<WorkItemType>> loadAvailableWorkItemTypes, object state)
        {
            try
            {
                _backlogVM.AvailableTypes.Clear();
                foreach (WorkItemType workItemType in await loadAvailableWorkItemTypes)
                {
                    _backlogVM.AvailableTypes.Add(new WorkItemTypeVM(workItemType));
                }
                if (_backlogVM.AvailableTypes.Count > 0) 
                    _backlogVM.CreateWorkItemVM.SelectedNewItemType = _backlogVM.AvailableTypes[0];
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
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
