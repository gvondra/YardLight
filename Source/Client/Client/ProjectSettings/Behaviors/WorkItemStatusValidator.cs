using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.ProjectSettings.ViewModel;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class WorkItemStatusValidator
    {
        private readonly WorkItemStatusVM _status;

        private const string MSG_CD_REQUIRED = "is-required";
        private const string MSG_TX_REQUIRED = "is required";
        private const string MSG_CD_INVALID_COLORCODE = "invalid-color-code";
        private const string MSG_TX_INVALID_COLORCODE = "invalid color code";
        private static readonly ValueTuple<string, string, string>[] _messages = new ValueTuple<string, string, string>[]
        {
            (nameof(WorkItemStatusVM.Title), MSG_CD_REQUIRED, MSG_TX_REQUIRED),
            (nameof(WorkItemStatusVM.ColorCode), MSG_CD_REQUIRED, MSG_TX_REQUIRED),
            (nameof(WorkItemStatusVM.ColorCode), MSG_CD_INVALID_COLORCODE, MSG_TX_INVALID_COLORCODE)
        };

        public WorkItemStatusValidator(WorkItemStatusVM status)
        {
            _status = status;
            _status.PropertyChanged += _status_PropertyChanged;
        }

        private void _status_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(WorkItemStatusVM.Title):
                    AssertIsRequired(nameof(WorkItemStatusVM.Title), _status.Title);
                    break;
                case nameof(WorkItemStatusVM.ColorCode):
                    AssertIsRequired(nameof(WorkItemStatusVM.ColorCode), _status.ColorCode);
                    AssertValidColorCode(_status.ColorCode);
                    break;
            }
        }

        private void AssertValidColorCode(string value)
        {
            BrushConverter brushConverter = new BrushConverter();
            try
            {
                if (!string.IsNullOrEmpty(value)) 
                    brushConverter.ConvertFromString(value);
                _status[nameof(WorkItemStatusVM.ColorCode)] = string.Empty;
            }
            catch
            {
                _status[nameof(WorkItemStatusVM.ColorCode)] = _messages
                    .First(m => string.Equals(nameof(WorkItemStatusVM.ColorCode), m.Item1, StringComparison.OrdinalIgnoreCase) && string.Equals(MSG_CD_INVALID_COLORCODE, m.Item2, StringComparison.OrdinalIgnoreCase))
                    .Item3;
            }
        }

        private void AssertIsRequired(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
                _status[propertyName] = _messages
                    .First(m => string.Equals(propertyName, m.Item1, StringComparison.OrdinalIgnoreCase) && string.Equals(MSG_CD_REQUIRED, m.Item2, StringComparison.OrdinalIgnoreCase))
                    .Item3;
            else
                _status[propertyName] = string.Empty;

        }
    }
}
