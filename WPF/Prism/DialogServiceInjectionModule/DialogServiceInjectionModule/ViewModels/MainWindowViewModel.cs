using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace DialogServiceInjectionModule
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        IDialogService dialogService;
        DelegateCommand showMessageCommand;

        /// <summary>
        /// メッセージを表示するコマンド
        /// </summary>
        public ICommand ShowMessageCommand
        {
            get {
                if(showMessageCommand is null)
                {
                    showMessageCommand = new DelegateCommand(
                        () =>
                        {
                            dialogService?.ShowMessage("Hello Prism");
                        });
                }
                return showMessageCommand;
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }
    }
}
