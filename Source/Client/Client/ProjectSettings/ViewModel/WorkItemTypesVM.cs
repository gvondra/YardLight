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
    public class WorkItemTypesVM : INotifyPropertyChanged
    {
        private Project _project;
        private WorkItemTypeVM _selectedType;
        private readonly ObservableCollection<WorkItemTypeVM> _types = new ObservableCollection<WorkItemTypeVM>();
        private bool _showInactive = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<WorkItemTypeVM> Types => _types;

        public bool ShowInactive
        {
            get => _showInactive;
            set
            {
                if (_showInactive != value)
                {
                    _showInactive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public WorkItemTypeVM SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                {
                    _selectedType= value;
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
