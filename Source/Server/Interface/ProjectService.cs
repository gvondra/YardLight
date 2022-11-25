using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public class ProjectService : IProjectService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public ProjectService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<Project> Create(ISettings settings, Project project)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, project)
                .AddPath("Project")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<Project> response = await _service.Send<Project>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<Project> Get(ISettings settings, Guid id)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project")
                .AddPath(id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<Project> response = await _service.Send<Project>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<List<Project>> Get(ISettings settings)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<List<Project>> response = await _service.Send<List<Project>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<Project> Update(ISettings settings, Project project)
        {
            return Update(settings, project.ProjectId, project);
        }

        public async Task<Project> Update(ISettings settings, Guid id, Project project)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, project)
                .AddPath("Project")
                .AddPath(id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<Project> response = await _service.Send<Project>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
