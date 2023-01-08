using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework;

namespace YardLight.Core
{
    public class ProjectSaver : IProjectSaver
    {
        public Task Create(ISettings settings, IProject project, string userEmailAddress)
        {
            Saver saver = new Saver();
            return saver.Save(new TransactionHandler(settings), th => project.Create(th, userEmailAddress));
        }

        public async Task Update(ISettings settings, IProject project)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), project.Update);
        }
    }
}
