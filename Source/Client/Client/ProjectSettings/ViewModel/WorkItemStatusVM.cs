using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.ProjectSettings.Behaviors;
using YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.ViewModel
{
    public class WorkItemStatusVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly List<object> _behaviors = new List<object>();
        private WorkItemStatus _workItemStatus;
        private bool _saveButtonEnabled = true;
        private string _createUserName;
        private string _updateUserName;

        public WorkItemStatusVM(WorkItemStatus workItemStatus)
        {
            _workItemStatus = workItemStatus;
            _behaviors.Add(new WorkItemStatusValidator(this));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WorkItemStatus InnerStatus => _workItemStatus;

        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set
            {
                if (_saveButtonEnabled != value)
                {
                    _saveButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => _workItemStatus.Title;
            set
            {
                if (_workItemStatus.Title != value)
                {
                    _workItemStatus.Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ColorCode
        {
            get => _workItemStatus.ColorCode;
            set
            {
                if (_workItemStatus?.ColorCode != value)
                {
                    _workItemStatus.ColorCode = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(BulletColor));
                }
            }
        }

        public Brush BulletColor
        {
            get
            {
                BrushConverter brushConverter = new BrushConverter();
                try
                {
                    if (!string.IsNullOrEmpty(ColorCode))
                        return (Brush)brushConverter.ConvertFromString(ColorCode);
                    else
                        return null;
                }
                catch
                {
                    return Brushes.Gray;
                }
            }
        }

        public short Order
        {
            get => _workItemStatus.Order ?? 0;
            set 
            {
                if (!_workItemStatus.Order.HasValue || _workItemStatus.Order.Value != value)
                {
                    _workItemStatus.Order = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get => _workItemStatus.IsActive ?? true;
            set
            {
                if (!_workItemStatus.IsActive.HasValue || _workItemStatus.IsActive.Value != value)
                {
                    _workItemStatus.IsActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string CreateUserName
        {
            get => _createUserName;
            set
            {
                if (_createUserName != value)
                {
                    _createUserName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string UpdateUserName
        {
            get => _updateUserName;
            set
            {
                if (_updateUserName != value)
                {
                    _updateUserName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime CreateTimestamp => (_workItemStatus.CreateTimestamp ?? DateTime.Now).ToLocalTime();
        public DateTime UpdateTimestamp => (_workItemStatus.UpdateTimestamp ?? DateTime.Now).ToLocalTime();

        public bool HasErrors => _errors.Any(pair => !string.IsNullOrEmpty(pair.Value));

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get => _errors.ContainsKey(columnName) ? _errors[columnName] : null;
            set
            {
                _errors[columnName] = value;
                NotifyPropertyChanged(nameof(HasErrors));
            }
        }

        internal void SetInnerWorkItemStatus(WorkItemStatus workItemStatus)
        {
            _workItemStatus = workItemStatus;
            NotifyPropertyChanged(nameof(CreateTimestamp));
            NotifyPropertyChanged(nameof(UpdateTimestamp));
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
