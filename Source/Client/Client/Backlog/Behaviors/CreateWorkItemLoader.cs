using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog.Behaviors
{
    public class CreateWorkItemLoader
    {
        private readonly CreateWorkItemVM _createWorkItemVM;

        public CreateWorkItemLoader(CreateWorkItemVM createWorkItemVM)
        {
            _createWorkItemVM = createWorkItemVM;
            SetNewWorkItemBackgroundBrush();
            _createWorkItemVM.PropertyChanged += CreateWorkItemVM_PropertyChanged;
        }

        private void CreateWorkItemVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case (nameof(CreateWorkItemVM.SelectedNewItemType)):
                    SetNewWorkItemBackgroundBrush();
                    break;
            }
        }

        private void SetNewWorkItemBackgroundBrush()
        {
            if (_createWorkItemVM.SelectedNewItemType == null)
                _createWorkItemVM.CreateWorkItemBackgroundBrush = Brushes.Transparent;
            else
            {
                BrushConverter brushConverter = new BrushConverter();
                try
                {
                    if (!string.IsNullOrEmpty(_createWorkItemVM.SelectedNewItemType.ColorCode))
                        _createWorkItemVM.CreateWorkItemBackgroundBrush = (Brush)brushConverter.ConvertFromString(_createWorkItemVM.SelectedNewItemType.ColorCode);
                    else
                        _createWorkItemVM.CreateWorkItemBackgroundBrush = Brushes.Transparent;
                }
                catch
                {
                    _createWorkItemVM.CreateWorkItemBackgroundBrush = Brushes.Transparent;
                }
            }
        }
    }
}
