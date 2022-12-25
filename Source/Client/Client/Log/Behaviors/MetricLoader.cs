using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Client.Log.ViewModel;
using YardLight.Interface;
using YardLight.Interface.Models;

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
                    _metricLogVM.BusyVisibility = Visibility.Visible;
                    Task.Run(() => LoadMetrics(_metricLogVM.SelectedEventCode))
                        .ContinueWith(LoadMetricsCallback, _metricLogVM.SelectedEventCode, TaskScheduler.FromCurrentSynchronizationContext());
                    break;
            }
        }

        private Task<List<Metric>> LoadMetrics(string selectedEvent)
        {
            if (!string.IsNullOrEmpty(selectedEvent))                
                return _metricService.Search(_settingFactory.CreateApi(), DateTime.UtcNow, selectedEvent);
            else
                return Task.FromResult(default(List<Metric>));
        }

        private async Task LoadMetricsCallback(Task<List<Metric>> loadMetrics, object state)
        {
            try
            {
                string selectedEvent = null;
                List<Metric> items = null;
                if (state != null) selectedEvent = (string)state;
                if (!string.IsNullOrEmpty(selectedEvent) && selectedEvent == _metricLogVM.SelectedEventCode)
                    items = await loadMetrics;
                if (!string.IsNullOrEmpty(selectedEvent) && items != null)
                {
                    _metricLogVM.Items.Clear();
                    foreach (Metric item in items)
                    {
                        _metricLogVM.Items.Add(new MetricLogItemVM(item));
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
            finally
            {
                _metricLogVM.BusyVisibility = Visibility.Collapsed;
            }
        }
    }
}
