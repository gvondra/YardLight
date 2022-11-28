using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IWorkItemStatusDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkItemStatusData data, Guid userId);
        Task Update(ISqlTransactionHandler transactionHandler, WorkItemStatusData data, Guid userId);
    }
}
