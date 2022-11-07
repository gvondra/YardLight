using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public interface IMetricService
    {
        Task<List<string>> GetEventCodes(ISettings settings);
        Task<List<Metric>> Search(ISettings settings, DateTime maxTimestamp, string eventCode);
    }
}
