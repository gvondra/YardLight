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
    public class WorkItemStatusesVM : INotifyPropertyChanged
    {
        private Project _project;
        private WorkItemStatusVM _selectedStatus;
        private readonly ObservableCollection<WorkItemStatusVM> _statuses = new ObservableCollection<WorkItemStatusVM>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<WorkItemStatusVM> Statuses => _statuses;

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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
