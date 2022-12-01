using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class WorkItemTypeVM : ViewModelBase
    {
        private readonly WorkItemType _innerType;

        public WorkItemTypeVM(WorkItemType innerType)
        {
            _innerType = innerType;
        }

        public WorkItemType InnerWorkItemType => _innerType;
        public Guid WorkItemTypeId => _innerType.WorkItemTypeId.Value;
        public string Title => _innerType.Title;
        public string ColorCode => _innerType.ColorCode;
        public bool IsActive => _innerType.IsActive ?? true;
    }
}
