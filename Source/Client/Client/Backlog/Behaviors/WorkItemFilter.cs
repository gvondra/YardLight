using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.Behaviors
{
    public class WorkItemFilter
    {
        private readonly BacklogVM _backlogVM;
        
        public WorkItemFilter(BacklogVM backlogVM)
        {
            _backlogVM = backlogVM;
            ApplyFilter();
            backlogVM.PropertyChanged += BackLog_PropertyChanged;
            backlogVM.Filter.PropertyChanged += Filter_PropertyChanged;
        }

        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BackLog_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case (nameof(BacklogVM.RootWorkItems)):
                    ApplyFilter();
                    break;
            }
        }

        public void ApplyFilter()
        {
            _backlogVM.FilteredChildren.Clear();
            foreach (WorkItemVM workItemVM in _backlogVM.RootWorkItems)
            {
                ApplyFilter(workItemVM, _backlogVM.Filter);
            }
            SetFilteredItems(_backlogVM.FilteredChildren,
                FilterItems(_backlogVM.Filter, _backlogVM.RootWorkItems)
                );
        }

        private void ApplyFilter(WorkItemVM workItemVM, WorkItemFilterVM filter)
        {
            workItemVM.FilteredChildren.Clear();
            foreach (WorkItemVM childItemVM in workItemVM.Children)
            {
                ApplyFilter(childItemVM, _backlogVM.Filter);
            }
            SetFilteredItems(workItemVM.FilteredChildren,
                FilterItems(filter, workItemVM.Children)
                );
        }

        private void SetFilteredItems(ObservableCollection<WorkItemVM> destination, IEnumerable<WorkItemVM> filteredItems)
        {
            //destination.Clear();
            foreach (WorkItemVM item in filteredItems)
            {
                destination.Add(item);
            }
        }

        private IEnumerable<WorkItemVM> FilterItems(WorkItemFilterVM filter, IEnumerable<WorkItemVM> allItems)
        {
            IEnumerable<WorkItemVM> filteredItems = allItems;
            if (!string.IsNullOrEmpty(filter.Title))
                filteredItems = filteredItems.Where(i => i.FilteredChildren.Count > 0 || StringMatch(i.Title, filter.Title));
            if (!string.IsNullOrEmpty(filter.Team))
                filteredItems = filteredItems.Where(i => i.FilteredChildren.Count > 0 || StringMatch(i.Team, filter.Team));
            if (!string.IsNullOrEmpty(filter.Itteration))
                filteredItems = filteredItems.Where(i => i.FilteredChildren.Count > 0 || StringMatch(i.Itteration, filter.Itteration));
            return filteredItems;
        }

        private static bool StringMatch(string target, string match)
        {
            match = Regex.Escape(match.Trim());
            match = Regex.Replace(match, @"(\\ )+", ".*", RegexOptions.IgnoreCase);
            target = Regex.Replace(target, @"\s+", string.Empty, RegexOptions.IgnoreCase);
            return Regex.IsMatch(target, match, RegexOptions.IgnoreCase);
        }
    }
}
