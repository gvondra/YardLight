using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client
{
    public interface ISettingsFactory
    {
        YardLight.Interface.Authorization.ISettings CreateAuthorization();
        YardLight.Interface.Authorization.ISettings CreateAuthorization(string token);
        YardLight.Interface.ISettings CreateApi();
    }
}
