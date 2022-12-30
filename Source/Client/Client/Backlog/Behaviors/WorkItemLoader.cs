using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.Behaviors
{
    public class WorkItemLoader
    {
        private readonly WorkItemVM _workItemVM;

        public WorkItemLoader(WorkItemVM workItemVM)
        {
            _workItemVM = workItemVM;
            SetBulletColor();
        }

        private void SetBulletColor()
        {            
            BrushConverter brushConverter = new BrushConverter();
            try
            {
                if (!string.IsNullOrEmpty(_workItemVM.ColorCode))
                    _workItemVM.BulletColor = (Brush)brushConverter.ConvertFromString(_workItemVM.ColorCode);
                else
                    _workItemVM.BulletColor = Brushes.Black;
            }
            catch
            {
                _workItemVM.BulletColor = Brushes.Black;
            }
        }

        public void Load()
        {
            UserSession userSession = UserSessionLoader.GetUserSession();
            Task.Run(() => LoadItterations(userSession.OpenProjectId))
                .ContinueWith(LoadItterationsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            Task.Run(() => LoadTeams(userSession.OpenProjectId))
                .ContinueWith(LoadTeamsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            if (_workItemVM.LoadWorkItemCommentCommand == null)
                _workItemVM.LoadWorkItemCommentCommand = new LoadWorkItemCommentCommand(this);
            if (_workItemVM.CreateWorkIemCommentCommand == null)
                _workItemVM.CreateWorkIemCommentCommand = new CreateWorkIemCommentCommand();
            if (_workItemVM.WorkItemEditCommand == null)
                _workItemVM.WorkItemEditCommand = new WorkItemEditCommand();

        }

        private Task<List<string>> LoadItterations(Guid? projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                if (projectId.HasValue)
                    return workItemService.GetItterationsByProjectId(settingsFactory.CreateApi(), projectId.Value);
                else
                    return Task.FromResult(new List<string>());
            }
        }

        private async Task LoadItterationsCallback(Task<List<string>> loadItterations, object state)
        {
            try
            {
                _workItemVM.Itterations = await loadItterations;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }

        private Task<List<string>> LoadTeams(Guid? projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                if (projectId.HasValue)
                    return workItemService.GetTeamsByProjectId(settingsFactory.CreateApi(), projectId.Value);
                else
                    return Task.FromResult(new List<string>());
            }
        }

        private async Task LoadTeamsCallback(Task<List<string>> loadTeams, object state)
        {
            try
            {
                _workItemVM.Teams = await loadTeams;
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }

        public void LoadComments()
        {
            Guid? projectId = _workItemVM.ProjectId;
            Guid? workItemId = _workItemVM.WorkItemId;
            if (projectId.HasValue && workItemId.HasValue)
            {
                Task.Run(() => LoadComments(projectId.Value, workItemId.Value))
                    .ContinueWith(LoadCommentsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());

            }
        }

        private async Task<List<CommentVM>> LoadComments(Guid projectId, Guid workItemId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemCommentService workItemCommentService = scope.Resolve<IWorkItemCommentService>();
                IUserService userService = scope.Resolve<IUserService>();
                List<CommentVM> comments = (await workItemCommentService.GetByWorkItemId(settingsFactory.CreateApi(), projectId, workItemId))
                    .Select(c => new CommentVM(c))
                    .ToList();
                foreach (CommentVM comment in comments)
                {
                    comment.CreateUser = await userService.GetName(settingsFactory.CreateApi(), comment.CreateUserId.Value);
                }
                return comments;
            }
        }

        private async Task LoadCommentsCallback(Task<List<CommentVM>> loadComments, object state)
        {
            try
            {
                _workItemVM.Comments.Clear();
                foreach (CommentVM comment in await loadComments)
                {
                    _workItemVM.Comments.Add(comment);
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }
    }
}
