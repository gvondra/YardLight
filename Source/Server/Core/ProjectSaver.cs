using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Framework;

namespace YardLight.Core
{
    public class ProjectSaver : IProjectSaver
    {        
        public Task Create(ISettings settings, IProject project, Guid userId)
        {
            Saver saver = new Saver();
            return saver.Save(new TransactionHandler(settings), th => project.Create(th, userId));
        }

        public Task Update(ISettings settings, IProject project)
        {
            Saver saver = new Saver();
            return saver.Save(new TransactionHandler(settings), project.Update);
        }
    }
}
