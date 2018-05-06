using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Windows.Input;

namespace DialogServiceInjectionModule
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        IDialogService dialogService;
        IModuleManager moduleManager;
        IUnityContainer container;
        DelegateCommand loadModuleCommand;
        object[] moduleViews;

        /// <summary>
        /// moduleのview
        /// </summary>
        public object[] ModuleViews {
            get { return moduleViews; }
            private set { SetProperty(ref moduleViews, value); }
        }

        /// <summary>
        /// モジュールをロードするコマンド
        /// </summary>
        public ICommand LoadModuleCommand
        {
            get
            {
                if (loadModuleCommand is null)
                {
                    loadModuleCommand = new DelegateCommand(
                        () =>
                        {
                            // 手動でモジュールを読み込み
                            moduleManager?.LoadModule(typeof(Module1.Module).FullName);
                            moduleManager?.LoadModule(typeof(Module2.Module).FullName);

                            // Itemsを更新.
                            ModuleViews = container?.ResolveAll<object>().ToArray();
                        });
                }
                return loadModuleCommand;
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel(IDialogService dialogService, IModuleManager moduleManager, IUnityContainer container)
        {
            if (dialogService is null)
                throw new ArgumentNullException(nameof(dialogService));
            if (moduleManager is null)
                throw new ArgumentNullException(nameof(moduleManager));
            if (container is null)
                throw new ArgumentNullException(nameof(container));

            this.dialogService = dialogService;
            this.moduleManager = moduleManager;
            this.container = container;
        }
    }
}
