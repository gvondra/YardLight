using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.Backlog.Behaviors;

namespace YardLight.Client.Backlog.ViewModels
{
    public class CreateWorkItemVM : ViewModelBase
    {
        private readonly BacklogVM _backlogVM;
        private WorkItemTypeVM _selectedNewItemType;
        private string _newItemTitle;
        private bool _createWorkItemButtonEnabled = false;
        private Brush _createWorkItemBackgroundBrush = Brushes.Transparent;
        private string _createButtonText = "Create";

        public CreateWorkItemVM(BacklogVM backlogVM)
        {
            _backlogVM = backlogVM;
            _behaviors.Add(new CreateWorkItemValidator(this));
        }

        public BacklogVM BacklogVM => _backlogVM;
        public ObservableCollection<WorkItemTypeVM> AvailableTypes => _backlogVM.AvailableTypes;

        public string CreateButtonText
        {
            get => _createButtonText;
            set
            {
                if (_createButtonText != value)
                {
                    _createButtonText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Brush CreateWorkItemBackgroundBrush
        {
            get => _createWorkItemBackgroundBrush;
            set
            {
                if (_createWorkItemBackgroundBrush != value)
                {
                    _createWorkItemBackgroundBrush = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool CreateWorkItemButtonEnabled
        {
            get => _createWorkItemButtonEnabled;
            set
            {
                if (_createWorkItemButtonEnabled != value)
                {
                    _createWorkItemButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string NewItemTitle
        {
            get => _newItemTitle;
            set
            {
                if (_newItemTitle != value)
                {
                    _newItemTitle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public WorkItemTypeVM SelectedNewItemType
        {
            get => _selectedNewItemType;
            set
            {
                if (_selectedNewItemType != value)
                {
                    _selectedNewItemType = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
