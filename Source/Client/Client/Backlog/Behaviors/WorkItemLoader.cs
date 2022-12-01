using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog.Behaviors
{
    public class WorkItemLoader
    {
        private readonly WorkItemVM _workItemVM;

        public WorkItemLoader(WorkItemVM workItemVM)
        {
            _workItemVM = workItemVM;
            SetBulletColor();
        }

        private void SetBulletColor()
        {            
            BrushConverter brushConverter = new BrushConverter();
            try
            {
                if (!string.IsNullOrEmpty(_workItemVM.ColorCode))
                    _workItemVM.BulletColor = (Brush)brushConverter.ConvertFromString(_workItemVM.ColorCode);
                else
                    _workItemVM.BulletColor = Brushes.Black;
            }
            catch
            {
                _workItemVM.BulletColor = Brushes.Black;
            }
        }
    }
}
