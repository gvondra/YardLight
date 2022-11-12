using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.Log.ViewModel
{
    public class ErrorLogVM : INotifyPropertyChanged
    {
        private readonly ObservableCollection<ExceptionLogItemVM> _items = new ObservableCollection<ExceptionLogItemVM>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ExceptionLogItemVM> Items => _items;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
