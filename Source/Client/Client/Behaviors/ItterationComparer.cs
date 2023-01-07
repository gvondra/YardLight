using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Behaviors
{
    public class ItterationComparer : IComparer<Itteration>
    {
        public int Compare(Itteration x, Itteration y)
        {
            int result;
            result = -1 * DateTime.Compare(x.End ?? DateTime.Today, y.End ?? DateTime.Today);
            if (result == 0)
                result = -1 * DateTime.Compare(x.Start ?? DateTime.Today, y.Start ?? DateTime.Today);
            if (result == 0)
                result = -1 * string.Compare((x.Name ?? string.Empty), (y.Name ?? string.Empty), StringComparison.OrdinalIgnoreCase);
            return result;
        }
    }
}
