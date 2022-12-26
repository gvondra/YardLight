using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Interface.Models;

namespace YardLight.Client.Board.ViewModels
{
    public class BoardVM : ViewModelBase
    {
        private WorkItemFilterVM _filter;
        private Project _project;
        private Visibility _busyVisibility = Visibility.Collapsed;
        private ReadOnlyCollection<WorkItemVM> _workItems;

        public ReadOnlyCollection<WorkItemVM> WorkItems
        {
            get => _workItems;
            set
            {
                if (_workItems != value)
                {
                    _workItems = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public WorkItemFilterVM Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
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

    }
}
