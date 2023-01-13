using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YardLight.Client.ProjectSettings.ViewModel;
using YardLight.Interface;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class RemoveProjectMemberCommand : ICommand
    {
        private readonly MembersVM _membersVm;
        private bool _canExecute = true;

        public RemoveProjectMemberCommand(MembersVM membersVM)
        {
            _membersVm = membersVM;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (_membersVm.ProjectId.HasValue && !string.IsNullOrEmpty(parameter?.ToString()))
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() => RemoveProjectUser(_membersVm.ProjectId.Value, (string)parameter))
                    .ContinueWith(RemoveProjectUserCallback, parameter, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void RemoveProjectUser(Guid projectId, string emailAddress)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IProjectService projectService = scope.Resolve<IProjectService>();
                projectService.RemoveUser(settingsFactory.CreateApi(), projectId, emailAddress).Wait();
            }
        }

        private async Task RemoveProjectUserCallback(Task removeProjectUser, object state)
        {
            try
            {
                await removeProjectUser;
                if (state != null)
                {
                    _membersVm.Members.Remove(state.ToString());
                }
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
