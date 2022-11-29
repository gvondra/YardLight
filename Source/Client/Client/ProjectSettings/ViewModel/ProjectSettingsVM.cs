using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.ProjectSettings.Behaviors;
using Models = YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.ViewModel
{
    public class ProjectSettingsVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly List<object> _behaviors = new List<object>();
        private Models.Project _project;
        private string _saveButtonText = "Save";
        private bool _saveButtonEnabled = true;

        public ProjectSettingsVM(Models.Project project)
        {
            _project = project;
            _behaviors.Add(new ProjectSettingsValidator(this));
        }

        public ProjectSettingsVM() {}

        public event PropertyChangedEventHandler PropertyChanged;

        public Models.Project InnerProject => _project;

        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set
            {
                if (_saveButtonEnabled != value)
                {
                    _saveButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SaveButtonText
        {
            get => _saveButtonText;
            set
            {
                if (_saveButtonText != value)
                {
                    _saveButtonText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => _project?.Title;
            set
            {
                if (_project.Title != value)
                {
                    _project.Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool HasErrors => _errors.Any(pair => !string.IsNullOrEmpty(pair.Value));

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get => _errors.ContainsKey(columnName) ? _errors[columnName] : null;
            set
            {
                _errors[columnName] = value;
                NotifyPropertyChanged(nameof(HasErrors));
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
