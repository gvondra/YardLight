using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Authorization.ViewModel
{
    public class FindUserRoleVM : INotifyPropertyChanged
    {
        private bool _isActive = false;
        private Role _innerRole;

        public FindUserRoleVM(Role role)
        {
            _innerRole = role;
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name => _innerRole.Name;
        public string PolicyName => _innerRole?.PolicyName;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
