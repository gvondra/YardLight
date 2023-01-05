using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IItterationDataFactory
    {
        Task<IEnumerable<ItterationData>> GetByProjectId(ISettings settings, Guid projectId);
    }
}
