using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            _backlogVM.BusyVisibility = Visibility.Visible;
            _backlogVM.RootWorkItems = new ReadOnlyCollection<WorkItemVM>(new List<WorkItemVM>());
            UserSession userSession = UserSessionLoader.GetUserSession();
            Task.Run(() => LoadProject(userSession.OpenProjectId.Value))
                .ContinueWith(LoadProjectCallback, userSession.OpenProjectId.Value, TaskScheduler.FromCurrentSynchronizationContext());
            Task.Run(() => LoadItterations(userSession.OpenProjectId))
                .ContinueWith(LoadItterationsCallback, userSession.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
            Task.Run(() => LoadTeams(userSession.OpenProjectId))
                .ContinueWith(LoadTeamsCallback, userSession.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
            if (!_backlogVM.ContainsBehavior<CreateWorkItemLoader>())
                _backlogVM.AddBehavior(new CreateWorkItemLoader(_backlogVM.CreateWorkItemVM));
            if (_backlogVM.RefreshBackLogCommand == null)
                _backlogVM.RefreshBackLogCommand = new RefreshBackLogCommand(_backlogVM, this);
        }

        private Task<List<string>> LoadItterations(Guid? projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                if (projectId.HasValue)
                    return workItemService.GetItterationsByProjectId(settingsFactory.CreateApi(), projectId.Value);
                else
                    return Task.FromResult(new List<string>());
            }
        }

        private async Task LoadItterationsCallback(Task<List<string>> loadItterations, object state)
        {
            try
            {
                _backlogVM.Filter.Itterations = await loadItterations;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }

        private Task<List<string>> LoadTeams(Guid? projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                if (projectId.HasValue)
                    return workItemService.GetTeamsByProjectId(settingsFactory.CreateApi(), projectId.Value);
                else
                    return Task.FromResult(new List<string>());
            }
        }

        private async Task LoadTeamsCallback(Task<List<string>> loadTeams, object state)
        {
            try
            {
                _backlogVM.Filter.Teams = await loadTeams;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }

        private Task<List<WorkItem>> LoadWorkItems(Guid projectId, params Guid[] parentIds)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                ISettings settings = settingsFactory.CreateApi();
                if (parentIds != null && parentIds.Length > 0)
                    return workItemService.GetByParentIds(settings, projectId, parentIds);
                else
                    return workItemService.GetByProjectId(settings, projectId);
            }
        }
                
        private async Task LoadWorkItemsCallback(Task<List<WorkItem>> loadWorkItems, object state)
        {
            try
            {
                WorkItemVM item;
                WorkItemLoader itemLoader;
                List<WorkItemVM> items = new List<WorkItemVM>();
                Action<object> addBehavior;
                ReadOnlyCollection<WorkItemVM> currentChildItems;
                Action<object, IEnumerable<WorkItemVM>, IEnumerable<WorkItemVM>> setChildItems;
                bool isExpanded = false;
                if (state.GetType().Equals(typeof(BacklogVM)))
                {
                    addBehavior = ((BacklogVM)state).AddBehavior;
                    currentChildItems = ((BacklogVM)state).RootWorkItems;
                    setChildItems = (object s, IEnumerable<WorkItemVM> current, IEnumerable<WorkItemVM> additional) => ((BacklogVM)s).RootWorkItems = new ReadOnlyCollection<WorkItemVM>(current.Concat(additional).ToList());
                    isExpanded = true;
                }
                else
                {
                    addBehavior = ((WorkItemVM)state).AddBehavior;
                    currentChildItems = ((WorkItemVM)state).Children;
                    setChildItems = (object s, IEnumerable<WorkItemVM> current, IEnumerable<WorkItemVM> additional) => ((WorkItemVM)s).Children = new ReadOnlyCollection<WorkItemVM>(current.Concat(additional).ToList());
                }
                foreach (WorkItem innerWorkItem in (await loadWorkItems))
                {
                    item = WorkItemVM.Create(_backlogVM, innerWorkItem);
                    items.Add(item);
                    itemLoader = new WorkItemLoader(item);
                    addBehavior(itemLoader);
                    item.IsExpanded = isExpanded;
                    itemLoader.Load();
                    _ = Task.Run(() => LoadWorkItems(_backlogVM.Project.ProjectId, innerWorkItem.WorkItemId.Value))
                    .ContinueWith(LoadWorkItemsCallback, item, TaskScheduler.FromCurrentSynchronizationContext());
                }
                setChildItems(state, currentChildItems, items);
                _backlogVM.ReapplyFilter();
            }
            catch(System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
            finally
            {
                _backlogVM.CanRefresh = true;
                _backlogVM.BusyVisibility = Visibility.Collapsed;
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
                _ = Task.Run(() => LoadWorkItems(_backlogVM.Project.ProjectId))
                    .ContinueWith(LoadWorkItemsCallback, _backlogVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (System.Exception ex)
            {
                _backlogVM.BusyVisibility = Visibility.Collapsed;
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
                _backlogVM.BusyVisibility = Visibility.Collapsed;
                ErrorWindow.Open(ex, null);
            }
        }
    }
}
