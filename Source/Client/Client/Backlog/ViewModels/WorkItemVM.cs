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
        private readonly WorkItem _innerWorkItem;
        private readonly CreateWorkItemVM _createWorkItemVM;
        private readonly ObservableCollection<WorkItemVM> _filteredChildren = new ObservableCollection<WorkItemVM>();
        private ReadOnlyCollection<WorkItemVM> _children = new ReadOnlyCollection<WorkItemVM>(new List<WorkItemVM>());
        private Brush _bulletColor = Brushes.Black;
        private List<WorkItemStatusVM> _availableStatuses;
        private List<string> _itterations = new List<string>();
        private List<string> _teams = new List<string>();
        private string _newCommentText;
        private ObservableCollection<CommentVM> _comments = new ObservableCollection<CommentVM>();
        private LoadWorkItemCommentCommand _loadWorkItemCommentCommand;
        private CreateWorkIemCommentCommand _createWorkIemCommentCommand;
        private bool _isExpanded = true;
        private int _rowIndex = 0;
        private int _columnIndex = 0;
        private WorkItemEditCommand _workItemEditCommand;
        private Visibility _statusVisibility = Visibility.Visible;

        public WorkItemVM(WorkItem innerWorkItem)
        {
            _innerWorkItem = innerWorkItem;
            AddBehavior(new WorkItemValidator(this));
        }

        public WorkItemVM(BacklogVM backlog, WorkItem innerWorkItem) : this(innerWorkItem)
        {
            _createWorkItemVM = new CreateWorkItemVM(backlog, this)
            {
                CreateWorkItemVisible = Visibility.Collapsed
            };
        }

        public WorkItem InnerWorkItem => _innerWorkItem;
        public Guid? WorkItemId => _innerWorkItem.WorkItemId;
        public Guid? ParentWorkItemId => _innerWorkItem.ParentWorkItemId;
        public CreateWorkItemVM CreateWorkItemVM => _createWorkItemVM;
        public string ColorCode => _innerWorkItem.Type.ColorCode;
        public string StatusTitle => _innerWorkItem.Status?.Title;
        public ObservableCollection<CommentVM> Comments => _comments;
        public ObservableCollection<WorkItemVM> FilteredChildren => _filteredChildren;
        public Guid? ProjectId => _innerWorkItem.ProjectId;

        public Visibility StatusVisibility
        {
            get => _statusVisibility;
            set
            {
                if (_statusVisibility != value)
                {
                    _statusVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public WorkItemEditCommand WorkItemEditCommand
        {
            get => _workItemEditCommand;
            set
            {
                if (_workItemEditCommand != value)
                {
                    _workItemEditCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ReadOnlyCollection<WorkItemVM> Children
        { 
            get => _children; 
            set
            {
                if (_children != value)
                {
                    _children = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public CreateWorkIemCommentCommand CreateWorkIemCommentCommand
        {
            get => _createWorkIemCommentCommand;
            set
            {
                if (_createWorkIemCommentCommand != value)
                {
                    _createWorkIemCommentCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public LoadWorkItemCommentCommand LoadWorkItemCommentCommand
        {
            get => _loadWorkItemCommentCommand;
            set
            {
                if (_loadWorkItemCommentCommand != value)
                {
                    _loadWorkItemCommentCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string NewCommentText
        {
            get => _newCommentText;
            set
            {
                if (_newCommentText != value)
                {
                    _newCommentText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<string> Itterations
        {
            get => _itterations;
            set
            {
                if (_itterations != value)
                {
                    _itterations = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<string> Teams
        {
            get => _teams;
            set
            {
                if (_teams != value)
                {
                    _teams = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public WorkItemStatusVM Status
        {
            get => AvailableStatuses.First(s => s.WorkItemStatusId.Equals(_innerWorkItem.Status.WorkItemStatusId.Value));
            set
            {
                if (!AvailableStatuses.First(s => s.WorkItemStatusId.Equals(_innerWorkItem.Status.WorkItemStatusId.Value)).WorkItemStatusId.Equals(value.WorkItemStatusId))
                {
                    _innerWorkItem.Status = value.InnerWorkItemStatus;
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

        public string Description
        {
            get => _innerWorkItem.Description;
            set
            {
                if (_innerWorkItem.Description != value)
                {
                    _innerWorkItem.Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Criteria
        {
            get => _innerWorkItem.Criteria;
            set
            {
                if (_innerWorkItem.Criteria != value)
                {
                    _innerWorkItem.Criteria = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int RowIndex
        {
            get => _rowIndex;
            set
            {
                if (_rowIndex != value)
                {
                    _rowIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ColumnIndex
        {
            get => _columnIndex;
            set
            {
                if (_columnIndex != value)
                {
                    _columnIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<WorkItemStatusVM> AvailableStatuses
        {
            get
            {
                if (_availableStatuses == null)
                {
                    _availableStatuses = _innerWorkItem.Type.Statuses
                    .Where(s => s.WorkItemStatusId.Equals(_innerWorkItem.Status.WorkItemStatusId.Value) || (s.IsActive ?? false))
                    .OrderBy(s => s.Order)
                    .Select(s => s.WorkItemStatusId.Value.Equals(_innerWorkItem.Status.WorkItemStatusId.Value) ? new WorkItemStatusVM(_innerWorkItem.Status) : new WorkItemStatusVM(s))
                    .ToList();
                }
                return _availableStatuses;
            }
        }

        // default create method attaches a "create work item loader"
        public static WorkItemVM Create(BacklogVM backlog, WorkItem innerWorkItem)
        {
            WorkItemVM workItemVM = new WorkItemVM(backlog, innerWorkItem);
            workItemVM.AddBehavior(new CreateWorkItemLoader(workItemVM.CreateWorkItemVM));
            return workItemVM;
        }

        public void AppendChild(WorkItemVM workItemVM)
        {
            Children = new ReadOnlyCollection<WorkItemVM>(
                Children.Concat(new WorkItemVM[] { workItemVM })
                .ToList()
                );
        }
    }
}
