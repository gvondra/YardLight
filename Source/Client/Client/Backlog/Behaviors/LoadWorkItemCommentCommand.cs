using System;
using System.Windows.Input;

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
