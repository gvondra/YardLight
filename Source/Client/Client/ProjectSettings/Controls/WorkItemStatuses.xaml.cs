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

namespace YardLight.Client.ProjectSettings.Controls
{
    /// <summary>
    /// Interaction logic for WorkItemStatuses.xaml
    /// </summary>
    public partial class WorkItemStatuses : UserControl
    {
        public WorkItemStatuses()
        {
            InitializeComponent();
        }

        public WorkItemStatusesVM WorkItemStatusesVM => (WorkItemStatusesVM)DataContext;

        private void AddHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(Create)
                .ContinueWith(CreateCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task<WorkItemStatusVM> Create()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                UserSession userSession = UserSessionLoader.GetUserSession();
                Models.WorkItemStatus workItemStatus = new Models.WorkItemStatus
                {
                    Title = "New Status",
                    ColorCode = "DarkGray",
                    IsActive = true,
                    ProjectId = userSession.OpenProjectId
                };
                WorkItemStatusVM workItemStatusVM = new WorkItemStatusVM(workItemStatus);
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IUserService userService = scope.Resolve<IUserService>();
                Models.User user = await userService.Get(settingsFactory.CreateApi());
                workItemStatusVM.CreateUserName = user.Name;
                workItemStatusVM.UpdateUserName = user.Name;
                return workItemStatusVM;
            }
        }

        private async Task CreateCallback(Task<WorkItemStatusVM> create, object state)
        {
            try
            {
                WorkItemStatusVM workItemStatusVM = await create;
                workItemStatusVM.Order = (short)WorkItemStatusesVM.Statuses.Count;
                WorkItemStatusesVM.Statuses.Add(workItemStatusVM);
                WorkItemStatusesVM.SelectedStatus = workItemStatusVM;
                WorkItemStatusesVM.WorkItemTypeVM.InnerType.Statuses.Add(workItemStatusVM.InnerStatus);
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
