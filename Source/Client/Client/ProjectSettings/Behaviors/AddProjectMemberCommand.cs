using Autofac;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using YardLight.Client.ProjectSettings.ViewModel;
using YardLight.Interface;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class AddProjectMemberCommand : ICommand
    {
        private readonly MembersVM _membersVM;
        private bool _canExecute = true;

        public AddProjectMemberCommand(MembersVM membersVM)
        {
            _membersVM = membersVM;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (_membersVM.ProjectId.HasValue && !string.IsNullOrEmpty(_membersVM.AddMemberEmailAddress))
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() => AddProjectUser(_membersVM.ProjectId.Value, _membersVM.AddMemberEmailAddress))
                    .ContinueWith(AddProjectUserCallback, _membersVM.AddMemberEmailAddress, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        
        private Task AddProjectUser(Guid projectId, string emailAddress)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IProjectService projectService = scope.Resolve<IProjectService>();
                return projectService.AddUser(settingsFactory.CreateApi(), projectId, emailAddress);
            }
        }

        private async Task AddProjectUserCallback(Task addProjectUser, object state)
        {
            try
            {
                await addProjectUser;
                _membersVM.AddMemberEmailAddress = string.Empty;
                if (state != null)
                    _membersVM.Members.Add(state.ToString());
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
