using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public class ItterationService : IItterationService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public ItterationService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<List<Itteration>> GetByProjectId(ISettings settings, Guid projectId, string name = null)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("v2/Project/{projectId}/Itteration")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            if (!string.IsNullOrEmpty(name))
                request.AddQueryParameter("name", name);
            return _restUtil.Send<List<Itteration>>(_service, request);
        }

        public Task<Itteration> Save(ISettings settings, Guid projectId, Guid itterationId, Itteration itteration)
        {
            if (projectId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(projectId));
            if (itterationId.Equals(Guid.Empty))
                itterationId = Guid.NewGuid();
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, itteration)
                .AddPath("v2/Project/{projectId}/Itteration/{id}")
                .AddPathParameter("projectId", projectId.ToString("N"))
                .AddPathParameter("id", itterationId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Itteration>(_service, request);
        }

        public Task<Itteration> Save(ISettings settings, Itteration itteration)
        {
            if (!itteration.ProjectId.HasValue)
                throw new ArgumentNullException(nameof(itteration.ProjectId));
            if (!itteration.ItterationId.HasValue)
                itteration.ItterationId = Guid.NewGuid();
            return Save(settings, itteration.ProjectId.Value, itteration.ItterationId.Value, itteration);
        }
    }
}
