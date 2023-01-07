using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YardLight.Client.ProjectSettings.ViewModel;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class SaveItterationCommand : ICommand
    {
        private readonly ItterationVM _itterationVM;
        private bool _canExecute = true;

        public SaveItterationCommand(ItterationVM itterationVM)
        {
            _itterationVM = itterationVM;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            _canExecute = false;
            CanExecuteChanged.Invoke(this, new EventArgs());
            Task.Run(() => Save(_itterationVM))
                .ContinueWith(SaveCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task<Itteration> Save(ItterationVM itterationVM)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IItterationService itterationService = scope.Resolve<IItterationService>();
                return await itterationService.Save(settingsFactory.CreateApi(), itterationVM.InnerItteration);
            }            
        }

        private async Task SaveCallback(Task<Itteration> save, object state)
        {
            try
            {
                Itteration itteration = await save;
                _itterationVM.Virtual = itteration.Virtual ?? false;
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
