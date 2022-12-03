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
using YardLight.Client.Backlog.Behaviors;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog
{
    /// <summary>
    /// Interaction logic for Backlog.xaml
    /// </summary>
    public partial class Backlog : Page
    {
        private BacklogVMLoader _backlogVMLoader;

        public Backlog()
        {            
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += Backlog_Loaded;
        }

        public BacklogVM BacklogVM { get; set; }

        private void Backlog_Loaded(object sender, RoutedEventArgs e)
        {
            if (BacklogVM == null || DataContext == null)
            {
                BacklogVM = new BacklogVM();
                BacklogVM.AddBehavior(new WorkItemFilter(BacklogVM));
                DataContext = BacklogVM;
            }
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            if (_backlogVMLoader == null)
            {
                _backlogVMLoader = new BacklogVMLoader(BacklogVM);
                _backlogVMLoader.Load();
            }
        }

        private void AddChildHyperlink_Click(object sender, RoutedEventArgs e)
        {
            WorkItemVM workItemVM = (WorkItemVM)((dynamic)e.Source).DataContext;
            workItemVM.CreateWorkItemVM.CreateWorkItemVisible = workItemVM.CreateWorkItemVM.CreateWorkItemVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            WorkItemVM workItemVM = (WorkItemVM)((dynamic)e.Source).DataContext;
            EditWorkItem editWorkItem = new EditWorkItem()
            {
                DataContext = workItemVM
            };
            navigationService.Navigate(editWorkItem);
            if (workItemVM.LoadWorkItemCommentCommand != null && workItemVM.LoadWorkItemCommentCommand.CanExecute(null))
                workItemVM.LoadWorkItemCommentCommand.Execute(null);
        }
    }
}
