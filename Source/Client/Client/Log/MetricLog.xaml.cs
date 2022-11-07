using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YardLight.Client.Log.ViewModel;
using YardLight.Interface;

namespace YardLight.Client.Log
{
    /// <summary>
    /// Interaction logic for MetricLog.xaml
    /// </summary>
    public partial class MetricLog : Page
    {
        public MetricLog()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += MetricLog_Loaded;
        }

        private MetricLogVM MetricLogVM { get; set; }

        private void MetricLog_Loaded(object sender, RoutedEventArgs e)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                MetricLogVM = MetricLogVM.Create(scope.Resolve<ISettingsFactory>(), scope.Resolve<IMetricService>());
                DataContext = MetricLogVM;
                Task.Run(GetEventCodes)
                    .ContinueWith(GetEventCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private Task<List<string>> GetEventCodes()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IMetricService metricService = scope.Resolve<IMetricService>();
                return metricService.GetEventCodes(settingsFactory.CreateApi());
            }
        }

        private async Task GetEventCodesCallback(Task<List<string>> getEventCodes, object state)
        {
            try
            {
                MetricLogVM.EventCodes.Clear();
                foreach (string code in await getEventCodes)
                {
                    MetricLogVM.EventCodes.Add(code);
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
