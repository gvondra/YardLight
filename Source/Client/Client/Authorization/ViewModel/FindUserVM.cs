using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using InterfaceModels = YardLight.Interface.Models;

namespace YardLight.Client.Authorization.ViewModel
{
    public class FindUserVM : INotifyPropertyChanged
    {
        private string _findAddress;
        private InterfaceModels.User _user;
        private List<InterfaceModels.Role> _allRoles;
        private ObservableCollection<FindUserRoleVM> _roles = new ObservableCollection<FindUserRoleVM>();

        public event PropertyChangedEventHandler PropertyChanged;
        // the email address to be searched for
        public string FindAddress
        {
            get => _findAddress;
            set
            {
                if (_findAddress != value)
                {
                    _findAddress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<FindUserRoleVM> Roles => _roles;

        public InterfaceModels.User User
        {
            get => _user;
            set
            {
                _user = value;
                NotifyPropertyChanged();
            }
        }

        public List<InterfaceModels.Role> AllRoles
        {
            get => _allRoles;
            set
            {
                _allRoles = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
