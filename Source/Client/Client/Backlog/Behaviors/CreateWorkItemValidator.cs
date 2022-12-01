using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog.Behaviors
{
    public class CreateWorkItemValidator
    {
        private readonly CreateWorkItemVM _createWorkItemVM;

        public CreateWorkItemValidator(CreateWorkItemVM createWorkItemVM)
        {
            _createWorkItemVM = createWorkItemVM;
            _createWorkItemVM.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case (nameof(CreateWorkItemVM.NewItemTitle)):
                    ValidateNewWorkItemTitle(_createWorkItemVM.NewItemTitle);
                    break;
            }
        }

        private void ValidateNewWorkItemTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                _createWorkItemVM.CreateWorkItemButtonEnabled = false;
            else
                _createWorkItemVM.CreateWorkItemButtonEnabled = true;
        }
    }
}
