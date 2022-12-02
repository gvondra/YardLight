using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IWorkItemCommentDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkItemCommentData data, Guid userId);
    }
}
