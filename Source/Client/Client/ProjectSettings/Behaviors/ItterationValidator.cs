using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.ProjectSettings.ViewModel;

namespace YardLight.Client.ProjectSettings.Behaviors
{
    public class ItterationValidator
    {
        private readonly ItterationVM _itterationVM;

        private const string MSG_CD_REQUIRED = "is-required";
        private const string MSG_TX_REQUIRED = "is required";
        private static readonly ValueTuple<string, string, string>[] _messages = new ValueTuple<string, string, string>[]
        {
            (nameof(ItterationVM.Name), MSG_CD_REQUIRED, MSG_TX_REQUIRED),
        };

        public ItterationValidator(ItterationVM itterationVM)
        {
            _itterationVM = itterationVM;
            itterationVM.PropertyChanged += ItterationVM_PropertyChanged;
        }

        private void ItterationVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case (nameof(ItterationVM.Name)):
                    ValidateRequired(e.PropertyName, _itterationVM.Name);
                    break;
            }
        }

        private void ValidateRequired(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _itterationVM[propertyName] = _messages
                    .First(m => string.Equals(m.Item1, propertyName, StringComparison.OrdinalIgnoreCase) 
                        && string.Equals(m.Item2, MSG_CD_REQUIRED, StringComparison.OrdinalIgnoreCase))
                    .Item3;
            }
            else
            {
                _itterationVM[propertyName] = null;
            }
        }
    }
}
