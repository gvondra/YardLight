using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface.Models;
using YardLight.Interface;

namespace YardLight.Client.Backlog.Behaviors
{
    public class BoardVMLoader
    {
        private readonly BoardVM _boardVM;

        public BoardVMLoader(BoardVM boardVM)
        {
            _boardVM = boardVM;
        }

        public void Load()
        {
            _boardVM.BusyVisibility = Visibility.Visible;
            UserSession userSession = UserSessionLoader.GetUserSession();
            Task.Run(() => LoadProject(userSession.OpenProjectId.Value))
                .ContinueWith(LoadProjectCallback, userSession.OpenProjectId.Value, TaskScheduler.FromCurrentSynchronizationContext());
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
                ((WorkItemFilterVM)state).Itterations = await loadItterations;
                _boardVM.Filter = (WorkItemFilterVM)state;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
            finally
            {
                _boardVM.BusyVisibility = Visibility.Collapsed;
            }
        }

        private Task<List<WorkItemType>> LoadWorkItemTypes(Guid? projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemTypeService workItemTypeService = scope.Resolve<IWorkItemTypeService>();
                if (projectId.HasValue)
                    return workItemTypeService.GetByProjectId(settingsFactory.CreateApi(), projectId.Value);
                else
                    return Task.FromResult(new List<WorkItemType>());
            }
        }

        private async Task LoadWorkItemTypesCallback(Task<List<WorkItemType>> loadWorkItemTypes, object state)
        {
            try
            {
                WorkItemFilterVM workItemFilterVM = (WorkItemFilterVM)state;
                workItemFilterVM.WorkItemTypes = (await loadWorkItemTypes) ?? new List<WorkItemType>();
                if (workItemFilterVM.WorkItemTypeId.HasValue && workItemFilterVM.WorkItemTypes.Count > 0)
                    workItemFilterVM.WorkItemType = workItemFilterVM.WorkItemTypes.FirstOrDefault(t => t.WorkItemTypeId.Equals(workItemFilterVM.WorkItemTypeId.Value));
                else
                    workItemFilterVM.WorkItemType = null;
                if (workItemFilterVM.WorkItemType == null && workItemFilterVM.WorkItemTypes.Count > 0)
                    workItemFilterVM.WorkItemType = workItemFilterVM.WorkItemTypes[0];

                UserSession userSession = UserSessionLoader.GetUserSession();
                _ = Task.Run(() => LoadTeams(userSession.OpenProjectId))
                    .ContinueWith(LoadTeamsCallback, state, TaskScheduler.FromCurrentSynchronizationContext());
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
                ((WorkItemFilterVM)state).Teams = await loadTeams;
                UserSession userSession = UserSessionLoader.GetUserSession();
                _ = Task.Run(() => LoadItterations(userSession.OpenProjectId))
                .ContinueWith(LoadItterationsCallback, state, TaskScheduler.FromCurrentSynchronizationContext());
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
                    _boardVM.Project = project;
                }
                // loading the filter is a 3 step process: load types, load teams, load itteration. Then assign the fileter back to the board vm
                UserSession userSession = UserSessionLoader.GetUserSession();
                _ = Task.Run(() => LoadWorkItemTypes(userSession.OpenProjectId))
                    .ContinueWith(LoadWorkItemTypesCallback, new WorkItemFilterVM(userSession), TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }
    }
}
