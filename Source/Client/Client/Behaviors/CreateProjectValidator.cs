using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.ViewModel;

namespace YardLight.Client.Behaviors
{
    public class CreateProjectValidator
    {
        private readonly CreateProjectWindowVM _vm;

        public CreateProjectValidator(CreateProjectWindowVM vm)
        {
            _vm = vm;
            _vm.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CreateProjectWindowVM.Title):
                    if (string.IsNullOrEmpty(_vm.Title))
                        _vm[e.PropertyName] = "Title is required";
                    else
                        _vm[e.PropertyName] = string.Empty;
                    break;
            }
        }
    }
}
