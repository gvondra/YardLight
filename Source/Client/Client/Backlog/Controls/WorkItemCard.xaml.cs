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
using YardLight.Interface.Models;
using YardLight.Interface;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog.Controls
{
    /// <summary>
    /// Interaction logic for WorkItemCard.xaml
    /// </summary>
    public partial class WorkItemCard : UserControl
    {
        public WorkItemCard()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            WorkItemVM workItem = (WorkItemVM)DataContext;
            WorkItemStatusVM workItemStatusVM = (WorkItemStatusVM)((MenuItem)sender).DataContext;
            if (!workItem.Status.WorkItemStatusId.Equals(workItemStatusVM.WorkItemStatusId))
            {
                workItem.Status = workItemStatusVM;
                Task.Run(() => Update(workItem.InnerWorkItem))
                    .ContinueWith(UpdateCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private Task<WorkItem> Update(WorkItem workItem)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                return workItemService.Update(settingsFactory.CreateApi(), workItem);
            }
        }

        private async Task UpdateCallback(Task<WorkItem> update, object state)
        {
            try
            {
                await update;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
