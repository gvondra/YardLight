﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using YardLight.Client.Backlog.Behaviors;
using YardLight.Interface.Models;

namespace YardLight.Client.Backlog.ViewModels
{
    public class BacklogVM : ViewModelBase
    {
        private readonly CreateWorkItemVM _createWorkItemVM;
        private readonly ObservableCollection<WorkItemTypeVM> _availableTypes = new ObservableCollection<WorkItemTypeVM>();
        private readonly ObservableCollection<WorkItemVM> _rootWorkItems = new ObservableCollection<WorkItemVM>();
        private Project _project;

        public BacklogVM()
        {
            _createWorkItemVM = new CreateWorkItemVM(this);
        }

        public CreateWorkItemVM CreateWorkItemVM => _createWorkItemVM;
        public ObservableCollection<WorkItemTypeVM> AvailableTypes => _availableTypes;
        public ObservableCollection<WorkItemVM> RootWorkItems => _rootWorkItems;
        
        public Project Project
        {
            get => _project;
            set
            {
                if (_project != value)
                {
                    _project = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void AddBehavior(object behavior) => _behaviors.Add(behavior);
    }
}