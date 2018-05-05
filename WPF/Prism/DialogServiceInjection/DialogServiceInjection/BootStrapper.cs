using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;

namespace DialogServiceInjection
{
    public class Bootstrapper : UnityBootstrapper
    {
        // メモ
        // ConfigureContainer→ConfigureViewModelLocator→CreateShell→CreateShellの順番で呼び出し

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
            Container.RegisterType<UserControl1ViewModel>(new InjectionFactory(c => new UserControl1ViewModel(c.Resolve<IDialogService>())));
#endif
            // UserControl2ViewModelの生成.
            // こちらはConfirmDialogを使いたいのでそちらをResolveするようにする.
            Container.RegisterType<UserControl2ViewModel>(new InjectionFactory(c => new UserControl2ViewModel(c.Resolve<IDialogService>(nameof(ConfirmDialogService)))));
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
    }
}