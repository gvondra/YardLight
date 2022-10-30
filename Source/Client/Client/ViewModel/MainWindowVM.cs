using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YardLight.Client.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private Visibility _showUserAdmin = Visibility.Collapsed;
        private Visibility _showUserRole = Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility ShowUserAdmin
        {
            get => _showUserAdmin;
            set
            {
                if (_showUserAdmin != value)
                {
                    _showUserAdmin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility ShowUserRole
        {
            get => _showUserRole;
            set
            {
                if (_showUserRole != value)
                {
                    _showUserRole = value;
                    NotifyPropertyChanged();
                    SetShowUserAdmin();
                }
            }
        }

        private void SetShowUserAdmin()
            => ShowUserAdmin = ShowUserRole;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
