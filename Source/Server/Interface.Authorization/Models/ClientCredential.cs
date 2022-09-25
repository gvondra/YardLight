using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Interface.Authorization.Models
{
    public class ClientCredential
    {
        public Guid? ClientId { get; set; }
        public string Secret { get; set; }
    }
}
