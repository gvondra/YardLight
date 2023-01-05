using BrassLoon.DataClient;
using System;
using System.Threading.Tasks;
using YardLight.Data.Models;

namespace YardLight.Data.Framework
{
    public interface IItterationDataSaver
    {
        Task Save(ISqlTransactionHandler transactionHandler, ItterationData data, Guid userId);
    }
}
