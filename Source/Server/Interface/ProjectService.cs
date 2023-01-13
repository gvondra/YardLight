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

        public async Task AddUser(ISettings settings, Guid id, string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post)
                .AddPath("Project/{id}/Users")
                .AddPathParameter("id", id.ToString("N"))
                .AddQueryParameter("emailAddress", emailAddress)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            _restUtil.CheckSuccess(
                await _service.Send(request)
                );
        }

        public async Task RemoveUser(ISettings settings, Guid id, string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Delete)
                .AddPath("Project/{id}/Users")
                .AddPathParameter("id", id.ToString("N"))
                .AddQueryParameter("emailAddress", emailAddress)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            _restUtil.CheckSuccess(
                await _service.Send(request)
                );
        }

        public Task<string[]> GetUsers(ISettings settings, Guid id)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Project/{id}/Users")
                .AddPathParameter("id", id.ToString("N"))                
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<string[]>(_service, request);
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
