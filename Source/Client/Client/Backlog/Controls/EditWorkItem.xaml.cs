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
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.Controls
{
    /// <summary>
    /// Interaction logic for EditWorkItem.xaml
    /// </summary>
    public partial class EditWorkItem : UserControl
    {
        public EditWorkItem()
        {
            InitializeComponent();
        }

        WorkItemVM WorkItemVM => (WorkItemVM)DataContext;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            WorkItemVM workItemVM = WorkItemVM;
            Task.Run(() => Update(workItemVM.InnerWorkItem))
                .ContinueWith(UpdateCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            if (workItemVM.CreateWorkIemCommentCommand != null 
                && workItemVM.CreateWorkIemCommentCommand.CanExecute(null)
                && !string.IsNullOrEmpty(workItemVM.NewCommentText))
            {
                workItemVM.CreateWorkIemCommentCommand.Execute(workItemVM);
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
                MessageBox.Show("Work Item Saved", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
