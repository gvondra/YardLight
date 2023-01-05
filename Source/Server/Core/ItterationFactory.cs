using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Data.Framework;
using YardLight.Data.Models;
using YardLight.Framework;

namespace YardLight.Core
{
    public class ItterationFactory : IItterationFactory
    {
        private readonly IItterationDataFactory _dataFactory;
        private readonly IItterationDataSaver _dataSaver;

        public ItterationFactory(IItterationDataFactory dataFactory, IItterationDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private Itteration Create(ItterationData data) => new Itteration(data, _dataSaver);

        public IItteration Create(Guid projectId)
        {
            if (projectId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(projectId));
            return Create(new ItterationData { ItterationId = Guid.NewGuid(), ProjectId = projectId });
        }

        public async Task<IEnumerable<IItteration>> GetByProjectId(ISettings settings, Guid projectId)
        {
            return (await _dataFactory.GetByProjectId(new DataSettings(settings), projectId))
                .Select<ItterationData, IItteration>(Create);
        }

        public Task<IItteration> Get(ISettings settings, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
