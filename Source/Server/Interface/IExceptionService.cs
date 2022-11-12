using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface
{
    public interface IExceptionService
    {
        Task<List<YardLight.Interface.Models.Exception>> Search(ISettings settings, DateTime maxTimestamp);
    }
}
