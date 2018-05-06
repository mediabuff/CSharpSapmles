using Microsoft.Practices.Unity;
using System.Windows;

namespace DialogServiceInjectionModule
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Unityコンテナ
        /// </summary>
        IUnityContainer Container { get; } = new UnityContainer();
        /// <summary>
        /// DialogService
        /// </summary>
        IDialogService DialogService { get; } = new DialogService();


        /// <summary>
        /// アプリケーション開始時処理
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new Bootstrapper().Run();
        }
    }
}
