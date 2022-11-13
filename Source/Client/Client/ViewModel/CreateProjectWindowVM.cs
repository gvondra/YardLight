using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.Behaviors;

namespace YardLight.Client.ViewModel
{
    public class CreateProjectWindowVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly List<object> _behaviors = new List<object>();
        private string _title = string.Empty;

        public CreateProjectWindowVM()
        {
            _behaviors.Add(new CreateProjectValidator(this));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title= value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool CanCreate => !_errors.Any(p => !string.IsNullOrEmpty(p.Value));

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get => _errors.ContainsKey(columnName) ? _errors[columnName] : null;
            set
            {
                _errors[columnName] = value;
                NotifyPropertyChanged(nameof(CanCreate));
            } 
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
