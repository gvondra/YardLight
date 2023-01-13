using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Client.ProjectSettings.ViewModel;
using YardLight.Interface;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class MembersVMLoader
    {
        private readonly MembersVM _membersVM;

        public MembersVMLoader(MembersVM membersVM)
        {
            _membersVM = membersVM;
        }

        public void Load()
        {
            UserSession userSession = UserSessionLoader.GetUserSession();
            if (userSession.OpenProjectId.HasValue)
            {
                _membersVM.ProjectId = userSession.OpenProjectId.Value;
                _membersVM.BusyVisibility = Visibility.Visible;
                Task.Run(() => GetProjectUsers(userSession.OpenProjectId.Value))
                    .ContinueWith(GetProjectUsersCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private string[] GetProjectUsers(Guid projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IProjectService projectService = scope.Resolve<IProjectService>();
                return projectService.GetUsers(settingsFactory.CreateApi(), projectId).Result;
            }
        }

        private async Task GetProjectUsersCallback(Task<string[]> getProjectUsers, object state)
        {
            try
            {
                _membersVM.Members.Clear();
                foreach (string address in await getProjectUsers)
                {
                    _membersVM.Members.Add(address);
                }                
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
            finally
            {
                _membersVM.BusyVisibility = Visibility.Collapsed;
            }
        }
    }
}
