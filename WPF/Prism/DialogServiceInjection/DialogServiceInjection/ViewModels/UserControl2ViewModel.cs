using Prism.Commands;
using System.Windows.Input;

namespace DialogServiceInjection
{
    class UserControl2ViewModel
    {
        IDialogService dialogService;
        DelegateCommand showMessageCommand;

        /// <summary>
        /// メッセージを表示するコマンド
        /// </summary>
        public ICommand ShowMessageCommand
        {
            get
            {
                if (showMessageCommand is null)
                {
                    showMessageCommand = new DelegateCommand(
                        () =>
                        {
                            dialogService?.ShowMessage("UserControl2");
                        });
                }
                return showMessageCommand;
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserControl2ViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }
    }
}
