using BrassLoon.Interface.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.CommonAPI
{
    public class LogSettings : ISettings
    {
        private readonly string _baseAddress;
        private readonly string _token;

        public LogSettings(string baseAddress, string token)
        {
            _baseAddress = baseAddress;
            _token = token;
        }

        public string BaseAddress => _baseAddress;

        public Task<string> GetToken()
        {
            return Task.FromResult(_token);
        }
    }
}
