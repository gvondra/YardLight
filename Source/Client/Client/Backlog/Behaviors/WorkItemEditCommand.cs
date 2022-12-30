using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog.Behaviors
{
    public class WorkItemEditCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            NavigationService navigationService = NavigationService.GetNavigationService((DependencyObject)parameter);
            WorkItemVM workItemVM = (WorkItemVM)((dynamic)parameter).DataContext;
            EditWorkItem editWorkItem = new EditWorkItem()
            {
                DataContext = workItemVM
            };
            navigationService.Navigate(editWorkItem);
        }
    }
}
