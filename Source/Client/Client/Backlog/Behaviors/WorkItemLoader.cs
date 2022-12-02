using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface;

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
    }
}
