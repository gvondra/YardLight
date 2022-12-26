using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YardLight.Client.Backlog.Behaviors;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class BacklogVM : ViewModelBase
    {
        private readonly CreateWorkItemVM _createWorkItemVM;
        private readonly ObservableCollection<WorkItemTypeVM> _availableTypes = new ObservableCollection<WorkItemTypeVM>();
        private readonly ObservableCollection<WorkItemVM> _filteredChildren = new ObservableCollection<WorkItemVM>();
        private readonly WorkItemFilterVM _filter;
        private ReadOnlyCollection<WorkItemVM> _rootWorkItems = new ReadOnlyCollection<WorkItemVM>(new List<WorkItemVM>());
        private Project _project;
        private RefreshBackLogCommand _refreshBackLogCommand;
        private bool _canRefresh = false;
        private Visibility _busyVisibility = Visibility.Collapsed;

        public BacklogVM()
        {
            _createWorkItemVM = new CreateWorkItemVM(this);
            _filter = new WorkItemFilterVM(UserSessionLoader.GetUserSession());
        }

        public CreateWorkItemVM CreateWorkItemVM => _createWorkItemVM;
        public ObservableCollection<WorkItemTypeVM> AvailableTypes => _availableTypes;
        public WorkItemFilterVM Filter => _filter;
        public ObservableCollection<WorkItemVM> FilteredChildren => _filteredChildren;

        public Visibility BusyVisibility
        {
            get => _busyVisibility;
            set
            {
                if (_busyVisibility != value)
                {
                    _busyVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ReadOnlyCollection<WorkItemVM> RootWorkItems
        {
            get => _rootWorkItems;
            set
            {
                if (_rootWorkItems != value)
                {
                    _rootWorkItems = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public bool CanRefresh
        {
            get => _canRefresh;
            set
            {
                if (_canRefresh != value)
                {
                    _canRefresh = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public RefreshBackLogCommand RefreshBackLogCommand
        {
            get => _refreshBackLogCommand;
            set
            {
                if (_refreshBackLogCommand != value)
                {
                    _refreshBackLogCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Project Project
        {
            get => _project;
            set
            {
                if (_project != value)
                {
                    _project = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void AppendWorkItem(WorkItemVM workItemVM)
        {
            RootWorkItems = new ReadOnlyCollection<WorkItemVM>(
                RootWorkItems.Concat(new WorkItemVM[] { workItemVM })
                .ToList()
                );
        }

        public void ReapplyFilter()
        {
            WorkItemFilter workItemFilter = this._behaviors
                .Where(b => b.GetType().Equals(typeof(WorkItemFilter)))
                .Select(b => (WorkItemFilter)b)
                .FirstOrDefault();
            if (workItemFilter != null) 
                workItemFilter.ApplyFilter();
        }
    }
}
