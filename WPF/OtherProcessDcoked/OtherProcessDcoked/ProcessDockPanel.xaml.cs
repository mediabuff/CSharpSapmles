using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace OtherProcessDcoked
{
    /// <summary>
    /// ProcessDockPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class ProcessDockPanel : UserControl
    {
        Process _process;
        IntPtr _hWndOriginalParent;
        IntPtr _hWndDocked;
        System.Windows.Forms.Panel _panel;


        /// <summary>
        /// ProcessのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty DockProcessProperty =
            DependencyProperty.Register(nameof(Process), typeof(Process), typeof(ProcessDockPanel),
                new PropertyMetadata(
                    (obj, args) =>
                    {
                        var target = (ProcessDockPanel)obj;
                        target.Process = (Process)args.NewValue;
                    }));

        /// <summary>
        /// EnableWsChildのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty EnableWsChildProperty =
            DependencyProperty.Register(nameof(EnableWsChild), typeof(Boolean), typeof(ProcessDockPanel),
                new PropertyMetadata(
                    (obj, args) =>
                    {
                        var target = (ProcessDockPanel)obj;
                        target.EnableWsChild = (Boolean)args.NewValue;
                    }));


        /// <summary>
        /// ドッキングするProcess
        /// Processは(Process != null && Process.HasExited == false && Process.MainWindowHandle != IntPtr.Zero)である必要があります
        /// Process破棄処理は行わないので設定側が破棄してください
        /// </summary>
        public Process Process
        {
            get { return _process; }
            set
            {
                if (_process != value)
                {
                    _process = value;

                    try
                    {
                        DockProcess();
                    }
                    catch(Exception e)
                    {
                        Trace.WriteLine(e);
                        MessageBox.Show($"プロセスのドッキングに失敗しました\n{e}", "プロセスドッキング失敗");
                    }
                }
            }
        }

        /// <summary>
        /// Dock時にWS_CHILDを設定するか
        /// アプリケーションによってはfalseにする必要があります
        /// </summary>
        public Boolean EnableWsChild { get; set; } = false;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProcessDockPanel(Process process)
        {
            InitializeComponent();

            // Panelを生成してWindowsFormsHostの子供に設定.
            _panel = new System.Windows.Forms.Panel();
            editorFormHost.Child = _panel;

            Process = process;
        }
        public ProcessDockPanel() : this(null) { }


        /// <summary>
        /// Processをドッキングする
        /// </summary>
        private void DockProcess()
        {
            if(Debugger.IsAttached && !EnableWsChild)
            {
                Trace.WriteLine("Debugger.IsAttached && !EnableWsChildの場合はドッキングしません");
                return;
            }

            _hWndDocked = _process.MainWindowHandle;

            // ウィンドウスタイル設定
            if (EnableWsChild)
            {
                uint flags = Win32.GetWindowLong(_hWndDocked, Win32.GWL_STYLE);
                flags &= ~Win32.WS_POPUP;
                flags &= ~Win32.WS_CAPTION;
                flags &= ~Win32.WS_BORDER;
                flags &= ~Win32.WS_DLGFRAME;
                flags &= ~Win32.WS_THICKFRAME;
                flags &= ~Win32.WS_SYSMENU;
                flags &= ~Win32.WS_MINIMIZE;
                flags &= ~Win32.WS_MAXIMIZE;
                flags &= ~Win32.WS_MINIMIZEBOX;
                flags &= ~Win32.WS_MAXIMIZEBOX;
                flags |= Win32.WS_VISIBLE;
                flags |= Win32.WS_CHILD;
                Win32.SetWindowLong(_hWndDocked, Win32.GWL_EXSTYLE, 0);
                Win32.SetWindowLong(_hWndDocked, Win32.GWL_STYLE, flags);
            }

            // 埋め込み処理
            // https://stackoverflow.com/questions/28475308/how-to-take-handle-of-a-window-in-wpf
            _hWndOriginalParent = Win32.SetParent(_hWndDocked, _panel.Handle);

            // SetFormのサイズをPanelサイズに合わせる.
            _panel.SizeChanged += Panel_SizeChanged;
            AlignToPannel();
        }

        /// <summary>
        /// SetFormのPanelサイズに合わせる
        /// </summary>
        private void AlignToPannel()
        {
            Win32.MoveWindow(_hWndDocked, 0, 0, _panel.Width, _panel.Height, true);
        }

        /// <summary>
        /// サイズ変更イベント
        /// </summary>
        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            AlignToPannel();
        }


        /// <summary>
        /// Win32Apiをまとめたクラス
        /// </summary>
        private static class Win32
        {
            public const int GWL_EXSTYLE = -20;
            public const int GWL_STYLE = -16;

            public const uint WS_OVERLAPPED = 0x00000000;
            public const uint WS_POPUP = 0x80000000;
            public const uint WS_CHILD = 0x40000000;
            public const uint WS_MINIMIZE = 0x20000000;
            public const uint WS_VISIBLE = 0x10000000;
            public const uint WS_DISABLED = 0x08000000;
            public const uint WS_CLIPSIBLINGS = 0x04000000;
            public const uint WS_CLIPCHILDREN = 0x02000000;
            public const uint WS_MAXIMIZE = 0x01000000;
            public const uint WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
            public const uint WS_BORDER = 0x00800000;
            public const uint WS_DLGFRAME = 0x00400000;
            public const uint WS_VSCROLL = 0x00200000;
            public const uint WS_HSCROLL = 0x00100000;
            public const uint WS_SYSMENU = 0x00080000;
            public const uint WS_THICKFRAME = 0x00040000;
            public const uint WS_GROUP = 0x00020000;
            public const uint WS_TABSTOP = 0x00010000;
            public const uint WS_MINIMIZEBOX = 0x00020000;
            public const uint WS_MAXIMIZEBOX = 0x00010000;

            public const int SW_SHOWMAXIMIZED = 3;

            [DllImport("user32.dll")]
            public static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

            [DllImport("user32.dll")]
            public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll")]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [DllImport("user32.dll")]
            public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        }
    }
}