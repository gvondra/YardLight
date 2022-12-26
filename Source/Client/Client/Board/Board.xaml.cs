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
using YardLight.Client.Board.Behaviors;
using YardLight.Client.Board.ViewModels;

namespace YardLight.Client.Board
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : Page
    {
        private BoardVMLoader _boardVMLoader;

        public Board()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.Loaded += Board_Loaded;
        }

        public BoardVM BoardVM { get; set; }

        private void Board_Loaded(object sender, RoutedEventArgs e)
        {
            if (BoardVM == null || DataContext == null)
            {
                BoardVM = new BoardVM();
                BoardVM.AddBehavior(new WorkItemFilter(BoardVM));
                DataContext = BoardVM;
            }
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            if (_boardVMLoader == null)
            {
                _boardVMLoader = new BoardVMLoader(BoardVM);
                _boardVMLoader.Load();
            }
        }
    }
}
