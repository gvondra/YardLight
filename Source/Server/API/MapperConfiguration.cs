using AutoMapper;
using YardLight.Interface.Models;
using LogAPI = BrassLoon.Interface.Log.Models;
namespace API
{
    public sealed class MapperConfiguration
    {
        private readonly static AutoMapper.MapperConfiguration _mapperConfiguration;

        static MapperConfiguration()
        {
            _mapperConfiguration= new AutoMapper.MapperConfiguration((c) =>
            {
                c.CreateMap<LogAPI.Metric, Metric>();
            });
        }

        public static AutoMapper.MapperConfiguration Get() => _mapperConfiguration;
    }
}
