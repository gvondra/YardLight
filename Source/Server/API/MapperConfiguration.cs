﻿using AutoMapper;
using YardLight.Framework;
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
                c.CreateMap<LogAPI.Exception, YardLight.Interface.Models.Exception>();
                c.CreateMap<LogAPI.Metric, Metric>();
                c.CreateMap<Project, IProject>();
                c.CreateMap<IProject, Project>();
                c.CreateMap<WorkItemStatus, IWorkItemStatus>()
                .ForMember(s => s.Order, config => config.MapFrom(source => source.Order ?? 0))
                .ForMember(s => s.IsActive, config => config.MapFrom(source => source.IsActive ?? true))
                ;
                c.CreateMap<IWorkItemStatus, WorkItemStatus>();
                c.CreateMap<WorkItemType, IWorkItemType>()
                .ForMember(t => t.IsActive, config => config.MapFrom(source => source.IsActive ?? true))
                ;
                c.CreateMap<IWorkItemType, WorkItemType>();
            });
        }

        public static AutoMapper.MapperConfiguration Get() => _mapperConfiguration;
    }
}
