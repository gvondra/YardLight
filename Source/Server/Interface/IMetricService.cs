using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface
{
    public interface IMetricService
    {
        Task<List<string>> GetEventCodes(ISettings settings);
        Task<dynamic[]> Search(ISettings settings, DateTime maxTimestamp, string eventCode);
    }
}
