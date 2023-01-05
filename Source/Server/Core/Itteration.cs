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
    public class Itteration : IItteration
    {
        private readonly ItterationData _data;
        private readonly IItterationDataSaver _dataSaver;

        public Itteration(ItterationData data, IItterationDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid? ItterationId => _data.ItterationId;

        public Guid ProjectId => _data.ProjectId;

        public string Name { get => _data.Name; set => _data.Name = value; }
        public DateTime? Start { get => _data.Start; set => _data.Start = value; }
        public DateTime? End { get => _data.End; set => _data.End = value; }
        public bool Hidden { get => _data.Hidden; set => _data.Hidden = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Guid? CreateUserId => _data.CreateUserId;

        public Guid? UpdateUserId => _data.UpdateUserId;

        public Task Save(ITransactionHandler transactionHandler, Guid userId)
        {
            return _dataSaver.Save(transactionHandler, _data, userId);
        }
    }
}
