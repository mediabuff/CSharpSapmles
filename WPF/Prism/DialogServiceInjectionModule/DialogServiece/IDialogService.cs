using System;

namespace DialogServiceInjectionModule
{
    /// <summary>
    /// ダイアログ表示サービスインターフェース
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// メッセージを表示する
        /// </summary>
        void ShowMessage(String msg);
    }
}
