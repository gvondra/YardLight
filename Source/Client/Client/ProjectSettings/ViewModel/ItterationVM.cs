using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.Client.ProjectSettings.Behaviors;
using YardLight.Interface.Models;

namespace YardLight.Client.ProjectSettings.ViewModel
{
    public class ItterationVM : ViewModelBase
    {
        private readonly Itteration _innerItteration;
        private bool _saveButtonEnabled = true;
        private SaveItterationCommand _saveItterationCommand;

        public Itteration InnerItteration => _innerItteration;

        public ItterationVM(Itteration itteration)
        {
            _innerItteration = itteration;
        }

        public bool Virtual
        {
            get => _innerItteration.Virtual ?? false;
            set
            {
                if (!_innerItteration.Virtual.HasValue || _innerItteration.Virtual.Value != value)
                {
                    _innerItteration.Virtual = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public SaveItterationCommand SaveItterationCommand
        {
            get => _saveItterationCommand;
            set
            {
                if (_saveItterationCommand != value)
                {
                    _saveItterationCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set
            {
                if (_saveButtonEnabled != value)
                {
                    _saveButtonEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _innerItteration.Name;
            set
            {
                if (_innerItteration.Name != value)
                {
                    _innerItteration.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? Start
        {
            get => _innerItteration.Start;
            set
            {
                if (_innerItteration.Start != value)
                {
                    _innerItteration.Start = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? End
        {
            get => _innerItteration.End;
            set
            {
                if (_innerItteration.End != value)
                {
                    _innerItteration.End = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Hidden
        {
            get => _innerItteration.Hidden ?? false;
            set
            {
                if (!_innerItteration.Hidden.HasValue || _innerItteration.Hidden.Value != value)
                {
                    _innerItteration.Hidden = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? CreateTimestamp => _innerItteration.CreateTimestamp;
        public DateTime? UpdateTimestamp => _innerItteration.UpdateTimestamp;

        public static ItterationVM Create(Itteration itteration)
        {
            ItterationVM result = new ItterationVM(itteration);
            result.SaveItterationCommand = new SaveItterationCommand(result);
            result.AddBehavior(new ItterationValidator(result));
            return result;
        }
    }
}
