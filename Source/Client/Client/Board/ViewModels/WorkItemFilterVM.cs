using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Board.ViewModels
{
    public class WorkItemFilterVM : ViewModelBase
    {
        private readonly UserSession _userSession;
        private WorkItemType _workItemType;
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

        public string Team
        {
            get => _userSession.BoardFilterTeam;
            set
            {
                if (_userSession.BoardFilterTeam != value)
                {
                    _userSession.BoardFilterTeam = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Itteration
        {
            get => _userSession.BoardFilterItteration;
            set
            {
                if (_userSession.BoardFilterItteration != value)
                {
                    _userSession.BoardFilterItteration = value;
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
            get => _userSession.BoardWorkItemTypeId;
            set
            {
                if (_userSession.BoardWorkItemTypeId.HasValue != value.HasValue || (value.HasValue && !_userSession.BoardWorkItemTypeId.Value.Equals(value.Value)))
                {
                    _userSession.BoardWorkItemTypeId = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
