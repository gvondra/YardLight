using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.ProjectSettings.Behaviors;
using YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.ViewModel
{
    public class WorkItemTypeVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly WorkItemTypesVM _workItemTypesVM;
        private readonly WorkItemStatusesVM _statusesVM;
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly List<object> _behaviors = new List<object>();
        private readonly ObservableCollection<string> _brushes;
        private WorkItemType _workItemType;
        private bool _saveButtonEnabled = true;
        private string _createUserName;
        private string _updateUserName;

        public WorkItemTypeVM(WorkItemTypesVM workItemTypesVM, WorkItemType workItemType)
        {
            _workItemTypesVM = workItemTypesVM;
            _workItemType = workItemType;
            _statusesVM = new WorkItemStatusesVM(this);
            _brushes = GetBrushes();
            InitializeStatuses(workItemType);
            _behaviors.Add(new WorkItemTypeValidator(this));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WorkItemType InnerType => _workItemType;
        public WorkItemStatusesVM StatusesVM => _statusesVM;
        public WorkItemTypesVM WorkItemTypesVM => _workItemTypesVM;
        public ObservableCollection<string> Brushes => _brushes;

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
            get => _workItemType.Title;
            set
            {
                if (_workItemType.Title != value)
                {
                    _workItemType.Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ColorCode
        {
            get => _workItemType.ColorCode;
            set
            {
                if (_workItemType?.ColorCode != value)
                {
                    _workItemType.ColorCode = value;
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
                    return System.Windows.Media.Brushes.Gray;
                }
            }
        }

        public bool IsActive
        {
            get => _workItemType.IsActive ?? true;
            set
            {
                if (!_workItemType.IsActive.HasValue || _workItemType.IsActive.Value != value)
                {
                    _workItemType.IsActive = value;
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

        public DateTime CreateTimestamp => (_workItemType.CreateTimestamp ?? DateTime.Now).ToLocalTime();
        public DateTime UpdateTimestamp => (_workItemType.UpdateTimestamp ?? DateTime.Now).ToLocalTime();

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

        internal void SetInnerWorkItemType(WorkItemType workItemType)
        {
            _workItemType = workItemType;
            NotifyPropertyChanged(nameof(CreateTimestamp));
            NotifyPropertyChanged(nameof(UpdateTimestamp));
        }

        private void InitializeStatuses(WorkItemType workItemType)
        {
            _statusesVM.Statuses.Clear();
            foreach (WorkItemStatus workItemStatus in workItemType.Statuses ?? new List<WorkItemStatus>())
            {
                _statusesVM.Statuses.Add(new WorkItemStatusVM(workItemStatus));
            }
            if (_statusesVM.Statuses.Count > 0) 
                _statusesVM.SelectedStatus = _statusesVM.Statuses[0];
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal static ObservableCollection<string> GetBrushes()
        {
            ObservableCollection<string> result = new ObservableCollection<string>(
                typeof(System.Windows.Media.Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.CanRead)
                .Select(p => p.Name)
                );            
            return result;
        }
    }
}
