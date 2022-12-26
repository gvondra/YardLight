using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.Board.ViewModels;
using YardLight.Interface.Models;

namespace YardLight.Client.Board.Behaviors
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

        private void Layout()
        {
            if (_board.WorkItems != null && _board.WorkItems.Count > 0)
            {
                List<object> allItems = new List<object>();
                int i = 0;
                WorkItemType type = _board.WorkItems.SelectMany(itm => itm.Children).FirstOrDefault()?.InnerWorkItem?.Type;
                _board.ColumnCount = type?.Statuses?.Count ?? 0;
                Dictionary<Guid, int> columnIndexLookup = new Dictionary<Guid, int>();
                if (type?.Statuses != null)
                {
                    int column = 0;
                    foreach (WorkItemStatus status in type.Statuses)
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
                            allItems.Add(new WorkItemChildrenVM()
                            {
                                Children = new ReadOnlyCollection<WorkItemVM>(group.ToList()),
                                RowIndex = i,
                                ColumnIndex = columnIndexLookup[group.Key]
                            });
                        }
                    }
                }
                _board.AllBoardItems = new ReadOnlyCollection<object>(allItems);
            }
        }
    }
}
