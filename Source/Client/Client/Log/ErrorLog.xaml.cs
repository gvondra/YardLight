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
using Models = YardLight.Interface.Models;
namespace YardLight.Client.Log
{
    /// <summary>
    /// Interaction logic for ErrorLog.xaml
    /// </summary>
    public partial class ErrorLog : Page
    {
        public ErrorLog()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += ErrorLog_Loaded;
        }

        private ErrorLogVM ErrorLogVM { get; set; }

        private void ErrorLog_Loaded(object sender, RoutedEventArgs e)
        {
            ErrorLogVM = new ErrorLogVM();
            DataContext= ErrorLogVM;
            Task.Run(GetExceptions)
                .ContinueWith(GetExceptionsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<List<Models.Exception>> GetExceptions()
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IExceptionService exceptionService = scope.Resolve<IExceptionService>();
                return exceptionService.Search(settingsFactory.CreateApi(), DateTime.UtcNow);
            }
        }

        private async Task GetExceptionsCallback(Task<List<Models.Exception>> getExceptions, object state)
        {
            try
            {
                ErrorLogVM.Items.Clear();
                foreach (Models.Exception exception in await getExceptions)
                {
                    ErrorLogVM.Items.Add(new ExceptionLogItemVM(exception));
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
