using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog.Behaviors
{
    public class WorkItemValidator
    {
        private readonly WorkItemVM _workItemVM;

        private const string MSG_CD_REQUIRED = "is-required";
        private const string MSG_TX_REQUIRED = "is required";
        private static readonly ValueTuple<string, string, string>[] _messages = new ValueTuple<string, string, string>[]
        {
            (nameof(WorkItemVM.Title), MSG_CD_REQUIRED, MSG_TX_REQUIRED)
        };

        public WorkItemValidator(WorkItemVM workItemVM)
        {
            _workItemVM = workItemVM;
            _workItemVM.PropertyChanged += WorkItemVM_PropertyChanged;
        }

        private void WorkItemVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case (nameof(WorkItemVM.Title)):
                    AssertIsRequired(e.PropertyName, _workItemVM.Title);
                    break;
            }
        }

        private void AssertIsRequired(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
                _workItemVM[propertyName] = _messages
                    .First(m => string.Equals(propertyName, m.Item1, StringComparison.OrdinalIgnoreCase) && string.Equals(MSG_CD_REQUIRED, m.Item2, StringComparison.OrdinalIgnoreCase))
                    .Item3;
            else
                _workItemVM[propertyName] = string.Empty;

        }
    }
}
