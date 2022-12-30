using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client.Backlog.ViewModels
{
    public class WorkItemChildrenVM : ViewModelBase
    {
        private int _rowIndex = 0;
        private int _columnIndex = 0;
        private ReadOnlyCollection<WorkItemVM> _children;

        public ReadOnlyCollection<WorkItemVM> Children
        {
            get => _children;
            set
            {
                if (_children != value)
                {
                    _children = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int RowIndex
        {
            get => _rowIndex;
            set
            {
                if (_rowIndex != value)
                {
                    _rowIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ColumnIndex
        {
            get => _columnIndex;
            set
            {
                if (_columnIndex != value)
                {
                    _columnIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
