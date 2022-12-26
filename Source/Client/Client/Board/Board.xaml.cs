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

        public static readonly DependencyProperty RowCountProperty = DependencyProperty.RegisterAttached("RowCount", typeof(int), typeof(Board),
            new PropertyMetadata(0, new PropertyChangedCallback(SetGridRowDefinitions)));

        public static int GetRowCount(DependencyObject target) => (int)target.GetValue(RowCountProperty);

        public static void SetRowCount(DependencyObject target, int value) => target.SetValue(RowCountProperty, value);

        public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.RegisterAttached("ColumnCount", typeof(int), typeof(Board),
            new PropertyMetadata(0, new PropertyChangedCallback(SetGridColumnDefinitions)));

        public static int GetColumnCount(DependencyObject target) => (int)target.GetValue(ColumnCountProperty);

        public static void SetColumnCount(DependencyObject target, int value) => target.SetValue(ColumnCountProperty, value);

        private void Board_Loaded(object sender, RoutedEventArgs e)
        {
            if (BoardVM == null || DataContext == null)
            {
                BoardVM = new BoardVM();
                BoardVM.AddBehavior(new WorkItemFilter(BoardVM));
                BoardVM.AddBehavior(new BoardLayout(BoardVM));
                DataContext = BoardVM;
            }
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            if (_boardVMLoader == null)
            {
                _boardVMLoader = new BoardVMLoader(BoardVM);
                _boardVMLoader.Load();
            }         
        }

        private static void SetGridRowDefinitions(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            Grid grid = (Grid)target;
            for (int i = grid.RowDefinitions.Count; i <= (int)args.NewValue; i += 1)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
        }

        private static void SetGridColumnDefinitions(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            Grid grid = (Grid)target;
            if (grid.ColumnDefinitions.Count == 0)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            for (int i = grid.ColumnDefinitions.Count; i <= (int)args.NewValue; i += 1)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
        }
    }
}
