using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestoreTabControl
{
    /// <summary>
    /// TabItemViewModel
    /// </summary>
    public class TabItemViewModel : BindableBase
    {
        MainWindowViewModel _owner;


        /// <summary>
        /// tab header
        /// </summary>
        public String Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }
        String _header = "";

        /// <summary>
        /// Selected
        /// </summary>
        public Boolean IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        Boolean _isSelected = false;

        /// <summary>
        /// can add
        /// </summary>
        public Boolean CanAdd
        {
            get { return _canAdd; }
            set { SetProperty(ref _canAdd, value); }
        }
        Boolean _canAdd = true;

        /// <summary>
        /// can close
        /// </summary>
        public Boolean CanClose
        {
            get { return _canClose; }
            set { SetProperty(ref _canClose, value); }
        }
        Boolean _canClose = true;


        /// <summary>
        /// add tab
        /// </summary>
        public ICommand AddItemCommand { get { return _owner.AddTabItemCommand; } }

        /// <summary>
        /// remove tab.
        /// </summary>
        public ICommand RemoveItemCommand
        {
            get
            {
                if (_removeItemCommand == null)
                {
                    _removeItemCommand = new DelegateCommand(
                        () =>
                        {
                            _owner.RemoveTabItemCommand.Execute(this);
                        });
                }
                return _removeItemCommand;
            }
        }
        DelegateCommand _removeItemCommand;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TabItemViewModel(MainWindowViewModel owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
    }
}
