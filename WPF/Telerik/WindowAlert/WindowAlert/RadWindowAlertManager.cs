using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Telerik.Windows.Controls
{
    /// <summary>
    /// Extended RadDesktopAlert to WindowAlert.
    /// </summary>
    public partial class RadWindowAlertManager : IDisposable
    {
        Window _owner;
        RadDesktopAlertManager _manager;
        AlertScreenPosition _alertScreenPosition = AlertScreenPosition.BottomRight;


        /// <summary>
        /// AlertのWindowでの位置
        /// </summary>
        public AlertScreenPosition AlertWindowPosition
        {
            get { return _alertScreenPosition; }
            set
            {
                _alertScreenPosition = value;
                recreateManager();
            }
        }


        /// <summary>
        /// ShowAlert
        /// </summary>
        public void ShowAlert(String content, String header = "",
            Int32 showDuration = 3000, Boolean canMove = false, Boolean canAutoClose = true,
            Boolean showMenuButton = false, Boolean showInTaskSwitcher = false
        )
        {
            var alert = new RadDesktopAlert()
            {
                Header = header,
                Content = content,

                CanMove = canMove,
                CanAutoClose = canAutoClose,
                ShowDuration = showDuration,
                ShowMenuButton = showMenuButton,
                //ShowInTaskSwitcher = showInTaskSwitcher,
            };
            _manager.ShowAlert(alert);
            adjustAlertNum();
        }

        /// <summary>
        /// ShowAlert
        /// </summary>
        public void ShowAlert(RadDesktopAlert alert)
        {
            _manager.ShowAlert(alert);
            adjustAlertNum();
        }

        /// <summary>
        /// CloseAllAlerts
        /// </summary>
        public void CloseAllAlerts(Boolean useAnimations = true)
        {
            _manager?.CloseAllAlerts(useAnimations);
        }


        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseAllAlerts(false);
                    _owner.Loaded -= ownerLoaded;
                    _owner.LocationChanged -= ownerLocationChanged;
                    _owner.SizeChanged -= ownerSizeChanged;
                }
            }

            // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
            // TODO: 大きなフィールドを null に設定します。

            disposedValue = true;
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~RadWindowAlertManager()
        // {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion


        /// <summary>
        /// Constructors
        /// </summary>
        public RadWindowAlertManager(Window owner, AlertScreenPosition alertScreenPosition = AlertScreenPosition.BottomRight)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _alertScreenPosition = alertScreenPosition;

            if ((HwndSource)HwndSource.FromVisual(_owner) != null)
            {
                recreateManager();
            }
            else
            {
                _owner.Loaded += ownerLoaded;
            }
            _owner.LocationChanged += ownerLocationChanged;
            _owner.SizeChanged += ownerSizeChanged;
        }


        /// <summary>
        /// Owner Loaded Events.
        /// </summary>
        void ownerLoaded(Object sender, EventArgs args)
        {
            recreateManager();
        }

        /// <summary>
        /// Owner LocationChanged Events.
        /// </summary>
        void ownerLocationChanged(Object sender, EventArgs args)
        {
            recreateManager();
        }

        /// <summary>
        /// Owner SizeChanged Events.
        /// </summary>
        void ownerSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            recreateManager();
        }

        /// <summary>
        /// Recreate RadDesktopAlertManager.
        /// </summary>
        void recreateManager()
        {
            CloseAllAlerts(false);
            var alertOffsetPos = getAlertOffsetPosition(AlertWindowPosition);
            _manager = new RadDesktopAlertManager(AlertWindowPosition, alertOffsetPos);
        }

        /// <summary>
        /// GetAlertOffsetPosition.
        /// </summary>
        Point getAlertOffsetPosition(AlertScreenPosition alertScreenPos)
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(_owner);
            if (source != null)
            {
                if (Win32.GetClientRect(source.Handle, out Win32.RECT cr) && Win32.GetWindowRect(source.Handle, out Win32.RECT wr))
                {
                    var windowRect = new Rect(wr.Left, wr.Top, wr.Right - wr.Left, wr.Bottom - wr.Top);
                    var clientRect = new Rect(cr.Left, cr.Top, cr.Right - cr.Left, cr.Bottom - cr.Top);
                    var clientPos = new System.Drawing.Point((int)clientRect.Left, (int)clientRect.Top);
                    Win32.ClientToScreen(source.Handle, ref clientPos);
                    var barSize = (Int32)(clientPos.X - windowRect.X);
                    clientRect.Offset(clientPos.X, clientPos.Y);

                    switch (alertScreenPos)
                    {
                        case AlertScreenPosition.TopLeft:
                            return new Point(clientPos.X + barSize - SystemParameters.WorkArea.X,
                                clientPos.Y + barSize - SystemParameters.WorkArea.Y);
                        case AlertScreenPosition.TopCenter:
                            return new Point(clientPos.X + barSize + (clientRect.Width * 0.5) - (SystemParameters.WorkArea.Width * 0.5),
                                clientPos.Y + barSize - SystemParameters.WorkArea.Y);
                        case AlertScreenPosition.TopRight:
                            return new Point(windowRect.Right - (barSize * 2) - SystemParameters.WorkArea.Right,
                                clientPos.Y + barSize - SystemParameters.WorkArea.Y);
                        case AlertScreenPosition.BottomLeft:
                            return new Point(clientPos.X + barSize - SystemParameters.WorkArea.X,
                                windowRect.BottomLeft.Y - barSize - SystemParameters.WorkArea.Bottom);
                        case AlertScreenPosition.BottomCenter:
                            return new Point(clientPos.X + barSize + (clientRect.Width * 0.5) - (SystemParameters.WorkArea.Width * 0.5),
                                windowRect.BottomLeft.Y - barSize - SystemParameters.WorkArea.Bottom);
                        case AlertScreenPosition.BottomRight:
                            return new Point(windowRect.Right - (barSize * 2) - SystemParameters.WorkArea.Right,
                                windowRect.BottomLeft.Y - barSize - SystemParameters.WorkArea.Bottom);
                    }
                }
            }
            return new Point(0, 0);
        }

        /// <summary>
        /// Adjust the number of alerts by the height of the window
        /// </summary>
        void adjustAlertNum()
        {
            var source = HwndSource.FromVisual(_owner) as HwndSource;
            if (source != null && _manager.GetAllAlerts().Any())
            {
                if (Win32.GetClientRect(source.Handle, out Win32.RECT cr))
                {
                    var alertHeight = _manager.GetAllAlerts().Sum(x => x.Height);
                    var clientRect = new Rect(cr.Left, cr.Top, cr.Right - cr.Left, cr.Bottom - cr.Top);
                    while ((clientRect.Height < alertHeight) && _manager.GetAllAlerts().Any())
                    {
                        var alert = _manager.GetAllAlerts().FirstOrDefault();
                        _manager.CloseAlert(alert, false);

                        var oldHeight = alertHeight;
                        alertHeight = _manager.GetAllAlerts().Sum(x => x.Height);
                        if (Math.Abs(oldHeight - alertHeight) < 0.001)
                        {
                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Win32Api
        /// </summary>
        static class Win32
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean GetClientRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("user32.dll")]
            public static extern Boolean ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);


            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }
        }
    }
}
