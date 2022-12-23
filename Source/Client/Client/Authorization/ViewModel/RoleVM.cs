using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Authorization.ViewModel
{
    public class RoleVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly Role _innerRole;
        public bool _isNew = false;
        private readonly List<object> _validators = new List<object>();

        public RoleVM(Role innerRole)
        {
            _innerRole = innerRole;
            _validators.Add(new RoleValidator(this));
        }

        public RoleVM() : this(new Role()) { }

        public event PropertyChangedEventHandler PropertyChanged;

        public Role InnerRole => _innerRole;

        public bool IsNew
        {
            get => _isNew;
            set
            {
                if (_isNew != value)
                {
                    _isNew = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Guid? RoleId
        {
            get => _innerRole.RoleId;
            set
            {
                _innerRole.RoleId = value;
                NotifyPropertyChanged();
            }
        }

        public string PolicyName
        {
            get => _innerRole.PolicyName;
            set
            {
                if (_innerRole.PolicyName != value)
                {
                    _innerRole.PolicyName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _innerRole.Name;
            set
            {
                if (_innerRole.Name != value)
                {
                    _innerRole.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get => _errors.ContainsKey(columnName) ? _errors[columnName] : null;
            set => _errors[columnName] = value;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
