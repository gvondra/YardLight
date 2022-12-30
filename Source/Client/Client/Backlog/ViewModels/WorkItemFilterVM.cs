using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class WorkItemFilterVM : ViewModelBase
    {
        private readonly UserSession _userSession;
        private WorkItemType _workItemType;
        private string _title;
        private List<string> _itterations = new List<string>();
        private List<string> _teams = new List<string>();
        private List<WorkItemType> _workItemTypes = new List<WorkItemType>();

        public WorkItemFilterVM(UserSession userSession)
        {
            _userSession = userSession;
        }

        public List<string> Itterations
        {
            get => _itterations;
            set
            {
                if (_itterations != value)
                {
                    _itterations = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<string> Teams
        {
            get => _teams;
            set
            {
                if (_teams != value)
                {
                    _teams = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Team
        {
            get => _userSession.BacklogFilterTeam;
            set
            {
                if (_userSession.BacklogFilterTeam != value)
                {
                    _userSession.BacklogFilterTeam = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Itteration
        {
            get => _userSession.BacklogFilterItteration;
            set
            {
                if (_userSession.BacklogFilterItteration != value)
                {
                    _userSession.BacklogFilterItteration = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<WorkItemType> WorkItemTypes
        {
            get => _workItemTypes;
            set
            {
                if (_workItemTypes != value)
                {
                    _workItemTypes = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public WorkItemType WorkItemType
        {
            get => _workItemType;
            set
            {
                if (_workItemType != value)
                {
                    WorkItemTypeId = value?.WorkItemTypeId;
                    _workItemType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Guid? WorkItemTypeId
        {
            get => _userSession.BacklogWorkItemTypeId;
            set
            {
                if (_userSession.BacklogWorkItemTypeId.HasValue != value.HasValue || (value.HasValue && !_userSession.BacklogWorkItemTypeId.Value.Equals(value.Value)))
                {
                    _userSession.BacklogWorkItemTypeId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public void CopyFrom(WorkItemFilterVM source)
        //{
        //    Title = source.Title;
        //}

    }
}
