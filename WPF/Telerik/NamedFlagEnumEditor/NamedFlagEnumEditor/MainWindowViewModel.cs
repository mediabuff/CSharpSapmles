using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Telerik.Windows.Controls;

namespace NamedFlagEnumEditor
{
    [Flags]
    public enum TestFlags
    {
        [Display(Name =  "なし")]
        None = 0,
        [Display(Name = "テストフラグ1")]
        Test1 = 1 << 0,
        [Display(Name = "テストフラグ2")]
        Test2 = 1 << 1,
    }


    public class TreeItemViewModel : ViewModelBase
    {
        TestFlags _flag = TestFlags.None;
        String _header = "Header";
        Boolean _selected;
        MainWindowViewModel _owner;


        public TestFlags Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                RaisePropertyChanged();
            }
        }

        public String Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged();
            }
        }

        public Boolean Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RaisePropertyChanged();
            }
        }


        public TreeItemViewModel(MainWindowViewModel owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            PropertyChanged +=
                (_, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(Selected):
                            _owner.TreeItemSelectionChanged();
                            break;
                    }
                };
        }
    }


    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<TreeItemViewModel> TreeItems { get; } = new ObservableCollection<TreeItemViewModel>();

        public IEnumerable<TreeItemViewModel> SelectedTreeItems
        {
            get { return TreeItems.Where(x => x.Selected); }
        }

        public void TreeItemSelectionChanged()
        {
            RaisePropertyChanged(nameof(SelectedTreeItems));
        }


        public MainWindowViewModel()
        {
            TreeItems.Add(new TreeItemViewModel(this));
            TreeItems.Add(new TreeItemViewModel(this));
            TreeItems.Add(new TreeItemViewModel(this));
        }
    }
}
