using YardLight.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Authorization
{
    public interface IUserService
    {
        Task<User> Get(ISettings settings);
        Task<User> Get(ISettings settings, Guid id);
        Task<string> GetName(ISettings settings, Guid id);
        Task<User> GetByEmailAddress(ISettings settings, string emailAddress);
        Task<User> Update(ISettings settings, User user);
    }
}
