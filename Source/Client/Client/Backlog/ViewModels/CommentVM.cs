using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class CommentVM : ViewModelBase
    {
        private readonly Comment _innerComment;
        private string _createUser;

        public CommentVM(Comment innerComment)
        {
            _innerComment = innerComment;
        }

        public Comment InnerComment => _innerComment;
        public DateTime? CreateTimestamp => _innerComment.CreateTimestamp;
        public Guid? CreateUserId => _innerComment.CreateUserId;

        public string Text
        {
            get => _innerComment.Text;
            set
            {
                if (_innerComment.Text != value)
                {
                    _innerComment.Text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string CreateUser
        {
            get => _createUser;
            set
            {
                if (_createUser != value)
                {
                    _createUser = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
