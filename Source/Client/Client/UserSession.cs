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

        public Guid? OpenProjectId 
        { 
            get => _openProjectId;
            set
            {
                _openProjectId = value;
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
