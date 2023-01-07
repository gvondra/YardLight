using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YardLight.Client.ProjectSettings.ViewModel
{
    public class ItterationsVM : ViewModelBase
    {
        private readonly ObservableCollection<ItterationVM> _itterations = new ObservableCollection<ItterationVM>(); 
        private Visibility _busyVisibility = Visibility.Collapsed;
        private bool _showHidden = false;
        private ItterationVM _selectedItteration;

        public ObservableCollection<ItterationVM> Itterations => _itterations;

        public ItterationVM SelectedItteration
        {
            get => _selectedItteration;
            set
            {
                if (_selectedItteration != value)
                {
                    _selectedItteration = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool ShowHidden
        {
            get => _showHidden;
            set
            {
                if (_showHidden != value)
                {
                    _showHidden = value;
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
