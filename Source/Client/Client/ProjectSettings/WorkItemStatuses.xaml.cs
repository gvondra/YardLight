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

        private Task<List<Models.WorkItemStatus>> GetStatuses(Guid? projectId)
        {
            if (projectId.HasValue)
            {
                using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
                {
                    ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                    IWorkItemStatusService statusService = scope.Resolve<IWorkItemStatusService>();
                    return statusService.GetByProjectId(settingsFactory.CreateApi(), projectId.Value);
                }
            }
            else
            {
                return Task.FromResult(new List<Models.WorkItemStatus>());
            }
        }

        private async Task GetStatusesCallback(Task<List<Models.WorkItemStatus>> getStatuses, object state)
        {
            try
            {
                WorkItemStatusesVM.Statuses.Clear();
                WorkItemStatusesVM.SelectedStatus = null;
                foreach (Models.WorkItemStatus status in (await getStatuses).OrderBy(s => s.Order ?? 0))
                {
                    if (state == null || !((Guid?)state).HasValue || ((Guid?)state).Value.Equals(status.ProjectId.Value))
                    {
                        WorkItemStatusesVM.Statuses.Add(new WorkItemStatusVM(status));
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
                Models.WorkItemStatus status = new Models.WorkItemStatus
                {
                    Title = "New Status",
                    ColorCode = "DarkGray",
                    Order = (short)WorkItemStatusesVM.Statuses.Count,
                    ProjectId = WorkItemStatusesVM.Project.ProjectId
                };
                WorkItemStatusesVM.SelectedStatus = new WorkItemStatusVM(status);
                WorkItemStatusesVM.Statuses.Add(WorkItemStatusesVM.SelectedStatus);
            }
        }
    }
}
