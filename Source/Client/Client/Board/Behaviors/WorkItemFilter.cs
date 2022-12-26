using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.Board.ViewModels;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client.Board.Behaviors
{
    public class WorkItemFilter
    {
        private readonly BoardVM _boardVM;
        private WorkItemFilterVM _workItemFilterVM;

        public WorkItemFilter(BoardVM boardVM)
        {
            _boardVM = boardVM;
            SetWorkItemFilterVM(boardVM.Filter);
            _boardVM.PropertyChanged += BoardVM_PropertyChanged;            
        }

        private void SetWorkItemFilterVM(WorkItemFilterVM workItemFilterVM) 
        {
            if (_workItemFilterVM != null)
                _workItemFilterVM.PropertyChanged -= Filter_PropertyChanged;
            _workItemFilterVM = _boardVM.Filter;
            if (_workItemFilterVM != null)
                _workItemFilterVM.PropertyChanged += Filter_PropertyChanged;
        }

        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BoardVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BoardVM.Filter):
                    SetWorkItemFilterVM(_boardVM.Filter);
                    ApplyFilter();
                    break
                    ;
            }
        }

        private void ApplyFilter()
        {
            _boardVM.WorkItems = new ReadOnlyCollection<WorkItemVM>(new List<WorkItemVM>());
            Task.Run(() => LoadWorkItems(_boardVM.Project, _workItemFilterVM))
                .ContinueWith(LoadWorkItemsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task<List<WorkItem>> LoadWorkItems(Project project, WorkItemFilterVM workItemFilterVM)
        {
            using(ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                ISettings settings = settingsFactory.CreateApi();
                if (workItemFilterVM?.WorkItemTypeId != null)
                    return workItemService.GetByProjectIdTypeId(settings, project.ProjectId, workItemFilterVM.WorkItemTypeId.Value, workItemFilterVM.Team, workItemFilterVM.Itteration);
                else
                    return Task.FromResult(new List<WorkItem>());
            }
        }

        private async Task LoadWorkItemsCallback(Task<List<WorkItem>> loadWorkItems, object state)
        {
            try
            {
                _boardVM.WorkItems = new ReadOnlyCollection<WorkItemVM>(
                    (await loadWorkItems).Select(i => new WorkItemVM(i)).ToList()
                    );
            }
            catch (System.Exception ex) 
            {
                ErrorWindow.Open(ex, null);
            }
        }
    }
}
