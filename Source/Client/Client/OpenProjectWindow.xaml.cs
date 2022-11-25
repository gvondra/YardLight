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
    /// Interaction logic for OpenProject.xaml
    /// </summary>
    public partial class OpenProjectWindow : Window
    {
        public OpenProjectWindow()
        {
            InitializeComponent();
            this.Loaded += OpenProjectWindow_Loaded;
        }

        private OpenProjectWindowVM OpenProjectWindowVM { get; set; }

        private void OpenProjectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenProjectWindowVM = new OpenProjectWindowVM()
                {
                    OpenButtonEnabled = false
                };
                DataContext = OpenProjectWindowVM;                
                Task.Run(GetProjects)
                    .ContinueWith(GetProjectsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, this);
            }
        }

        private async Task<IEnumerable<Project>> GetProjects()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();   
                IProjectService projectService = scope.Resolve<IProjectService>();
                return await projectService.Get(settingsFactory.CreateApi());
            }
        }

        private async Task GetProjectsCallback(Task<IEnumerable<Project>> getProjects, object state)
        {
            try
            {
                UserSession userSession = UserSessionLoader.GetUserSession();
                int selectedIndex = 0;
                OpenProjectWindowVM.SelectedProjectIndex = -1;
                OpenProjectWindowVM.ProjectIds.Clear();
                OpenProjectWindowVM.Projects.Clear();
                int i = 0;
                foreach (Project project in await getProjects)
                {
                    OpenProjectWindowVM.ProjectIds.Add(project.ProjectId);
                    OpenProjectWindowVM.Projects.Add(project.Title);
                    if (userSession != null 
                        && userSession.OpenProjectId.HasValue
                        && userSession.OpenProjectId.Value == project.ProjectId)
                    {
                        selectedIndex = i;
                    }
                    i += 1;
                }
                if (OpenProjectWindowVM.Projects.Count > 0)
                    OpenProjectWindowVM.SelectedProjectIndex = selectedIndex;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, this);
            }
            finally
            {
                OpenProjectWindowVM.OpenButtonEnabled = true;
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserSession userSession = UserSessionLoader.GetUserSession();
                if (OpenProjectWindowVM.SelectedProjectIndex < 0)
                    userSession.OpenProjectId = null;
                else 
                    userSession.OpenProjectId = OpenProjectWindowVM.ProjectIds[OpenProjectWindowVM.SelectedProjectIndex];
                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, this);
            }
        }
    }
}
