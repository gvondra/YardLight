using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Log.ViewModel
{
    public class MetricLogItemVM
    {
        private readonly Metric _innerItem;

        public MetricLogItemVM(Metric innerItem)
        {
            _innerItem = innerItem;
        }

        public DateTime Timestamp => ((DateTime)_innerItem.CreateTimestamp).ToLocalTime();

        public double? Magnitude => _innerItem.Magnitude != null ? Math.Round((double)_innerItem.Magnitude, 3) : default(double?);

        public string Status => _innerItem.Status;

        public string RequestorName => _innerItem.RequestorName;
    }
}
