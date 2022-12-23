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
    /// Interaction logic for WorkItemTypes.xaml
    /// </summary>
    public partial class WorkItemTypes : Page
    {
        public WorkItemTypes()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += WorkItemTypes_Loaded;
        }

        public WorkItemTypesVM WorkItemTypesVM { get; set; }

        private void WorkItemTypes_Loaded(object sender, RoutedEventArgs e)
        {
            WorkItemTypesVM = new WorkItemTypesVM();
            DataContext = WorkItemTypesVM;
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            UserSession userSession = UserSessionLoader.GetUserSession();
            if (userSession?.OpenProjectId != null)
            {
                Task.Run(() => GetProject(userSession.OpenProjectId))
                    .ContinueWith(GetProjectCallback, userSession.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
                Task.Run(() => GetTypes(userSession.OpenProjectId, WorkItemTypesVM.ShowInactive))
                    .ContinueWith(GetTypesCallback, userSession.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task<List<WorkItemTypeVM>> GetTypes(Guid? projectId, bool showInactive)
        {
            if (projectId.HasValue)
            {
                using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
                {
                    ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                    IWorkItemTypeService typeService = scope.Resolve<IWorkItemTypeService>();
                    bool? isAtive = showInactive ? default(bool?) : true;
                    List<WorkItemTypeVM> typeVMs = (await typeService.GetByProjectId(settingsFactory.CreateApi(), projectId.Value, isAtive))
                        .Select(t => new WorkItemTypeVM(WorkItemTypesVM, t))
                        .ToList();
                    IUserService userService = scope.Resolve<IUserService>();
                    foreach (WorkItemTypeVM workItemTypeVM in typeVMs)
                    {
                        workItemTypeVM.CreateUserName = await userService.GetName(settingsFactory.CreateApi(), workItemTypeVM.InnerType.CreateUserId.Value);
                        workItemTypeVM.UpdateUserName = await userService.GetName(settingsFactory.CreateApi(), workItemTypeVM.InnerType.UpdateUserId.Value);                        
                        foreach (WorkItemStatusVM workItemStatusVM in workItemTypeVM.StatusesVM.Statuses)
                        {
                            workItemStatusVM.CreateUserName = await userService.GetName(settingsFactory.CreateApi(), workItemStatusVM.InnerStatus.CreateUserId.Value);
                            workItemStatusVM.UpdateUserName = await userService.GetName(settingsFactory.CreateApi(), workItemStatusVM.InnerStatus.UpdateUserId.Value);
                        }
                    }
                    return typeVMs;
                }
            }
            else
            {
                return new List<WorkItemTypeVM>();
            }
        }

        private async Task GetTypesCallback(Task<List<WorkItemTypeVM>> getTypes, object state)
        {
            try
            {
                WorkItemTypesVM.Types.Clear();
                WorkItemTypesVM.SelectedType = null;
                foreach (WorkItemTypeVM typeVM in (await getTypes))
                {
                    if (state == null || !((Guid?)state).HasValue || ((Guid?)state).Value.Equals(typeVM.InnerType.ProjectId.Value))
                    {
                        WorkItemTypesVM.Types.Add(typeVM);
                    }
                }
                if (WorkItemTypesVM.Types.Count > 0)
                    WorkItemTypesVM.SelectedType = WorkItemTypesVM.Types[0];
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
                    WorkItemTypesVM.Project = project;
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private void AddHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (WorkItemTypesVM.Project != null)
            {
                Task.Run(CreateTypeVM)
                    .ContinueWith(CreateTypeVMCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task<WorkItemTypeVM> CreateTypeVM()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                Models.WorkItemType type = new Models.WorkItemType
                {
                    Title = "New Type",
                    ColorCode = "DarkGray",
                    ProjectId = WorkItemTypesVM.Project.ProjectId,
                    Statuses = new List<Models.WorkItemStatus>()
                };
                WorkItemTypeVM workItemTypeVM = new WorkItemTypeVM(WorkItemTypesVM, type);
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IUserService userService = scope.Resolve<IUserService>();
                User user = await userService.Get(settingsFactory.CreateApi());
                workItemTypeVM.CreateUserName = user.Name;
                workItemTypeVM.UpdateUserName = user.Name;
                return workItemTypeVM;
            }
        }

        private async Task CreateTypeVMCallback(Task<WorkItemTypeVM> createTypeVM, object state)
        {
            try
            {
                WorkItemTypeVM workItemTypeVM = await createTypeVM;
                WorkItemTypesVM.Types.Add(workItemTypeVM);
                WorkItemTypesVM.SelectedType = workItemTypeVM;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private void ShowInactiveCheckBox_Click(object sender, RoutedEventArgs e)
        {
            WorkItemTypesVM.Types.Clear();
            Task.Run(() => GetTypes(WorkItemTypesVM.Project.ProjectId, WorkItemTypesVM.ShowInactive))
                .ContinueWith(GetTypesCallback, WorkItemTypesVM.Project.ProjectId, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
