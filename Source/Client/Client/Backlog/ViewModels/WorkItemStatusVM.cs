using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class WorkItemStatusVM : ViewModelBase
    {
        private readonly WorkItemStatus _innerStatus;

        public WorkItemStatusVM(WorkItemStatus innerStatus)
        {
            _innerStatus = innerStatus;
        }

        public WorkItemStatus InnerWorkItemStatus => _innerStatus;
        public Guid WorkItemStatusId => _innerStatus.WorkItemStatusId.Value;
        public string Title => _innerStatus.Title;
        public string ColorCode => _innerStatus.ColorCode;

        public Brush BulletBrush
        {
            get
            {
                BrushConverter brushConverter = new BrushConverter();
                try
                {
                    if (!string.IsNullOrEmpty(ColorCode))
                        return (Brush)brushConverter.ConvertFromString(ColorCode);
                    else
                        return Brushes.Black;
                }
                catch
                {
                    return Brushes.Black;
                }
            }
        }
    }
}
