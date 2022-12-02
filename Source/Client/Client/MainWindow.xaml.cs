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
using YardLight.Client.ViewModel;
using YardLight.Interface;
using Models = YardLight.Interface.Models;

namespace YardLight.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainWindowVM = new MainWindowVM();
            DataContext = MainWindowVM;
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private MainWindowVM MainWindowVM { get; set; }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UserSession userSession = UserSessionLoader.GetUserSession();
            MainWindowVM.ShowProjectSettings = BoolToVisibility(userSession?.OpenProjectId != null);
            MainWindowVM.ShowProject = BoolToVisibility(userSession?.OpenProjectId != null);
            //if (userSession?.OpenProjectId != null)
            //{
            //    GoogleLogin.ShowLoginDialog(true, this);
            //    Task.Run(() => GetProject(userSession?.OpenProjectId))
            //        .ContinueWith(GetProjectCallback, userSession?.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
            //}
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void GoToPageCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NavigationService navigationService = navigationFrame.NavigationService;
            JournalEntry journalEntry = navigationService.RemoveBackEntry();
            while (journalEntry != null)
                journalEntry = navigationService.RemoveBackEntry();
            //NavigationService navigationService = NavigationService.GetNavigationService(navigationFrame);
            navigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
        }

        private void GoogleLoginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GoogleLogin.ShowLoginDialog(checkAccessToken: false, owner: this);
        }

        public void AfterTokenRefresh()
        {
            MainWindowVM.ShowUserRole = BoolToVisibility(AccessToken.UserHasUserAdminRoleAccess());
            MainWindowVM.ShowLogs = BoolToVisibility(AccessToken.UserHasLogReadAccess());
        }

        private Visibility BoolToVisibility(bool value) => value ? Visibility.Visible : Visibility.Collapsed;

        private void CreateProjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GoogleLogin.ShowLoginDialog(owner: this);
            CreateProjectWindow window= new CreateProjectWindow() { Owner = this };
            if (window.ShowDialog() ?? false)
            {
                AfterChangeProject();
            }
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GoogleLogin.ShowLoginDialog(owner: this);
            Window window = new OpenProjectWindow() { Owner = this };
            if (window.ShowDialog() ?? false)
            {
                AfterChangeProject();
            }
        }

        private void AfterChangeProject()
        {
            NavigationService navigationService = navigationFrame.NavigationService;
            navigationService.Navigate(new Uri("NavigationPage/Home.xaml", UriKind.Relative));
            UserSession userSession = UserSessionLoader.GetUserSession();
            MainWindowVM.ShowProjectSettings = BoolToVisibility(userSession?.OpenProjectId != null);
            MainWindowVM.ShowProject = BoolToVisibility(userSession?.OpenProjectId != null);
            //Task.Run(() => GetProject(userSession?.OpenProjectId))
            //    .ContinueWith(GetProjectCallback, userSession?.OpenProjectId, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //private Task<Models.Project> GetProject(Guid? projectId)
        //{
        //    if (projectId.HasValue)
        //    {
        //        using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
        //        {
        //            ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
        //            IProjectService projectService = scope.Resolve<IProjectService>();
        //            return projectService.Get(settingsFactory.CreateApi(), projectId.Value);
        //        }
        //    }
        //    else
        //    {
        //        return Task.FromResult(default(Models.Project));
        //    }
        //}

        //private async Task GetProjectCallback(Task<Models.Project> getProject, object state)
        //{
        //    try
        //    {
        //        if (state != null)
        //        {
        //            Models.Project project = await getProject;
        //            if (project == null || project.ProjectId == (Guid)state)
        //                MainWindowVM.Project = project;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ErrorWindow.Open(ex, this);
        //    }
        //}
    }
}
