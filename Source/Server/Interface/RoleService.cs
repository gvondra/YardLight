using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Interface
{
    public class RoleService : IRoleService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public RoleService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<Role> Create(ISettings settings, Role role)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, role)
                .AddPath("Role")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Role>(_service, request);
        }

        public Task<List<Role>> Get(ISettings settings)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Role")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Role>>(_service, request);
        }

        public Task<Role> Update(ISettings settings, Role role)
        {
            if (!role.RoleId.HasValue || role.RoleId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(role.RoleId));
            return Update(settings, role.RoleId.Value, role);
        }

        public Task<Role> Update(ISettings settings, Guid roleId, Role role)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, role)
                .AddPath("Role/{id}")
                .AddPathParameter("id", roleId.ToString("D"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Role>(_service, request);
        }
    }
}
