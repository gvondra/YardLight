using BrassLoon.DataClient;
using System;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IProjectDataSaver
    {
        // the user id given is granted access to the new project
        Task Create(ISqlTransactionHandler transactionHandler, ProjectData data, Guid userId);
        Task Update(ISqlTransactionHandler transactionHandler, ProjectData data);
    }
}
