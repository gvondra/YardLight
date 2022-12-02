using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YardLight.Client.Backlog.Behaviors;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class WorkItemVM : ViewModelBase
    {
        private readonly BacklogVM _backlog;
        private readonly WorkItem _innerWorkItem;
        private readonly CreateWorkItemVM _createWorkItemVM;
        private readonly ObservableCollection<WorkItemVM> _children = new ObservableCollection<WorkItemVM>();
        private Brush _bulletColor = Brushes.Black;

        public WorkItemVM(BacklogVM backlog, WorkItem innerWorkItem)
        {
            _backlog = backlog;
            _innerWorkItem = innerWorkItem;
            _createWorkItemVM = new CreateWorkItemVM(_backlog, this)
            {
                CreateWorkItemVisible = Visibility.Collapsed
            };
            AddBehavior(new WorkItemValidator(this));
        }

        public WorkItem InnerWorkItem => _innerWorkItem;
        public Guid? WorkItemId => _innerWorkItem.WorkItemId;
        public Guid? ParentWorkItemId => _innerWorkItem.ParentWorkItemId;
        public CreateWorkItemVM CreateWorkItemVM => _createWorkItemVM;
        public ObservableCollection<WorkItemVM> Children => _children;
        public string ColorCode => _innerWorkItem.Type.ColorCode;
        public string StatusTitle => _innerWorkItem.Status?.Title;
        
        public WorkItemStatus Status
        {
            get => _innerWorkItem.Status;
            set
            {
                if (_innerWorkItem.Status != value)
                {
                    _innerWorkItem.Status = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(StatusTitle));
                }
            }
        }

        public string Title
        { 
            get => _innerWorkItem.Title;
            set
            {
                if (_innerWorkItem.Title != value)
                {
                    _innerWorkItem.Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Team
        {
            get => _innerWorkItem.Team;
            set
            {
                if (_innerWorkItem.Team != value)
                {
                    _innerWorkItem.Team = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Itteration
        {
            get => _innerWorkItem.Itteration;
            set
            {
                if (_innerWorkItem.Itteration != value)
                {
                    _innerWorkItem.Itteration = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Effort
        {
            get => _innerWorkItem.Effort;
            set
            {
                if (_innerWorkItem.Effort != value)
                {
                    _innerWorkItem.Effort = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Priority
        {
            get => _innerWorkItem.Priority;
            set
            {
                if (_innerWorkItem.Priority != value)
                {
                    _innerWorkItem.Priority = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Value
        {
            get => _innerWorkItem.Value;
            set
            {
                if (_innerWorkItem.Value != value)
                {
                    _innerWorkItem.Value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? TargetDate
        {
            get => _innerWorkItem.TargetDate;
            set
            {
                if (_innerWorkItem.TargetDate != value)
                {
                    _innerWorkItem.TargetDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? StartDate
        {
            get => _innerWorkItem.StartDate;
            set
            {
                if (_innerWorkItem.StartDate != value)
                {
                    _innerWorkItem.StartDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? CloseDate
        {
            get => _innerWorkItem.CloseDate;
            set
            {
                if (_innerWorkItem.CloseDate != value)
                {
                    _innerWorkItem.CloseDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Brush BulletColor
        {
            get => _bulletColor;
            set
            {
                if (_bulletColor != value)
                {
                    _bulletColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<WorkItemStatus> AvailableStatuses
        {
            get
            {
                return _innerWorkItem.Type.Statuses
                    .Where(s => s.WorkItemStatusId.Equals(_innerWorkItem.Status.WorkItemStatusId.Value) || (s.IsActive ?? false))                    
                    .OrderBy(s => s.Order)
                    .Select(s => s.WorkItemStatusId.Value.Equals(Status.WorkItemStatusId.Value) ? Status : s)
                    .ToList();
            }
        }

        public void AddBehavior(object behavior) => _behaviors.Add(behavior);
    }
}
