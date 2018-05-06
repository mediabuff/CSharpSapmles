using DialogServiceInjectionModule;
using Prism.Commands;
using System.Windows.Input;

namespace Module1
{
    public class ModuleViewModel
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
                            dialogService?.ShowMessage("Module1");
                        });
                }
                return showMessageCommand;
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModuleViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }
    }
}
