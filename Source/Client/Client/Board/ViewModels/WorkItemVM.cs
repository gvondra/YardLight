﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Interface.Models;

namespace YardLight.Client.Board.ViewModels
{
    public class WorkItemVM : ViewModelBase
    {
        private readonly WorkItem _innerWorkItem;
        private int _rowIndex = 0;
        private int _columnIndex = 0;
        private ReadOnlyCollection<WorkItemVM> _children;

        public WorkItemVM(WorkItem innerWorkItem)
        {
            _innerWorkItem = innerWorkItem;
        }

        public WorkItem InnerWorkItem => _innerWorkItem;

        public string Title => _innerWorkItem.Title;

        public string ColorCode => _innerWorkItem.Type.ColorCode;

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
