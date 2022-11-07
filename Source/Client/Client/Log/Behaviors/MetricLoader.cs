using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Client.Log.ViewModel;
using YardLight.Interface;

namespace YardLight.Client.Log.Behaviors
{
    public class MetricLoader
    {
        private readonly MetricLogVM _metricLogVM;
        private readonly IMetricService _metricService;
        private readonly ISettingsFactory _settingFactory;

        public MetricLoader(MetricLogVM metricLogVM, 
            ISettingsFactory settingsFactory,
            IMetricService metricService)
        {
            _metricLogVM = metricLogVM;
            _settingFactory = settingsFactory;
            _metricService = metricService;
            _metricLogVM.PropertyChanged += _metricLogVM_PropertyChanged;
        }

        private void _metricLogVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName) 
            {
                case nameof(MetricLogVM.SelectedEventCode):
                    Task.Run(() => LoadMetrics(_metricLogVM.SelectedEventCode))
                        .ContinueWith(LoadMetricsCallback, _metricLogVM.SelectedEventCode, TaskScheduler.FromCurrentSynchronizationContext());
                    break;
            }
        }

        private Task<dynamic[]> LoadMetrics(string selectedEvent)
        {
            if (!string.IsNullOrEmpty(selectedEvent))                
                return _metricService.Search(_settingFactory.CreateApi(), DateTime.UtcNow, selectedEvent);
            else
                return Task.FromResult(default(dynamic[]));
        }

        private async Task LoadMetricsCallback(Task<dynamic[]> loadMetrics, object state)
        {
            try
            {
                string selectedEvent = null;
                IEnumerable<dynamic> items = null;
                if (state != null) selectedEvent = (string)state;
                if (!string.IsNullOrEmpty(selectedEvent) && selectedEvent == _metricLogVM.SelectedEventCode)
                    items = await loadMetrics;
                if (!string.IsNullOrEmpty(selectedEvent) && items != null)
                {
                    _metricLogVM.Items.Clear();
                    foreach (dynamic item in items)
                    {
                        _metricLogVM.Items.Add(new MetricLogItemVM(item));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }
    }
}
