using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client.Log.ViewModel
{
    public class MetricLogItemVM
    {
        private readonly dynamic _innerItem;

        public MetricLogItemVM(dynamic innerItem)
        {
            _innerItem = innerItem;
        }

        public DateTime Timestamp => ((DateTime)_innerItem.CreateTimestamp).ToLocalTime();

        public double? Magnitude => _innerItem.Magnitude != null ? Math.Round((double)_innerItem.Magnitude, 3) : default(double?);
    }
}
