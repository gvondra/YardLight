using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
        }

        public Guid? WorkItemId => _innerWorkItem.WorkItemId;
        public Guid? ParentWorkItemId => _innerWorkItem.ParentWorkItemId;
        public string Title => _innerWorkItem.Title;
        public CreateWorkItemVM CreateWorkItemVM => _createWorkItemVM;
        public ObservableCollection<WorkItemVM> Children => _children;
        public string ColorCode => _innerWorkItem.Type.ColorCode;
        public string StatusTitle => _innerWorkItem.Status?.Title;

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
        public void AddBehavior(object behavior) => _behaviors.Add(behavior);
    }
}
