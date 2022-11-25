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
using System.Windows.Shapes;
using YardLight.Client.ViewModel;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public CreateProjectWindow()
        {
            InitializeComponent();
            this.Loaded += CreateProjectWindow_Loaded;
        }

        public CreateProjectWindowVM CreateProjectWindowVM { get; set; }

        private void CreateProjectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CreateProjectWindowVM = new CreateProjectWindowVM() { Title = "New Project" };
            DataContext= CreateProjectWindowVM;
            GoogleLogin.ShowLoginDialog();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CreateProjectWindowVM.CanCreate)
                {
                    Project project = new Project
                    {
                        Title = CreateProjectWindowVM.Title
                    };
                    Task.Run(() => CreateProject(project))
                        .ContinueWith(CreateProjectCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, this);
            }
        }

        private Task<Project> CreateProject(Project project)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IProjectService projectService = scope.Resolve<IProjectService>();
                return projectService.Create(settingsFactory.CreateApi(), project);
            }
        }

        private async Task CreateProjectCallback(Task<Project> createProject, object state)
        {
            try
            {
                Project project = await createProject;
                UserSession userSession = UserSessionLoader.GetUserSession();
                userSession.OpenProjectId = project.ProjectId;
                DialogResult = true;
                this.Close();
                MessageBox.Show(Window.GetWindow(this), "Project Created", project.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, this);
            }
        }
    }
}
