using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.ProjectSettings.ViewModel;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class ProjectSettingsValidator
    {
        private readonly ProjectSettingsVM _projectSettings;

        private const string MSG_CD_REQUIRED = "is-required";
        private const string MSG_TX_REQUIRED = "is required";
        private static readonly ValueTuple<string, string, string>[] _messages = new ValueTuple<string, string, string>[]
        {
            (nameof(ProjectSettingsVM.Title), MSG_CD_REQUIRED, MSG_TX_REQUIRED)
        };

        public ProjectSettingsValidator(ProjectSettingsVM projectSettings)
        {
            _projectSettings = projectSettings;
            _projectSettings.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ProjectSettingsVM.Title):
                    AssertIsRequired(e.PropertyName, _projectSettings.Title);
                    break;
            }
        }

        private void AssertIsRequired(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
                _projectSettings[propertyName] = _messages
                    .First(m => string.Equals(propertyName, m.Item1, StringComparison.OrdinalIgnoreCase) && string.Equals(MSG_CD_REQUIRED, m.Item2, StringComparison.OrdinalIgnoreCase))
                    .Item3;
            else
                _projectSettings[propertyName] = string.Empty;

        }
    }
}
