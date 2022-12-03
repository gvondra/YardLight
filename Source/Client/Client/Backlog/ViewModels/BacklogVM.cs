using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.Backlog.Behaviors;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class BacklogVM : ViewModelBase
    {
        private readonly CreateWorkItemVM _createWorkItemVM;
        private readonly ObservableCollection<WorkItemTypeVM> _availableTypes = new ObservableCollection<WorkItemTypeVM>();
        private readonly ObservableCollection<WorkItemVM> _rootWorkItems = new ObservableCollection<WorkItemVM>();
        private Project _project;
        private RefreshBackLogCommand _refreshBackLogCommand;
        private bool _canRefresh = false;

        public BacklogVM()
        {
            _createWorkItemVM = new CreateWorkItemVM(this);
        }

        public CreateWorkItemVM CreateWorkItemVM => _createWorkItemVM;
        public ObservableCollection<WorkItemTypeVM> AvailableTypes => _availableTypes;
        public ObservableCollection<WorkItemVM> RootWorkItems => _rootWorkItems;
        
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

    }
}
