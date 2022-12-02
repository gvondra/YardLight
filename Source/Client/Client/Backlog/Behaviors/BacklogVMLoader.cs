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
            _backlogVM.CanRefresh = false;
            UserSession userSession = UserSessionLoader.GetUserSession();
            Task.Run(() => LoadProject(userSession.OpenProjectId.Value))
                .ContinueWith(LoadProjectCallback, userSession.OpenProjectId.Value, TaskScheduler.FromCurrentSynchronizationContext());
            if (!_backlogVM.ContainsBehavior<CreateWorkItemLoader>())
                _backlogVM.AddBehavior(new CreateWorkItemLoader(_backlogVM.CreateWorkItemVM));
            if (_backlogVM.RefreshBackLogCommand == null)
                _backlogVM.RefreshBackLogCommand = new RefreshBackLogCommand(_backlogVM, this);
        }

        private Task<List<WorkItem>> LoadAllWorkItems(Guid projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                return workItemService.GetByProjectId(settingsFactory.CreateApi(), projectId);
            }
        }

        private async Task LoadAllWorkItemsCallback(Task<List<WorkItem>> loadAllWorkItems, object state)
        {
            try
            {
                _backlogVM.RootWorkItems.Clear();
                List<WorkItemVM> items = new List<WorkItemVM>();
                foreach (WorkItem workItem in await loadAllWorkItems)
                {
                    WorkItemVM workItemVM = new WorkItemVM(_backlogVM, workItem);
                    workItemVM.AddBehavior(new CreateWorkItemLoader(workItemVM.CreateWorkItemVM));
                    items.Add(workItemVM);
                    if (!workItemVM.ParentWorkItemId.HasValue)
                    {
                        _backlogVM.RootWorkItems.Add(workItemVM);
                        _backlogVM.AddBehavior(new WorkItemLoader(workItemVM));
                    }
                }
                foreach (WorkItemVM item in items.Where(i => i.ParentWorkItemId.HasValue))
                {
                    WorkItemVM parent = items.FirstOrDefault(i => item.ParentWorkItemId.Value.Equals(i.WorkItemId.Value));
                    parent.AddBehavior(new WorkItemLoader(item));
                    parent.Children.Add(item);
                }
                _backlogVM.CanRefresh = true;
            }
            catch(System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
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
                _ = Task.Run(() => LoadAllWorkItems(_backlogVM.Project.ProjectId))
                    .ContinueWith(LoadAllWorkItemsCallback, _backlogVM.Project.ProjectId, TaskScheduler.FromCurrentSynchronizationContext());
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
                    _ = Task.Run(() => LoadAvailableWorkItemTypes(project.ProjectId))
                    .ContinueWith(LoadAvailableWorkItemTypesCallback, project.ProjectId, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }
    }
}
