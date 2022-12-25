using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.ViewModel
{
    public class WorkItemStatusesVM : ViewModelBase
    {
        private readonly ObservableCollection<WorkItemStatusVM> _statuses = new ObservableCollection<WorkItemStatusVM>();
        private readonly WorkItemTypeVM _workItemTypeVM;
        private Project _project;
        private WorkItemStatusVM _selectedStatus;

        public WorkItemStatusesVM(WorkItemTypeVM workItemTypeVM)
        {
            _workItemTypeVM = workItemTypeVM;
        }

        public ObservableCollection<WorkItemStatusVM> Statuses => _statuses;
        public WorkItemTypeVM WorkItemTypeVM => _workItemTypeVM;

        public WorkItemStatusVM SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus= value;
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
