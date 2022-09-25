using YardLight.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Authorization.Data.Framework
{
    public interface IUserDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, UserData user);
        Task Update(ISqlTransactionHandler transactionHandler, UserData user);
        Task AddRole(ISqlTransactionHandler transactionHandler, Guid userId, int roleId);
        Task RemoveRole(ISqlTransactionHandler transactionHandler, Guid userId, int roleId);
    }
}
