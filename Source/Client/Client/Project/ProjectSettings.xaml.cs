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
using YardLight.Client.Project.ViewModel;
using YardLight.Interface;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.Project
{
    /// <summary>
    /// Interaction logic for ProjectSettings.xaml
    /// </summary>
    public partial class ProjectSettings : Page
    {
        public ProjectSettings()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            ProjectSettingsVM = new ProjectSettingsVM();
            DataContext = ProjectSettingsVM;
            InitializeComponent();
            this.Loaded += ProjectSettings_Loaded;
        }

        private ProjectSettingsVM ProjectSettingsVM { get; set; }

        private void ProjectSettings_Loaded(object sender, RoutedEventArgs e)
        {
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            UserSession userSession = UserSessionLoader.GetUserSession();
            if (userSession?.OpenProjectId != null)
            {
                Task.Run(() => GetProject(userSession.OpenProjectId))
                    .ContinueWith(GetProjectCallback, userSession.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
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
                    ProjectSettingsVM = new ProjectSettingsVM(project);
                    DataContext = ProjectSettingsVM;
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ProjectSettingsVM.HasErrors)
                {
                    ProjectSettingsVM.SaveButtonText = "Please wait";
                    ProjectSettingsVM.SaveButtonEnabled = false;
                    Task.Run(() => Save(ProjectSettingsVM.InnerProject))
                        .ContinueWith(SaveCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private Task Save(Models.Project project)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IProjectService projectService = scope.Resolve<IProjectService>();
                return projectService.Update(settingsFactory.CreateApi(), project);
            }
        }

        private async Task SaveCallback(Task save, object state)
        {
            try
            {
                await save;
                ProjectSettingsVM.SaveButtonText = "Save";
                ProjectSettingsVM.SaveButtonEnabled = true;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
