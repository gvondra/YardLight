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
using YardLight.Interface.Authorization;
using YardLight.Interface.Authorization.Models;

namespace YardLight.Client.Authorization
{
    /// <summary>
    /// Interaction logic for Roles.xaml
    /// </summary>
    public partial class Roles : Page
    {
        public Roles()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += Roles_Loaded;
        }

        private RolesVM RolesVM { get; set; }

        private void Roles_Loaded(object sender, RoutedEventArgs e)
        {
            RolesVM = new RolesVM();
            DataContext = RolesVM;
            GoogleLogin.ShowLoginDialog();
            Task.Run(GetRoles)
                .ContinueWith(GetRolesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<IEnumerable<Role>> GetRoles()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                IRoleService roleService = scope.Resolve<IRoleService>();
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                return roleService.GetAll(settingsFactory.CreateAuthorization());
            }
        }

        private async Task GetRolesCallback(Task<IEnumerable<Role>> getRoles, object state)
        {
            try
            {
                RolesVM.Roles.Clear();
                foreach (Role role in await getRoles)
                {
                    RolesVM.Roles.Add(new RoleVM(role));
                }
                if (RolesVM.Roles.Count == 0)
                {
                    RolesVM.Roles.Add(CreateNewRoleVM());                    
                }
                RolesVM.SelectedRole = RolesVM.Roles[0];
            }
            catch(Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems != null && e.AddedItems.Count == 1)
                {
                    RolesVM.SelectedRole = (RoleVM)e.AddedItems[0];
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private void AddHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RoleVM role = CreateNewRoleVM();
                RolesVM.Roles.Add(role);
                RolesVM.SelectedRole = role;
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private RoleVM CreateNewRoleVM()
            => new RoleVM
            {
                Name = "New Role",
                PolicyName = "Context:Access",
                IsNew = true
            };

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RolesVM.SelectedRole != null)
                {
                    if (RolesVM.SelectedRole.IsNew)
                        Task.Run(() => CreateRole(RolesVM.SelectedRole))
                            .ContinueWith(SaveRoleCallback, RolesVM.SelectedRole, TaskScheduler.FromCurrentSynchronizationContext());
                    else
                        Task.Run(() => UpdateRole(RolesVM.SelectedRole))
                            .ContinueWith(SaveRoleCallback, RolesVM.SelectedRole, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private Task<Role> CreateRole(RoleVM roleVM)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                IRoleService roleService = scope.Resolve<IRoleService>();
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                return roleService.Create(settingsFactory.CreateAuthorization(), roleVM.InnerRole);
            }
        }

        private Task<Role> UpdateRole(RoleVM roleVM)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                IRoleService roleService = scope.Resolve<IRoleService>();
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                return roleService.Update(settingsFactory.CreateAuthorization(), roleVM.InnerRole);
            }
        }

        private async Task SaveRoleCallback(Task<Role> saveRole, object roleVM)
        {
            try
            {
                int index = RolesVM.Roles.IndexOf((RoleVM)roleVM);
                RolesVM.Roles[index] = new RoleVM(await saveRole);
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
