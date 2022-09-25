using AutoMapper;
using YardLight.Authorization.Core.Framework;
using YardLight.Interface.Authorization.Models;
namespace AuthorizationAPI
{
    public sealed class MapperConfiguration
    {
        private static readonly AutoMapper.MapperConfiguration _mapperConfiguration;

        static MapperConfiguration()
        {
            _mapperConfiguration = new AutoMapper.MapperConfiguration(configExp =>
            {
                configExp.CreateMap<IClient, Client>();
                configExp.CreateMap<Client, IClient>();
                configExp.CreateMap<IRole, Role>();
                configExp.CreateMap<Role, IRole>();
                configExp.CreateMap<IUser, User>();
                configExp.CreateMap<User, IUser>();
            });
        }

        public static IMapper CreateMapper() => new Mapper(_mapperConfiguration);
    }
}
