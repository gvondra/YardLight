using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public interface IUserService
    {
        Task<User> Get(ISettings settings);
        Task<User> Get(ISettings settings, Guid userId);
        Task<string> GetName(ISettings settings, Guid userId);
        Task<List<User>> Search(ISettings settings, string emailAddress);
        Task<User> Update(ISettings settings, Guid userId, User user);
        Task<User> Update(ISettings settings, User user);
    }
}
