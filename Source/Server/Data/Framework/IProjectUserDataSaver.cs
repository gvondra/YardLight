using BrassLoon.DataClient;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IProjectUserDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, ProjectUserData data);
        Task Update(ISqlTransactionHandler transactionHandler, ProjectUserData data);
    }
}
