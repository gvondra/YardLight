using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class BacklogVM : INotifyPropertyChanged
    {
        private Project _project;

        public event PropertyChangedEventHandler PropertyChanged;

        public Project Project
        {
            get => _project;
            set
            {
                if (_project != value)
                {
                    _project = value;
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
