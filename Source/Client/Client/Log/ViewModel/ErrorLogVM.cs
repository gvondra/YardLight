using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.Log.ViewModel
{
    public class ErrorLogVM : INotifyPropertyChanged
    {

        private readonly ObservableCollection<ExceptionLogItemVM> _items = new ObservableCollection<ExceptionLogItemVM>();

        private Visibility _busyVisibility = Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ExceptionLogItemVM> Items => _items;

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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
