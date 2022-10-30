using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client.Authorization.ViewModel
{
    public class RolesVM : INotifyPropertyChanged
    {
        private readonly ObservableCollection<RoleVM> _roles = new ObservableCollection<RoleVM>();
        private RoleVM _selectedRole;

        public ObservableCollection<RoleVM> Roles => _roles;

        public event PropertyChangedEventHandler PropertyChanged;

        public RoleVM SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
