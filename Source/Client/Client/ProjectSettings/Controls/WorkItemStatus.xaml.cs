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
using YardLight.Interface.Models;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.Controls
{
    /// <summary>
    /// Interaction logic for WorkItemStatus.xaml
    /// </summary>
    public partial class WorkItemStatus : UserControl
    {
        public WorkItemStatus()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            WorkItemStatusVM workItemStatusVM = (WorkItemStatusVM)DataContext;
            workItemStatusVM.SaveButtonEnabled = false;
            Task.Run(() => Save(workItemStatusVM))
                .ContinueWith(SaveCallback, workItemStatusVM, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<Models.WorkItemStatus> Save(WorkItemStatusVM workItemStatusVM)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemStatusService workItemStatusService = scope.Resolve<IWorkItemStatusService>();
                if (workItemStatusVM.InnerStatus.WorkItemStatusId.HasValue)
                    return workItemStatusService.Update(settingsFactory.CreateApi(), workItemStatusVM.InnerStatus);
                else
                    return workItemStatusService.Create(settingsFactory.CreateApi(), workItemStatusVM.InnerStatus);
            }
        }

        private async Task SaveCallback(Task<Models.WorkItemStatus> save, object state)
        {
            WorkItemStatusVM workItemStatusVM = (WorkItemStatusVM)state;
            try
            {
                Models.WorkItemStatus workItemStatus = await save;                   
                workItemStatusVM.SetInnerWorkItemStatus(workItemStatus);
                MessageBox.Show("Status Saved", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
            finally
            {
                workItemStatusVM.SaveButtonEnabled = true;
            }
        }
    }
}
