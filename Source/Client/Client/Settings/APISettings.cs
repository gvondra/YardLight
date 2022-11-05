using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface;

namespace YardLight.Client
{
    internal class APISettings : ISettings
    {
        private readonly string _baseAddress;
        private readonly string _token;

        public APISettings(string baseAddress, string token)
        {
            _baseAddress = baseAddress;
            _token = token;
        }

        public string BaseAddress => _baseAddress;

        public Task<string> GetToken() => Task.FromResult(_token);
    }
}
