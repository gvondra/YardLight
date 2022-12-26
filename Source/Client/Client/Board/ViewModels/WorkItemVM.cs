using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Board.ViewModels
{
    public class WorkItemVM : ViewModelBase
    {
        private readonly WorkItem _innerWorkItem;

        public WorkItemVM(WorkItem innerWorkItem)
        {
            _innerWorkItem = innerWorkItem;
        }

        public string Title => _innerWorkItem.Title;
    }
}
