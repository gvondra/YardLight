using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.Authorization.ViewModel;

namespace YardLight.Client.Authorization
{
    public class RoleValidator
    {
        private const string MSG_CODE_REQUIRED = "required";
        private const string MSG_TXT_REQUIRED = " is required";
        private readonly Dictionary<string, Dictionary<string, string>> _messages = new Dictionary<string, Dictionary<string, string>>
        {
            {
                nameof(RoleVM.Name),
                new Dictionary<string, string>
                {
                    { MSG_CODE_REQUIRED, MSG_TXT_REQUIRED }
                }
            },
            {
                nameof(RoleVM.PolicyName),
                new Dictionary<string, string>
                {
                    { MSG_CODE_REQUIRED, MSG_TXT_REQUIRED }
                }
            }
        };

        private readonly RoleVM _roleVM;

        public RoleValidator(RoleVM roleVM)
        {
            _roleVM = roleVM;
            _roleVM.PropertyChanged += OnProperyChanged;
        }

        public void OnProperyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(RoleVM.Name):
                    if (string.IsNullOrEmpty(_roleVM.Name))
                        _roleVM[e.PropertyName] = _messages[e.PropertyName][MSG_CODE_REQUIRED];
                    break;
                case nameof(RoleVM.PolicyName):
                    if (string.IsNullOrEmpty(_roleVM.PolicyName))
                        _roleVM[e.PropertyName] = _messages[e.PropertyName][MSG_CODE_REQUIRED];
                    break;
            }
        }
    }
}
