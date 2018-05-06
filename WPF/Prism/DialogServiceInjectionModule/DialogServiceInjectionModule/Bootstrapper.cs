using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;

namespace DialogServiceInjectionModule
{
    public class Bootstrapper : UnityBootstrapper
    {
        // メモ
        // ConfigureContainer→ConfigureViewModelLocator→CreateShell→InitializeShellの順番で呼び出し

        /// <summary>
        /// shellの生成
        /// </summary>
        protected override DependencyObject CreateShell()
        {
            // 最初に表示するviewを生成(このプロジェクトではMainWindow)
            return this.Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// shellの初期化
        /// </summary>
        protected override void InitializeShell()
        {
            // shellを表示する
            ((Window)this.Shell).Show();
        }

        /// <summary>
        /// Containerの設定
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // これは不要、最初から登録されている.
            //Container.RegisterInstance<IUnityContainer>(Container);

            // DialogServiceをDIコンテナに登録.
            // ContainerControlledLifetimeManagerはシングルトン.
            // デフォルトのPerResolveLifetimeManagerは毎回newする.
            // 今回はどちらでも良いのでシングルトンにしてあります.
            Container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            // 同じインターフェースで別実装を使う場合は名前付きを使用する.
            Container.RegisterType<IDialogService, ConfirmDialogService>(nameof(ConfirmDialogService), new ContainerControlledLifetimeManager());

            // UserControl1ViewModelの生成.
            // デフォルトのDialogServiceが使われるため定義しなくてもよい.
#if false
            Container.RegisterType<Module1.ModuleViewModel>(new InjectionFactory(c => new Module1.ModuleViewModel(c.Resolve<IDialogService>())));
#endif
            // UserControl2ViewModelの生成.
            // こちらはConfirmDialogを使いたいのでそちらをResolveするようにする.
            Container.RegisterType<Module2.ModuleViewModel>(new InjectionFactory(c => new Module2.ModuleViewModel(c.Resolve<IDialogService>(nameof(ConfirmDialogService)))));
        }

        /// <summary>
        /// ViewModelLocatorの設定
        /// 現状のデフォルト設定ならこの関数自体定義しなくても実行できる
        /// 基本はViewModelが命名規則に沿っていない場合に使うらしい
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory(type => Container.Resolve(type));

            //!@note ResolveOverrideの使い方の備忘録として残しておく
#if false
            ViewModelLocationProvider.Register<UserControl2>(
                () =>
                {
                    // UserControl2ではConfirmDialogServiceを使う.
                    // コンストラクタインジェクションの場合はParameterOverrideで引数名と値を渡す.
                    // ただし、ResolverOverride自体あまり行儀が良くないらしいのでContainer.Resolveの名前付きで対応するのが好ましいとのこと.
                    var vm = Container.Resolve<UserControl2ViewModel>(new ParameterOverride("dialogService", Container.Resolve<IDialogService>("ConfirmDialog")));
                    return vm;
                });
#endif
        }

        /// <summary>
        /// Moduleの設定
        /// </summary>
        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            // ModuleCatalogに今回使うModuleを手動で登録.
            // 手動で読み込みを行うためInitializationMode.OnDemandを設定しておく.
            var catalog = (ModuleCatalog)this.ModuleCatalog;
            catalog.AddModule(typeof(Module1.Module).FullName, typeof(Module1.Module).AssemblyQualifiedName, InitializationMode.OnDemand);
            catalog.AddModule(typeof(Module2.Module).FullName, typeof(Module2.Module).AssemblyQualifiedName, InitializationMode.OnDemand);
        }
    }
}