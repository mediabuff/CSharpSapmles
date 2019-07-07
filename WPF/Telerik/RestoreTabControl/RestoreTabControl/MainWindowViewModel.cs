using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestoreTabControl
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        ObservableCollection<TabItemViewModel> _tabItems = new ObservableCollection<TabItemViewModel>();


        /// <summary>
        /// TabItems
        /// </summary>
        public ObservableCollection<TabItemViewModel> TabItems
        {
            get { return _tabItems; }
            set { SetProperty(ref _tabItems, value); }
        }


        /// <summary>
        /// add tab
        /// </summary>
        public ICommand AddTabItemCommand
        {
            get
            {
                if (_addTabItemCommand == null)
                {
                    _addTabItemCommand = new DelegateCommand(
                        () =>
                        {
                            TabItems.Add(new TabItemViewModel(this) { Header = $"Tab{TabItems.Count + 1}" });
                        });
                }
                return _addTabItemCommand;
            }
        }
        DelegateCommand _addTabItemCommand;

        /// <summary>
        /// remove tab.
        /// </summary>
        public DelegateCommand<TabItemViewModel> RemoveTabItemCommand
        {
            get
            {
                if (_removeTabItemCommand == null)
                {
                    _removeTabItemCommand = new DelegateCommand<TabItemViewModel>(
                        (removeItem) =>
                        {
                            TabItems.Remove(removeItem);
                        });
                }
                return _removeTabItemCommand;
            }
        }
        DelegateCommand<TabItemViewModel> _removeTabItemCommand;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            _tabItems.AddRange(
                new TabItemViewModel[]
                {
                    new TabItemViewModel(this)
                    {
                        Header = "Tab1",
                        CanClose = false,
                        CanAdd = false,
                    },
                    new TabItemViewModel(this)
                    {
                        Header = "Tab2",
                        CanClose = false,
                    },
                });
        }
    }
}
