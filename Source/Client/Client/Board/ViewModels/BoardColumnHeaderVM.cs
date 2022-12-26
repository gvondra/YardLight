using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client.Board.ViewModels
{
    public class BoardColumnHeaderVM : ViewModelBase
    {
        private int _rowIndex = 0;
        private int _columnIndex = 0;
        private string _title;
        private Guid? _id;

        public Guid? Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
