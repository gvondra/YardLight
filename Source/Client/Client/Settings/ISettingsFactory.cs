using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client
{
    public interface ISettingsFactory
    {
        YardLight.Interface.ISettings CreateApi();
        YardLight.Interface.ISettings CreateApi(string token);
    }
}
