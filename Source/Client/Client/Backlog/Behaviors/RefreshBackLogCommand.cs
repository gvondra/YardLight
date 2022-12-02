using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YardLight.Client.Backlog.ViewModels;

namespace YardLight.Client.Backlog.Behaviors
{
    public class RefreshBackLogCommand : ICommand
    {
        private readonly BacklogVM _backlog;
        private readonly BacklogVMLoader _backlogLoader;
        private bool _canRefresh = false;

        public RefreshBackLogCommand(BacklogVM backlog, BacklogVMLoader backlogLoader)
        {
            _backlog = backlog;
            _backlogLoader = backlogLoader;
            _canRefresh = _backlog.CanRefresh;
            _backlog.PropertyChanged += Backlog_PropertyChanged;
        }

        private void Backlog_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BacklogVM.CanRefresh))
            {
                _canRefresh = _backlog.CanRefresh;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canRefresh;

        public void Execute(object parameter)
        {
            _backlogLoader.Load();
        }
    }
}
