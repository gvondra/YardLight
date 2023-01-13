using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Client.ProjectSettings.Behaviors;

namespace YardLight.Client.ProjectSettings.ViewModel
{
    public class MembersVM : ViewModelBase
    {
        private readonly ObservableCollection<string> _members = new ObservableCollection<string>();
        private Visibility _busyVisibility = Visibility.Collapsed;
        private string _addMemberEmailAddress;
        private AddProjectMemberCommand _addProjectMemberCommand;
        private RemoveProjectMemberCommand _removeProjectMemberCommand;
        private Guid? _projectId;

        public ObservableCollection<string> Members => _members;

        public Guid? ProjectId
        {
            get => _projectId;
            set
            {
                if (_projectId != value)
                {
                    _projectId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public AddProjectMemberCommand AddProjectMemberCommand
        {
            get => _addProjectMemberCommand;
            set
            {
                if (_addProjectMemberCommand != value)
                {
                    _addProjectMemberCommand = value;
                    NotifyPropertyChanged(nameof(AddProjectMemberCommand));
                }
            }
        }
        public RemoveProjectMemberCommand RemoveProjectMemberCommand
        {
            get => _removeProjectMemberCommand;
            set
            {
                if (_removeProjectMemberCommand != value)
                {
                    _removeProjectMemberCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string AddMemberEmailAddress
        {
            get => _addMemberEmailAddress;
            set
            {
                if (_addMemberEmailAddress != value)
                {
                    _addMemberEmailAddress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility BusyVisibility
        {
            get => _busyVisibility;
            set
            {
                if (_busyVisibility != value)
                {
                    _busyVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public static MembersVM Create()
        {
            MembersVM membersVM = new MembersVM();
            membersVM.AddProjectMemberCommand = new AddProjectMemberCommand(membersVM);
            membersVM.RemoveProjectMemberCommand = new RemoveProjectMemberCommand(membersVM);
            return membersVM;
        }
    }
}
