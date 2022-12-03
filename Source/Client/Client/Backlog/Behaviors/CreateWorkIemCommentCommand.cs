using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.Behaviors
{
    public class CreateWorkIemCommentCommand : ICommand
    {
        private bool _canExecute = true;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (!typeof(WorkItemVM).IsAssignableFrom(parameter.GetType()))
                throw new ArgumentException("Input must be of type " + nameof(WorkItemVM));
            WorkItemVM workItemVM = (WorkItemVM)parameter;
            if (!string.IsNullOrEmpty(workItemVM.NewCommentText))
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() => CreateComment(workItemVM.ProjectId.Value, workItemVM.WorkItemId.Value, workItemVM.NewCommentText))
                    .ContinueWith(CreateCommentCallback, workItemVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private Task<Comment> CreateComment(Guid projectId, Guid workItemId, string commentText)
        {
            using(ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemCommentService workItemCommentService = scope.Resolve<IWorkItemCommentService>();
                Comment comment = new Comment
                {
                    Text = commentText
                };
                return workItemCommentService.Create(settingsFactory.CreateApi(), projectId, workItemId, comment);
            }
        }

        private async Task CreateCommentCallback(Task<Comment> createComment, object state)
        {
            try
            {
                await createComment;
                WorkItemVM workItemVM = (WorkItemVM)state;
                workItemVM.NewCommentText = string.Empty;
                if (workItemVM.LoadWorkItemCommentCommand != null && workItemVM.LoadWorkItemCommentCommand.CanExecute(null))
                    workItemVM.LoadWorkItemCommentCommand.Execute(null);
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
            finally
            {
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }
    }
}
