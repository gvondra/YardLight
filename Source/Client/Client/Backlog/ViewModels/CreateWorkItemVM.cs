using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private WorkItemVM _parentWorkItem;
        private Visibility _createWorkItemVisible = Visibility.Visible;

        public CreateWorkItemVM(BacklogVM backlogVM, WorkItemVM parentWorkItem)
        {
            _backlogVM = backlogVM;
            _behaviors.Add(new CreateWorkItemValidator(this));
            _parentWorkItem = parentWorkItem;
            if (_backlogVM.AvailableTypes.Count > 0)
            {
                _selectedNewItemType = _backlogVM.AvailableTypes[0];
            }
        }

        public CreateWorkItemVM(BacklogVM backlogVM)
            : this(backlogVM, null)
        {}

        public BacklogVM BacklogVM => _backlogVM;
        public ObservableCollection<WorkItemTypeVM> AvailableTypes => _backlogVM.AvailableTypes;
        public WorkItemVM ParentWorkItem => _parentWorkItem;

        public Visibility CreateWorkItemVisible
        {
            get => _createWorkItemVisible;
            set
            {
                if (_createWorkItemVisible != value)
                {
                    _createWorkItemVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
            get
            {                
                return _selectedNewItemType;
            }
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
