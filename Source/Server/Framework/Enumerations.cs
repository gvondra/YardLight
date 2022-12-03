using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Framework.Enumerations
{
    public enum WorkItemCommentType : short
    {
        NotSet = 0x00,
        Description = 0x01,
        Criteria = 0x02,
        Comment = 0x03
    }
}
