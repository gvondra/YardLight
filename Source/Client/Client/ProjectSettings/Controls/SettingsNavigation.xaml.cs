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

namespace YardLight.Client.ProjectSettings.Controls
{
    /// <summary>
    /// Interaction logic for SettingsNavigation.xaml
    /// </summary>
    public partial class SettingsNavigation : UserControl
    {
        public SettingsNavigation()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty NavigationFrameProperty = DependencyProperty.Register(
            "NavigationFrame", typeof(Frame), typeof(SettingsNavigation)
            );

        public Frame NavigationFrame
        {
            get => (Frame)GetValue(NavigationFrameProperty);
            set => SetValue(NavigationFrameProperty, value);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            string commandParameter = ((dynamic)e.Source)?.CommandParameter;
            if (!string.IsNullOrEmpty(commandParameter))
            {
                NavigationService navigationService = NavigationFrame.NavigationService;
                //NavigationService navigationService = NavigationService.GetNavigationService(navigationFrame);
                navigationService.Navigate(new Uri(commandParameter, UriKind.Relative));
            }
        }
    }
}
