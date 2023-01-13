using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public interface IItterationService
    {
        Task<List<Itteration>> GetByProjectId(ISettings settings, Guid projectId, string name = null);
        Task<Itteration> Save(ISettings settings, Guid projectId, Guid itterationId, Itteration itteration);
        Task<Itteration> Save(ISettings settings, Itteration itteration);
    }
}
