using Prism.Commands;
using System.Windows.Input;

namespace DialogServiceInjection
{
    class UserControl1ViewModel
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
                            dialogService?.ShowMessage("UserControl1");
                        });
                }
                return showMessageCommand;
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserControl1ViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }
    }
}
