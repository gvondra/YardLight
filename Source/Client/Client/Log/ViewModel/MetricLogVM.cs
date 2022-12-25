using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Client.Log.Behaviors;
using YardLight.Interface;

namespace YardLight.Client.Log.ViewModel
{
    public class MetricLogVM : INotifyPropertyChanged
    {
        private readonly ObservableCollection<string> _eventCodes = new ObservableCollection<string>();
        private readonly ObservableCollection<object> _items = new ObservableCollection<object>();
        private string _selectedEventCode;
        private Visibility _busyVisibility = Visibility.Collapsed;
        private readonly List<object> _behaviors = new List<object>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> EventCodes => _eventCodes;

        public ObservableCollection<object> Items => _items;

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

        public string SelectedEventCode
        {
            get => _selectedEventCode;
            set
            {
                if (_selectedEventCode != value)
                {
                    _selectedEventCode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // factory method for MetricLogVM
        public static MetricLogVM Create(
            ISettingsFactory settingsFactory,
            IMetricService metricService)
        {
            MetricLogVM vm = new MetricLogVM();
            vm._behaviors.Add(new MetricLoader(vm, settingsFactory, metricService));
            return vm;
        }
    }
}
