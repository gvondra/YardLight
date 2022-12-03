using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client.Backlog.ViewModels
{
    public class WorkItemFilterVM : ViewModelBase
    {
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void CopyFrom(WorkItemFilterVM source)
        {
            Title = source.Title;
        }

    }
}
