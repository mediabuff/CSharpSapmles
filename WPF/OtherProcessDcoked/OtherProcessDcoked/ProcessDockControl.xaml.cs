using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OtherProcessDcoked
{
    /// <summary>
    /// ProcessDockControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ProcessDockControl : UserControl, IDisposable
    {
        ProcessDockPanel _processDockPanel;
        String[] _processFullPaths;
        Boolean _isLoaded = false;
        Boolean _request = false;
        Process _process;
        Task _bootTask;
        CancellationTokenSource _bootCts;


        /// <summary>
        /// ProcessFullPathsのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty ProcessFullPathsProperty =
            DependencyProperty.Register(nameof(ProcessFullPaths), typeof(String[]), typeof(ProcessDockControl),
                new PropertyMetadata(
                    (obj, args) =>
                    {
                        var target = (ProcessDockControl)obj;
                        target.ProcessFullPaths = (String[])args.NewValue;
                    }));

        /// <summary>
        /// BootProcessWaitMillisecondのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty BootProcessWaitMillisecondProperty =
            DependencyProperty.Register(nameof(BootProcessWaitMillisecond), typeof(Int32), typeof(ProcessDockControl),
                new PropertyMetadata(
                    (obj, args) =>
                    {
                        var target = (ProcessDockControl)obj;
                        target.BootProcessWaitMillisecond = (Int32)args.NewValue;
                    }));


        /// <summary>
        /// ProcessFullPaths
        /// </summary>
        public IEnumerable<String> ProcessFullPaths
        {
            get { return _processFullPaths; }
            set
            {
                if (_processFullPaths != value)
                {
                    _processFullPaths = value.ToArray();
                    BootProcessAndDock();
                }
            }
        }

        /// <summary>
        /// Processの起動待ち時間
        /// </summary>
        public Int32 BootProcessWaitMillisecond { get; set; } = 30000;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProcessDockControl()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                _isLoaded = true;
                if (_request)
                {
                    BootProcessAndDock();
                }
            };
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~ProcessDockControl()
        {
            Dispose(false);
        }


        /// <summary>
        /// Processを起動してドッキングする
        /// </summary>
        private void BootProcessAndDock()
        {
            if (!_isLoaded)
            {
                _request = true;
                return;
            }

            if(ProcessFullPaths == null || !ProcessFullPaths.Any())
            {
                return;
            }

            _request = false;
            rebootButton.IsEnabled = false;

            var deadline = DateTime.Now + TimeSpan.FromSeconds(BootProcessWaitMillisecond);

            // _taskが実行中ならCancelしておく
            CancelBootTask(deadline);

            // 以前のProcessをKill
            KillProcess();

            // プロセス実行&ドッキングタスク実行
            _bootCts = new CancellationTokenSource();
            _bootTask = Task.Run(
                async () =>
                {
                    try
                    {
                        var processFullPaths = new String[] { };
                        lock (_processFullPaths)
                        {
                            processFullPaths = ProcessFullPaths.ToArray();
                        }

                        Process newProcess  = null;
                        var newProcessPath = "";

                        // processFullPathsを上から順に起動してみる
                        foreach (var path in processFullPaths)
                        {
                            var psi = new ProcessStartInfo(path);
                            try
                            {
                                newProcess = Process.Start(psi);
#if false
                                // こちらじゃないと完全に待機できないアプリケーションがある.
                                // ただ、そういう場合はだいたいMainWindowHandleもZeroが返るので無視.
                                newProcess.WaitForInputIdle();
#else
                                while (!newProcess.HasExited && (deadline < DateTime.Now))
                                {
                                    newProcess.WaitForInputIdle(100);
                                    _bootCts.Token.ThrowIfCancellationRequested();
                                }
#endif
                            }
                            catch (Exception)
                            {
                                if (newProcess != null && !newProcess.HasExited)
                                {
                                    newProcess.Kill();
                                }
                                newProcess = null;
                            }

                            // 正しく起動した場合はループ終了.
                            if (newProcess != null) {
                                newProcess.Refresh();
                                newProcessPath = path;
                                break;
                            }
                        }

                        _bootCts.Token.ThrowIfCancellationRequested();

                        // 起動したProcessをDockする.
                        if (newProcess != null && !newProcess.HasExited)
                        {
                            // MainWindowHandleが生成されるまで待機
                            // GUIを持たないWindowやタスクバーに表示されないWindowは常にIntPre.Zeroなので注意
                            // Chrome等のマルチプロセスアプリケーションも常にZero(そもそもHasExitedもtrueになる)になるようです.
                            // https://stackoverflow.com/questions/22314315/setwindowpos-not-working-for-browsers-no-mainwindowhandle?rq=1
                            // 電卓等のUWPアプリも常にZeroとのこと...
                            while (newProcess.MainWindowHandle == IntPtr.Zero)
                            {
                                await Task.Delay(100);

                                // Timeoutチェック.
                                if (deadline < DateTime.Now)
                                {
                                    throw new TimeoutException("プロセス起動待ちがタイムアウトしました");
                                }
                                // プロセスチェック
                                if (newProcess == null)
                                {
                                    throw new OperationCanceledException("起動したプロセスが終了しました");
                                }
                                if (newProcess.HasExited)
                                {
                                    foreach (Process current in Process.GetProcessesByName(Path.GetFileName(newProcessPath)))
                                    {
                                        if ((current.Id == newProcess.Id) && !current.HasExited)
                                            throw new Exception("Oh oh!");
                                    }
                                }
                                newProcess.Refresh();

                                _bootCts.Token.ThrowIfCancellationRequested();
                            }

                            _bootCts.Token.ThrowIfCancellationRequested();

                            // DockPanelの生成.
                            await Dispatcher.BeginInvoke(new Action(
                                () =>
                                {
                                    CreateProcessDockPanel(newProcess);
                                    bootProcessPathTextBox.Text = newProcessPath;
                                    bootProcessPathTextBox.ToolTip = newProcessPath;
                                }));
                        }

                        if(newProcess == null)
                        {
                            Trace.Write("プロセスは起動しませんでした");
                        }

                        Trace.Assert(_process == null);
                        if (_process == null) {
                            _process = newProcess;
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);
                        MessageBox.Show($"プロセスの起動に失敗しました\n{e}", "プロセス起動失敗");
                    }
                    finally
                    {
                        // 再起動ボタンの有効化.
                        await Dispatcher.BeginInvoke(new Action(
                            () =>
                            {
                                rebootButton.IsEnabled = true;
                            }));
                    }
                }, _bootCts.Token);
        }

        /// <summary>
        /// ProcessをKillする
        /// </summary>
        private void KillProcess()
        {
            try
            {
                if (_process != null)
                {
                    lock (_process)
                    {
                        if (!_process.HasExited)
                        {
                            _process.Kill();
                        }
                        _process = null;
                    }
                }
            }
            catch (Exception e) { Trace.Write(e); }
        }

        /// <summary>
        /// BootTaskのキャンセル
        /// </summary>
        private void CancelBootTask(DateTime deadline)
        {
            if(_bootTask != null && !_bootTask.IsCompleted)
            {
                Trace.Assert(_bootCts != null);
                if (_bootCts != null)
                {
                    _bootCts.Cancel();

                    while(_bootTask != null && (!_bootTask.IsCanceled || !_bootTask.IsCompleted))
                    {
                        Thread.Sleep(100);

                        if(DateTime.Now < deadline)
                        {
                            throw new TimeoutException("BootTaskのキャンセル待ちがタイムアウトしました");
                        }
                    }
                }
            }

            _bootTask = null;
            _bootCts = null;
        }

        /// <summary>
        /// ProcessDockPanelを生成する
        /// </summary>
        private void CreateProcessDockPanel(Process process)
        {
            _processDockPanel = new ProcessDockPanel(process);
            dockPanel.Children.Clear();
            dockPanel.Children.Add(_processDockPanel);
        }

#region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    CancelBootTask(DateTime.Now + TimeSpan.FromMilliseconds(1000));
                    KillProcess();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~ProcessDockPanel()
        // {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
#endregion

        /// <summary>
        /// 再起動ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BootProcessAndDock();
        }
    }
}