using DialogServiceInjectionModule;
using Prism.Commands;
using System.Windows.Input;

namespace Module2
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
                            dialogService?.ShowMessage("Module2");
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
