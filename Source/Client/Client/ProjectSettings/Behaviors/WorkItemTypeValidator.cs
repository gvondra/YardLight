using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.ProjectSettings.ViewModel;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class WorkItemTypeValidator
    {
        private readonly WorkItemTypeVM _type;

        private const string MSG_CD_REQUIRED = "is-required";
        private const string MSG_TX_REQUIRED = "is required";
        private const string MSG_CD_INVALID_COLORCODE = "invalid-color-code";
        private const string MSG_TX_INVALID_COLORCODE = "invalid color code";
        private static readonly ValueTuple<string, string, string>[] _messages = new ValueTuple<string, string, string>[]
        {
            (nameof(WorkItemTypeVM.Title), MSG_CD_REQUIRED, MSG_TX_REQUIRED),
            (nameof(WorkItemTypeVM.ColorCode), MSG_CD_REQUIRED, MSG_TX_REQUIRED),
            (nameof(WorkItemTypeVM.ColorCode), MSG_CD_INVALID_COLORCODE, MSG_TX_INVALID_COLORCODE)
        };

        public WorkItemTypeValidator(WorkItemTypeVM type)
        {
            _type = type;
            _type.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(WorkItemTypeVM.Title):
                    AssertIsRequired(nameof(WorkItemTypeVM.Title), _type.Title);
                    break;
                case nameof(WorkItemTypeVM.ColorCode):
                    AssertIsRequired(nameof(WorkItemTypeVM.ColorCode), _type.ColorCode);
                    AssertValidColorCode(_type.ColorCode);
                    break;
            }
        }

        private void AssertValidColorCode(string value)
        {
            BrushConverter brushConverter = new BrushConverter();
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    brushConverter.ConvertFromString(value);
                    _type[nameof(WorkItemTypeVM.ColorCode)] = string.Empty;
                }                    
            }
            catch
            {
                _type[nameof(WorkItemTypeVM.ColorCode)] = _messages
                    .First(m => string.Equals(nameof(WorkItemTypeVM.ColorCode), m.Item1, StringComparison.OrdinalIgnoreCase) && string.Equals(MSG_CD_INVALID_COLORCODE, m.Item2, StringComparison.OrdinalIgnoreCase))
                    .Item3;
            }
        }

        private void AssertIsRequired(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
                _type[propertyName] = _messages
                    .First(m => string.Equals(propertyName, m.Item1, StringComparison.OrdinalIgnoreCase) && string.Equals(MSG_CD_REQUIRED, m.Item2, StringComparison.OrdinalIgnoreCase))
                    .Item3;
            else
                _type[propertyName] = string.Empty;

        }
    }
}
