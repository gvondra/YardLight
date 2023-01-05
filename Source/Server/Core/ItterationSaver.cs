using System;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework;

namespace YardLight.Core
{
    public class ItterationSaver : IItterationSaver
    {
        private readonly Saver _saver;

        public ItterationSaver(Saver saver)
        {
            _saver = saver;
        }

        public Task Save(ISettings settings, IItteration itteration, Guid userId)
        {
            return _saver.Save(new TransactionHandler(settings), userId, itteration.Save);
        }
    }
}
