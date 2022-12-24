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
using YardLight.Client.Authorization.ViewModel;
using YardLight.Interface;
using InterfaceModels = YardLight.Interface.Models;

namespace YardLight.Client.Authorization
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class User : Page
    {
        public User()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += User_Loaded;
        }

        public FindUserVM UserVM { get; set; }

        private void User_Loaded(object sender, RoutedEventArgs e)
        {
            UserVM = new FindUserVM();
            DataContext = UserVM;
            GoogleLogin.ShowLoginDialog();
            Task.Run(GetAllRoles)
                .ContinueWith(GetAllRolesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<List<InterfaceModels.Role>> GetAllRoles()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IRoleService roleService = scope.Resolve<IRoleService>();
                return roleService.Get(settingsFactory.CreateApi());
            }
        }

        private async Task GetAllRolesCallback(Task<List<InterfaceModels.Role>> task, object state)
        {
            try
            {
                UserVM.AllRoles = await task;
                LoadUserRoles();
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private void LoadUserRoles()
        {
            UserVM.Roles.Clear();
            if (UserVM.User != null && UserVM.AllRoles != null)
            {
                List<string> activeRoles = (UserVM.User.Roles ?? new List<InterfaceModels.AppliedRole>())
                    .Select(r => r.PolicyName)
                    .ToList();
                foreach (InterfaceModels.Role role in UserVM.AllRoles)
                {
                    UserVM.Roles.Add(
                        new FindUserRoleVM(role)
                        {
                            IsActive = activeRoles.Any(r => string.Equals(r, role.PolicyName, StringComparison.OrdinalIgnoreCase))
                        });
                }
            }
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(UserVM.FindAddress))
                {
                    GoogleLogin.ShowLoginDialog();
                    Task.Run(() => GetUser(UserVM.FindAddress))
                        .ContinueWith(GetUserCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private async Task<InterfaceModels.User> GetUser(string emailAddress)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IUserService userService = scope.Resolve<IUserService>();
                return (await userService.Search(settingsFactory.CreateApi(), emailAddress)).FirstOrDefault();
            }
        }

        private async Task GetUserCallback(Task<InterfaceModels.User> task, object state)
        {
            try
            {
                UserVM.User = await task;
                LoadUserRoles();
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
                if (UserVM.User != null)
                {
                    UserVM.User.Roles = UserVM.Roles
                        .Where(r => r.IsActive)
                        .Select(r => new InterfaceModels.AppliedRole { Name = r.Name, PolicyName = r.PolicyName })
                        .ToList();
                    Task.Run(() => SaveUser(UserVM.User))
                        .ContinueWith(SaverUserCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private async Task<InterfaceModels.User> SaveUser(InterfaceModels.User user)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IUserService userService = scope.Resolve<IUserService>();
                return await userService.Update(settingsFactory.CreateApi(), user);
            }
        }

        private async Task SaverUserCallback(Task<InterfaceModels.User> task, object state)
        {
            try
            {
                InterfaceModels.User user = await task;
                UserVM.User = user;
                MessageBox.Show(Window.GetWindow(this), "Save Complete", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
