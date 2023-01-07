using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client
{
    public sealed class UserSession
    {
        private bool _enableSave = false;
        private Guid? _openProjectId;
        private string _backlogFilterTeam;
        private string _backlogFilterItteration;
        private string _boardFilterTeam;
        private string _boardFilterItteration;
        private Guid? _boardWorkItemTypeId;
        private Guid? _backlogWorkItemTypeId;

        public string BacklogFilterTeam
        {
            get => _backlogFilterTeam;
            set
            {
                _backlogFilterTeam = value;
                Save();
            }
        }

        public string BacklogFilterItteration
        {
            get => _backlogFilterItteration;
            set
            {
                _backlogFilterItteration = value;
                Save();
            }
        }

        public string BoardFilterTeam
        {
            get => _boardFilterTeam;
            set
            {
                _boardFilterTeam = value;
                Save();
            }
        }

        public Guid? BoardWorkItemTypeId
        {
            get => _boardWorkItemTypeId;
            set
            {
                _boardWorkItemTypeId = value;
                Save();
            }
        }

        public Guid? BacklogWorkItemTypeId
        {
            get => _backlogWorkItemTypeId;
            set
            {
                _backlogWorkItemTypeId = value;
                Save();
            }
        }

        public string BoardFilterItteration
        {
            get => _boardFilterItteration;
            set
            {
                _boardFilterItteration = value;
                Save();
            }
        }

        public Guid? OpenProjectId 
        { 
            get => _openProjectId;
            set
            {
                _openProjectId = value;
                if (_enableSave)
                {
                    _backlogFilterTeam = string.Empty;
                    _backlogFilterItteration = string.Empty;
                }
                Save();
            }
        }

        private void Save()
        {
            // the auto save is disabled while loading the instance
            if (_enableSave)
                UserSessionLoader.SaveUserSession(this);
        }
        
        public void EnableSave() => _enableSave = true;
    }
}
