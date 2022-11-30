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
using YardLight.Interface.Authorization;
using YardLight.Interface;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.Controls
{
    /// <summary>
    /// Interaction logic for WorkItemType.xaml
    /// </summary>
    public partial class WorkItemType : UserControl
    {
        public WorkItemType()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            WorkItemTypeVM workItemTypeVM = (WorkItemTypeVM)DataContext;
            workItemTypeVM.SaveButtonEnabled = false;
            Task.Run(() => Save(workItemTypeVM))
                .ContinueWith(SaveCallback, workItemTypeVM, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task<WorkItemTypeVM> Save(WorkItemTypeVM workItemTypeVM)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                Models.WorkItemType workItemType;
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemTypeService workItemTypeService = scope.Resolve<IWorkItemTypeService>();
                if (workItemTypeVM.InnerType.WorkItemTypeId.HasValue)
                    workItemType = await workItemTypeService.Update(settingsFactory.CreateApi(), workItemTypeVM.InnerType);
                else
                    workItemType = await workItemTypeService.Create(settingsFactory.CreateApi(), workItemTypeVM.InnerType);
                workItemTypeVM = new WorkItemTypeVM(workItemTypeVM.WorkItemTypesVM, workItemType);
                IUserService userService = scope.Resolve<IUserService>();
                workItemTypeVM.CreateUserName = await userService.GetName(settingsFactory.CreateAuthorization(), workItemType.CreateUserId.Value);
                workItemTypeVM.UpdateUserName = await userService.GetName(settingsFactory.CreateAuthorization(), workItemType.UpdateUserId.Value);
                foreach (WorkItemStatusVM workItemStatusVM in workItemTypeVM.StatusesVM.Statuses)
                {
                    workItemStatusVM.CreateUserName = await userService.GetName(settingsFactory.CreateAuthorization(), workItemStatusVM.InnerStatus.CreateUserId.Value);
                    workItemStatusVM.UpdateUserName = await userService.GetName(settingsFactory.CreateAuthorization(), workItemStatusVM.InnerStatus.UpdateUserId.Value);
                }
                return workItemTypeVM;
            }
        }

        private async Task SaveCallback(Task<WorkItemTypeVM> save, object state)
        {
            WorkItemTypeVM workItemTypeVM = (WorkItemTypeVM)state;
            WorkItemTypesVM workItemTypesVM = workItemTypeVM.WorkItemTypesVM;
            try
            {
                WorkItemTypeVM savedTypeVM = await save;
                int i = workItemTypesVM.Types.IndexOf(workItemTypeVM);
                workItemTypesVM.Types[i] = savedTypeVM;
                workItemTypesVM.SelectedType = savedTypeVM;
                MessageBox.Show("Type Saved", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
            finally
            {
                workItemTypeVM.SaveButtonEnabled = true;
            }
        }
    }
}
