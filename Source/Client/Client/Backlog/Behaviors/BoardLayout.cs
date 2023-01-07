using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YardLight.Client.Backlog.ViewModels;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.Behaviors
{
    public class BoardLayout
    {
        private readonly BoardVM _board;

        public BoardLayout(BoardVM board)
        {
            _board = board;
            _board.PropertyChanged += Board_PropertyChanged;
        }

        private void Board_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BoardVM.WorkItems):
                    Layout();
                    break;
            }
        }

        public void Layout()
        {
            List<object> allItems = new List<object>();
            if (_board.WorkItems != null && _board.WorkItems.Count > 0)
            {
                int i = 0;
                WorkItemType type = _board.WorkItems.SelectMany(itm => itm.Children).FirstOrDefault()?.InnerWorkItem?.Type;
                _board.ColumnCount = type?.Statuses?.Where(s => !(s.IsDefaultHidden ?? false))?.Count() ?? 0;
                Dictionary<Guid, int> columnIndexLookup = new Dictionary<Guid, int>();
                if (type?.Statuses != null)
                {
                    int column = 0;
                    foreach (WorkItemStatus status in type.Statuses.Where(s => !(s.IsDefaultHidden ?? false)))
                    {
                        column += 1;
                        allItems.Add(new BoardColumnHeaderVM()
                        {
                            Id = status.WorkItemStatusId,
                            Title = status.Title,
                            RowIndex = 0,
                            ColumnIndex = column
                        });
                        columnIndexLookup.Add(status.WorkItemStatusId.Value, column);
                    }
                }
                allItems.AddRange(_board.WorkItems);
                foreach (WorkItemVM workItemVM in _board.WorkItems)
                {
                    i += 1;
                    workItemVM.RowIndex = i;
                    workItemVM.ColumnIndex = 0;
                    if (workItemVM.Children != null)
                    {
                        foreach (IGrouping<Guid, WorkItemVM> group in workItemVM.Children.GroupBy(itm => itm.InnerWorkItem.Status.WorkItemStatusId.Value))
                        {
                            if (columnIndexLookup.ContainsKey(group.Key))
                            {
                                allItems.Add(new WorkItemChildrenVM()
                                {
                                    Children = new ReadOnlyCollection<WorkItemVM>(group.ToList()),
                                    RowIndex = i,
                                    ColumnIndex = columnIndexLookup[group.Key],
                                    WorkItemStatusId = group.Key,
                                    ParentWorkItemId = workItemVM.WorkItemId.Value
                                });
                                AddPropertyChangeHandlers_SetStatusVisibility(group);
                            }
                        }
                    }
                }
            }
            _board.AllBoardItems = new ReadOnlyCollection<object>(allItems);
        }

        private void AddPropertyChangeHandlers_SetStatusVisibility(IEnumerable<WorkItemVM> workItemVMs)
        {
            foreach (WorkItemVM workItemVM in workItemVMs)
            {
                workItemVM.StatusVisibility = Visibility.Collapsed;
                workItemVM.PropertyChanged += WorkItemVM_PropertyChanged;
            }
        }

        private void WorkItemVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(WorkItemVM.Status):
                    UpdatedStatus((WorkItemVM)sender);
                    break;
            }
        }

        private void UpdatedStatus(WorkItemVM workItemVM)
        {
            bool found = false;
            foreach (WorkItemChildrenVM child in _board.AllBoardItems
                .Where(i => i.GetType().Equals(typeof(WorkItemChildrenVM)))
                .Select(i => (WorkItemChildrenVM)i)
                .Where(i => i.ParentWorkItemId.Equals(workItemVM.ParentWorkItemId.Value))
                )
            {
                IEnumerable<WorkItemVM> newChildern = child.Children.Where(i => !i.WorkItemId.Value.Equals(workItemVM.WorkItemId.Value));
                if (child.WorkItemStatusId.Equals(workItemVM.Status.WorkItemStatusId))
                {
                    child.Children = new ReadOnlyCollection<WorkItemVM>(
                        newChildern.Concat(new WorkItemVM[] { workItemVM }).ToList()
                        );
                    found = true;
                }
                else
                {
                    child.Children = new ReadOnlyCollection<WorkItemVM>(
                        newChildern.ToList()
                        );
                }
            }
            if (!found)
            {
                AddChild(workItemVM);
            }
        }

        private void AddChild(WorkItemVM childWorkItemVM)
        {
            int? columnIndex = FindStatusColumnIndex(childWorkItemVM.Status.WorkItemStatusId);
            if (columnIndex.HasValue)
            {
                WorkItemChildrenVM[] workItemChildrenVMs = new WorkItemChildrenVM[]
                {
                new WorkItemChildrenVM
                {
                Children = new ReadOnlyCollection<WorkItemVM>(new WorkItemVM[] { childWorkItemVM }),
                RowIndex = FindParentRowIndex(childWorkItemVM.ParentWorkItemId.Value),
                ColumnIndex = columnIndex.Value,
                WorkItemStatusId = childWorkItemVM.Status.WorkItemStatusId,
                ParentWorkItemId = childWorkItemVM.ParentWorkItemId.Value
             }
                };
                _board.AllBoardItems = new ReadOnlyCollection<object>(
                    _board.AllBoardItems.Concat(workItemChildrenVMs).ToList()
                    );
            }
        }

        private int FindParentRowIndex(Guid workItemId)
        {
            return _board.AllBoardItems
                .Where(i => i.GetType().Equals(typeof(WorkItemVM)))
                .Select(i => (WorkItemVM)i)
                .FirstOrDefault(i => i.WorkItemId.Value.Equals(workItemId))
                .RowIndex;
        }

        private int? FindStatusColumnIndex(Guid statusId)
        {
            return _board.AllBoardItems
                .Where(i => i.GetType().Equals(typeof(BoardColumnHeaderVM)))
                .Select(i => (BoardColumnHeaderVM)i)
                .FirstOrDefault(i => i.Id.Equals(statusId))?.ColumnIndex;
        }
    }
}
