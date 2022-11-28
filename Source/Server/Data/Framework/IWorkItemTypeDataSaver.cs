using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IWorkItemTypeDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkItemTypeData data, Guid userId);
        Task Update(ISqlTransactionHandler transactionHandler, WorkItemTypeData data, Guid userId);
    }
}
