using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private Visibility _showUserAdmin = Visibility.Collapsed;
        private Visibility _showUserRole = Visibility.Collapsed;
        private Visibility _showLogs = Visibility.Collapsed;
        private Visibility _showProjectSettings = Visibility.Collapsed;
        private Visibility _showProject = Visibility.Collapsed;
        //private Models.Project _project = null;

        public event PropertyChangedEventHandler PropertyChanged;

        //public Models.Project Project
        //{
        //    get => _project;
        //    set
        //    {
        //        if (_project != value)
        //        {
        //            _project= value;
        //            NotifyPropertyChanged();
        //        }
        //        ShowProjectSettings = _project == null ? Visibility.Collapsed : Visibility.Visible;
        //    }
        //}

        public Visibility ShowProjectSettings
        {
            get => _showProjectSettings;
            set
            {
                if (_showProjectSettings!= value)
                {
                    _showProjectSettings= value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Visibility ShowProject
        {
            get => _showProject;
            set
            {
                if (_showProject != value)
                {
                    _showProject = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility ShowUserAdmin
        {
            get => _showUserAdmin;
            set
            {
                if (_showUserAdmin != value)
                {
                    _showUserAdmin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility ShowUserRole
        {
            get => _showUserRole;
            set
            {
                if (_showUserRole != value)
                {
                    _showUserRole = value;
                    NotifyPropertyChanged();
                    SetShowUserAdmin();
                }
            }
        }

        public Visibility ShowLogs
        {
            get => _showLogs;
            set
            {
                if (_showLogs != value)
                {
                    _showLogs = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void SetShowUserAdmin()
            => ShowUserAdmin = ShowUserRole;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
