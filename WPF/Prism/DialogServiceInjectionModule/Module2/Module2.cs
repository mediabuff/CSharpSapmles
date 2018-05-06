using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Module2
{
    public class Module : IModule
    {
        /// <summary>
        /// Unityコンテナ
        /// Dependency属性をつけると勝手にインジェクションされる
        /// </summary>
        [Dependency]
        public IUnityContainer Container { get; set; }

        /// <summary>
        /// モジュール初期化処理
        /// </summary>
        public void Initialize()
        {
            Container.RegisterType<object, ModuleView>(typeof(ModuleView).FullName);
        }
    }
}