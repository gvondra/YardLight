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
using YardLight.Client.ProjectSettings.Behaviors;
using YardLight.Client.ProjectSettings.ViewModel;

namespace YardLight.Client.ProjectSettings
{
    /// <summary>
    /// Interaction logic for Itterations.xaml
    /// </summary>
    public partial class Itterations : Page
    {
        private ItterationsVMLoader _itterationsVMLoader;
        public Itterations()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += Itterations_Loaded;
        }

        public ItterationsVM ItterationsVM { get; set; }

        private void Itterations_Loaded(object sender, RoutedEventArgs e)
        {
            ItterationsVM = new ItterationsVM();
            DataContext = ItterationsVM;
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            _itterationsVMLoader = new ItterationsVMLoader(ItterationsVM);
            _itterationsVMLoader.Load();
        }

        private void AddHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserSession userSession = UserSessionLoader.GetUserSession();
                if (userSession.OpenProjectId.HasValue)
                {
                    ItterationVM last = ItterationsVM.Itterations
                        .Where(i => i.End.HasValue)
                        .OrderByDescending(i => i.End.Value)
                        .FirstOrDefault();
                    ItterationVM itterationVM = ItterationVM.Create(new Interface.Models.Itteration() { ProjectId = userSession.OpenProjectId.Value, ItterationId = Guid.NewGuid() });                    
                    itterationVM.Name = "New Itteration";
                    itterationVM.Hidden = false;
                    if (last != null)
                    {
                        itterationVM.Start = last.End.Value.AddDays(1);
                        if (last.Start.HasValue)
                            itterationVM.End = itterationVM.Start.Value.AddDays(last.End.Value.Subtract(last.Start.Value).Days);
                    }
                    ItterationsVM.Itterations.Add(itterationVM);
                    ItterationsVM.SelectedItteration = itterationVM;
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
