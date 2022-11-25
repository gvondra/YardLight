using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client.ViewModel
{
    public class OpenProjectWindowVM : INotifyPropertyChanged
    {
        private readonly ObservableCollection<string> _projects = new ObservableCollection<string>();
        private readonly List<Guid> _projectIds = new List<Guid>();
        private int _selectedProjectIndex;
        private bool _openButtonEnabled = true;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Projects => _projects;

        public List<Guid> ProjectIds => _projectIds;

        public bool OpenButtonEnabled
        {
            get => _openButtonEnabled;
            set
            {
                if (_openButtonEnabled != value)
                {
                    _openButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int SelectedProjectIndex
        {
            get => _selectedProjectIndex;
            set
            {
                if (_selectedProjectIndex != value)
                {
                    _selectedProjectIndex= value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
