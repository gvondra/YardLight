using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.Behaviors
{
    public class BoardWorkItemFilter
    {
        private readonly BoardVM _boardVM;
        private WorkItemFilterVM _workItemFilterVM;

        public BoardWorkItemFilter(BoardVM boardVM)
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
            if (string.IsNullOrEmpty(_workItemFilterVM.Itteration) 
                || (_workItemFilterVM.Itterations != null && !_workItemFilterVM.Itterations.Any(f => string.Equals(f, _workItemFilterVM.Itteration))))
            {
                _boardVM.ItterationStart = null;
                _boardVM.ItterationEnd = null;
            }
            if (!string.IsNullOrEmpty(_workItemFilterVM.Itteration))
            {
                Task.Run(() => GetItteration(_boardVM.Project, _workItemFilterVM))
                    .ContinueWith(GetItterationCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private Task<List<Itteration>> GetItteration(Project project, WorkItemFilterVM workItemFilterVM)
        {
            Task<List<Itteration>> result = null;
            if (!string.IsNullOrEmpty(workItemFilterVM.Itteration))
            {
                using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
                {
                    ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                    IItterationService itterationService = scope.Resolve<IItterationService>();
                    result = itterationService.GetByProjectId(settingsFactory.CreateApi(), project.ProjectId, workItemFilterVM.Itteration);
                }
            }
            if (result == null)
                result = Task.FromResult(new List<Itteration>());
            return result;
        }

        private async Task GetItterationCallback(Task<List<Itteration>> getItteration, object state)
        {
            try
            {
                List<Itteration> itterations = await getItteration;
                if (itterations != null && itterations.Count > 0)
                {
                    _boardVM.ItterationStart = itterations[0].Start;
                    _boardVM.ItterationEnd = itterations[0].End;
                }
                else
                {
                    _boardVM.ItterationStart = null;
                    _boardVM.ItterationEnd = null;
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }

        private async Task<List<WorkItemVM>> LoadWorkItems(Project project, WorkItemFilterVM workItemFilterVM)
        {
            using (ILifetimeScope scope = DependencyInjection.ContainerFactory.Container.BeginLifetimeScope())
            {
                ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
                IWorkItemService workItemService = scope.Resolve<IWorkItemService>();
                ISettings settings = settingsFactory.CreateApi();
                List<WorkItemVM> result = new List<WorkItemVM>();
                if (workItemFilterVM?.WorkItemTypeId != null)
                {
                    result = (await workItemService.GetByProjectIdTypeId(settings, project.ProjectId, workItemFilterVM.WorkItemTypeId.Value, workItemFilterVM.Team, workItemFilterVM.Itteration))
                        .Select(i => new WorkItemVM(i))
                        .ToList();
                    foreach (WorkItemVM item in result)
                    {                        
                        item.Children = new ReadOnlyCollection<WorkItemVM>(
                            (await workItemService.GetByParentIds(settings, project.ProjectId, item.InnerWorkItem.WorkItemId.Value))
                            .Select(i => new WorkItemVM(i)).ToList()
                            );
                    }
                }
                return result;
            }
        }

        private async Task LoadWorkItemsCallback(Task<List<WorkItemVM>> loadWorkItems, object state)
        {
            try
            {
                List<WorkItemVM> workItems = await loadWorkItems;
                WorkItemLoader workItemLoader;
                foreach (WorkItemVM item in workItems)
                {
                    workItemLoader = new WorkItemLoader(item);
                    item.AddBehavior(workItemLoader);
                    workItemLoader.Load();
                    if (item.Children != null)
                    {
                        foreach (WorkItemVM child in item.Children)
                        {
                            workItemLoader = new WorkItemLoader(child);
                            item.AddBehavior(workItemLoader);
                            workItemLoader.Load();
                        }
                    }
                }
                _boardVM.WorkItems = new ReadOnlyCollection<WorkItemVM>(
                    (workItems)
                    );
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, null);
            }
        }
    }
}
