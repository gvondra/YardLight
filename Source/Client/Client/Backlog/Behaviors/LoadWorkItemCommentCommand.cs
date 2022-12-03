using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog.Behaviors
{
    public class LoadWorkItemCommentCommand : ICommand
    {
        private readonly WorkItemLoader _workItemLoader;

        public LoadWorkItemCommentCommand(WorkItemLoader workItemLoader)
        {
            _workItemLoader = workItemLoader;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _workItemLoader.LoadComments();
        }
    }
}
