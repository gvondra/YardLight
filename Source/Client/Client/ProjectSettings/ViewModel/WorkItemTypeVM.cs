﻿using System;
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
    public class WorkItemTypeVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly List<object> _behaviors = new List<object>();
        private WorkItemType _workItemType;
        private bool _saveButtonEnabled = true;
        private string _createUserName;
        private string _updateUserName;

        public WorkItemTypeVM(WorkItemType workItemType)
        {
            _workItemType = workItemType;
            _behaviors.Add(new WorkItemTypeValidator(this));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WorkItemType InnerType => _workItemType;

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
                    return Brushes.Gray;
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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
