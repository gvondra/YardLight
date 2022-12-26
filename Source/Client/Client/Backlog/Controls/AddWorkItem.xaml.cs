using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YardLight.Client.Backlog.Behaviors;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.Controls
{
    /// <summary>
    /// Interaction logic for AddWorkItem.xaml
    /// </summary>
    public partial class AddWorkItem : UserControl
    {
        public AddWorkItem()
        {
            InitializeComponent();
        }

        protected CreateWorkItemVM CreateWorkItemVM => (CreateWorkItemVM)DataContext;

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateWorkItemVM createWorkItemVM = CreateWorkItemVM;
            createWorkItemVM.CreateButtonText = "Stand by";
            createWorkItemVM.CreateWorkItemButtonEnabled = false;
            Task.Run(() => CreateWorkItem(createWorkItemVM))
                .ContinueWith(CreateWorkItemCallback, CreateWorkItemVM, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<WorkItem> CreateWorkItem(CreateWorkItemVM createWorkItemVM)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                WorkItemType workItemType = createWorkItemVM.SelectedNewItemType.InnerWorkItemType;
                WorkItemStatus workItemStatus = workItemType.Statuses
                    .OrderBy(s => s.Order)
                    .FirstOrDefault(s => s.IsActive ?? true);
                if (workItemStatus == null)
                    throw new ApplicationException($"Active status not found for type {workItemType.Title}. Verify that the type is configured with at least 1 active status.");
                WorkItem workItem = new WorkItem
                {
                    ParentWorkItemId = createWorkItemVM?.ParentWorkItem?.WorkItemId,
                    ProjectId = createWorkItemVM.BacklogVM.Project.ProjectId,
                    Title = createWorkItemVM.NewItemTitle,
                    Type = workItemType,
                    Status = workItemStatus
                };
                return workItemService.Create(settingsFactory.CreateApi(), workItem);
            }
        }

        private async Task CreateWorkItemCallback(Task<WorkItem> createWorkItem, object state)
        {
            CreateWorkItemVM createWorkItemVM = (CreateWorkItemVM)state;
            try
            {                
                WorkItem workItem = await createWorkItem;
                WorkItemVM workItemVM = WorkItemVM.Create(createWorkItemVM.BacklogVM, workItem);
                WorkItemVM parent = null;
                WorkItemLoader workItemLoader;
                if (workItemVM.ParentWorkItemId.HasValue)
                {
                    parent = Find(createWorkItemVM.BacklogVM.RootWorkItems, workItemVM.ParentWorkItemId.Value);
                }
                workItemLoader = new WorkItemLoader(workItemVM);
                if (parent == null)
                {
                    createWorkItemVM.BacklogVM.AppendWorkItem(workItemVM);                    
                    createWorkItemVM.BacklogVM.AddBehavior(workItemLoader);
                }
                else
                {
                    parent.AppendChild(workItemVM);
                    parent.AddBehavior(workItemLoader);
                    parent.IsExpanded = true;
                }
                workItemLoader.Load();
                createWorkItemVM.CreateWorkItemVisible = Visibility.Collapsed;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
            finally
            {
                createWorkItemVM.CreateButtonText = "Create";
                createWorkItemVM.NewItemTitle = string.Empty;
            }
        }

        private WorkItemVM Find(IEnumerable<WorkItemVM> workItems, Guid id)
        {
            WorkItemVM workItemVM = null;
            IEnumerator<WorkItemVM> enumerator = workItems.GetEnumerator();
            while (workItemVM == null && enumerator.MoveNext())
            {
                if (id.Equals(enumerator.Current.WorkItemId))
                    workItemVM = enumerator.Current;
                else 
                    workItemVM = Find(enumerator.Current.Children, id);
            }
            return workItemVM;
        }
    }
}
