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
    /// Interaction logic for Members.xaml
    /// </summary>
    public partial class Members : Page
    {
        private MembersVMLoader _membersVMLoader;
        public Members()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += Members_Loaded;
        }

        private MembersVM MembersVM { get; set; }

        private void Members_Loaded(object sender, RoutedEventArgs e)
        {
            MembersVM = MembersVM.Create();
            DataContext = MembersVM;
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            _membersVMLoader = new MembersVMLoader(MembersVM);
            _membersVMLoader.Load();
        }
    }
}
