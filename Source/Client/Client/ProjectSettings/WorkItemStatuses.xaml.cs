using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YardLight.Client.ProjectSettings.ViewModel;
using YardLight.Interface;
using YardLight.Interface.Authorization;
using YardLight.Interface.Authorization.Models;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings
{
    /// <summary>
    /// Interaction logic for WorkItemStatuses.xaml
    /// </summary>
    public partial class WorkItemStatuses : Page
    {
        public WorkItemStatuses()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += WorkItemStatuses_Loaded;
        }

        public WorkItemStatusesVM WorkItemStatusesVM { get; set; }

        private void WorkItemStatuses_Loaded(object sender, RoutedEventArgs e)
        {
            WorkItemStatusesVM = new WorkItemStatusesVM();
            DataContext = WorkItemStatusesVM;
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            UserSession userSession = UserSessionLoader.GetUserSession();
            if (userSession?.OpenProjectId != null)
            {
                Task.Run(() => GetProject(userSession.OpenProjectId))
                    .ContinueWith(GetProjectCallback, userSession.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
                Task.Run(() => GetStatuses(userSession.OpenProjectId))
                    .ContinueWith(GetStatusesCallback, userSession.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task<List<WorkItemStatusVM>> GetStatuses(Guid? projectId)
        {
            if (projectId.HasValue)
            {
                using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
                {
                    ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                    IWorkItemStatusService statusService = scope.Resolve<IWorkItemStatusService>();
                    List<WorkItemStatusVM> statusVMs = (await statusService.GetByProjectId(settingsFactory.CreateApi(), projectId.Value))
                        .Select(s => new WorkItemStatusVM(s))
                        .ToList();
                    IUserService userService = scope.Resolve<IUserService>();
                    foreach (WorkItemStatusVM workItemStatusVM in statusVMs)
                    {
                        workItemStatusVM.CreateUserName = await userService.GetName(settingsFactory.CreateAuthorization(), workItemStatusVM.InnerStatus.CreateUserId.Value);
                        workItemStatusVM.UpdateUserName = await userService.GetName(settingsFactory.CreateAuthorization(), workItemStatusVM.InnerStatus.UpdateUserId.Value);
                    }
                    return statusVMs;
                }
            }
            else
            {
                return new List<WorkItemStatusVM>();
            }
        }

        private async Task GetStatusesCallback(Task<List<WorkItemStatusVM>> getStatuses, object state)
        {
            try
            {
                WorkItemStatusesVM.Statuses.Clear();
                WorkItemStatusesVM.SelectedStatus = null;
                foreach (WorkItemStatusVM statusVM in (await getStatuses).OrderBy(s => s.Order))
                {
                    if (state == null || !((Guid?)state).HasValue || ((Guid?)state).Value.Equals(statusVM.InnerStatus.ProjectId.Value))
                    {
                        WorkItemStatusesVM.Statuses.Add(statusVM);
                    }
                }
                if (WorkItemStatusesVM.Statuses.Count > 0)
                    WorkItemStatusesVM.SelectedStatus = WorkItemStatusesVM.Statuses[0];
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private Task<Models.Project> GetProject(Guid? projectId)
        {
            if (projectId.HasValue)
            {
                using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
                {
                    ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                    IProjectService projectService = scope.Resolve<IProjectService>();
                    return projectService.Get(settingsFactory.CreateApi(), projectId.Value);
                }
            }
            else
            {
                return Task.FromResult(default(Models.Project));
            }
        }

        private async Task GetProjectCallback(Task<Models.Project> getProject, object state)
        {
            try
            {
                Models.Project project = await getProject;
                if (project.ProjectId == (Guid)state)
                {
                    WorkItemStatusesVM.Project = project;
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private void AddHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (WorkItemStatusesVM.Project != null)
            {
                Task.Run(CreateStatusVM)
                    .ContinueWith(CreateStatusVMCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task<WorkItemStatusVM> CreateStatusVM()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                Models.WorkItemStatus status = new Models.WorkItemStatus
                {
                    Title = "New Status",
                    ColorCode = "DarkGray",
                    Order = (short)WorkItemStatusesVM.Statuses.Count,
                    ProjectId = WorkItemStatusesVM.Project.ProjectId
                };
                WorkItemStatusVM workItemStatusVM = new WorkItemStatusVM(status);
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IUserService userService = scope.Resolve<IUserService>();
                User user = await userService.Get(settingsFactory.CreateAuthorization());
                workItemStatusVM.CreateUserName = user.Name;
                workItemStatusVM.UpdateUserName = user.Name;
                return workItemStatusVM;
            }
        }

        private async Task CreateStatusVMCallback(Task<WorkItemStatusVM> createStatusVM, object state)
        {
            try
            {
                WorkItemStatusVM workItemStatusVM = await createStatusVM;
                WorkItemStatusesVM.Statuses.Add(workItemStatusVM);
                WorkItemStatusesVM.SelectedStatus = workItemStatusVM;
            }
            catch(System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
