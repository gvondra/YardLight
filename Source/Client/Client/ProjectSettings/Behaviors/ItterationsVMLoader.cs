using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Client.Behaviors;
using YardLight.Client.ProjectSettings.ViewModel;
using YardLight.Interface;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class ItterationsVMLoader
    {
        private readonly ItterationsVM _itterationsVM;

        public ItterationsVMLoader(ItterationsVM itterationsVM)
        {
            _itterationsVM = itterationsVM;
            _itterationsVM.PropertyChanged += ItterationsVM_PropertyChanged;
        }

        private void ItterationsVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case (nameof(ItterationsVM.ShowHidden)):
                    Load();
                    break;
            }
        }

        public void Load()
        {
            UserSession userSession = UserSessionLoader.GetUserSession();
            if (userSession.OpenProjectId.HasValue)
            {
                _itterationsVM.BusyVisibility = Visibility.Visible;
                _itterationsVM.Itterations.Clear();
                Task.Run(() => GetItterations(userSession.OpenProjectId.Value))
                    .ContinueWith(GetItterationsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private Task<List<Models.Itteration>> GetItterations(Guid projectId)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IItterationService itterationService = scope.Resolve<IItterationService>();
                return itterationService.GetByProjectId(settingsFactory.CreateApi(), projectId);
            }
        }

        private async Task GetItterationsCallback(Task<List<Models.Itteration>> getItterations, object state)
        {
            try
            {
                _itterationsVM.Itterations.Clear();
                foreach (Models.Itteration itteration in (await getItterations).OrderBy(i => i, new ItterationComparer()))
                {
                    if (_itterationsVM.ShowHidden || !(itteration.Hidden ?? false))
                        _itterationsVM.Itterations.Add(ItterationVM.Create(itteration));
                }
                if (_itterationsVM.Itterations.Count > 0)
                    _itterationsVM.SelectedItteration = _itterationsVM.Itterations[0];
                else
                    _itterationsVM.SelectedItteration = null;
            }
            catch(System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
            finally
            {
                _itterationsVM.BusyVisibility = Visibility.Collapsed;
            }
        }
    }
}
