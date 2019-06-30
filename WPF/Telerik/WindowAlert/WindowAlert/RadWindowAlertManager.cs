using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Telerik.Windows.Controls;

namespace WindowAlert
{
    /// <summary>
    /// Window用のAlertManager
    /// </summary>
    public partial class RadWindowAlertManager : IDisposable
    {
        Window _owner;
        RadDesktopAlertManager _manager;
        AlertScreenPosition _alertScreenPosition = AlertScreenPosition.BottomRight;
        Double _alertWidth = 400;


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
        /// Alert1つの最大幅
        /// </summary>
        /// 
        public Double MaxAlertWidth
        {
            get { return _alertWidth; }
            set
            {
                _alertWidth = value;
                CloseAllAlerts(false);
            }
        }


        /// <summary>
        /// Alertを追加する
        /// </summary>
        public void ShowAlert(String header,
            Int32 showDuration = 3000, Boolean canMove = false, Boolean canAutoClose = true,
            Boolean showMenuButton = false, Boolean showInTaskSwitcher = false)
        {
            var alert = new RadDesktopAlert()
            {
                Header = header,
                MaxWidth = MaxAlertWidth,

                CanMove = canMove,
                CanAutoClose = canAutoClose,
                ShowDuration = showDuration,
                ShowMenuButton = showMenuButton,
                ShowInTaskSwitcher = showInTaskSwitcher,
            };
            _manager.ShowAlert(alert);
            adjustAlertNum();
        }

        /// <summary>
        /// Alertを追加する
        /// </summary>
        public void ShowAlert(RadDesktopAlert alert)
        {
            if (MaxAlertWidth < alert.MaxWidth)
                throw new ArgumentException($@"MaxAlertWidth < alert.MaxWidth({MaxAlertWidth} < {alert.MaxWidth}).\nMaxAlertWidthで先に最大サイズを設定してください");

            _manager.ShowAlert(alert);
            adjustAlertNum();
        }

        /// <summary>
        /// 全てのAlertを閉じる
        /// </summary>
        public void CloseAllAlerts(Boolean useAnimations = true)
        {
            _manager?.CloseAllAlerts(useAnimations);
        }


        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

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
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion


        /// <summary>
        /// コンストラクタ
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
        /// OwnerのLoadedイベント時に呼ばれる処理
        /// </summary>
        void ownerLoaded(Object sender, EventArgs args)
        {
            recreateManager();
        }

        /// <summary>
        /// OwnerのLoadedイベント時に呼ばれる処理
        /// </summary>
        void ownerLocationChanged(Object sender, EventArgs args)
        {
            recreateManager();
        }

        /// <summary>
        /// OwnerのLoadedイベント時に呼ばれる処理
        /// </summary>
        void ownerSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            recreateManager();
        }

        /// <summary>
        /// RadDesktopAlertManagerの再生成
        /// </summary>
        void recreateManager()
        {
            CloseAllAlerts(false);
            var alertOffsetPos = getAlertOffsetPosition(AlertWindowPosition, MaxAlertWidth);
            _manager = new RadDesktopAlertManager(AlertWindowPosition, alertOffsetPos);
        }

        /// <summary>
        /// AlertのOffset位置を取得
        /// </summary>
        Point getAlertOffsetPosition(AlertScreenPosition alertScreenPos, Double alertWidth)
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(_owner);
            if (source != null)
            {
                Win32.RECT cr, wr;
                if (Win32.GetClientRect(source.Handle, out cr) && Win32.GetWindowRect(source.Handle, out wr))
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
                            return new Point(clientPos.X + barSize,
                                clientPos.Y + barSize);
                        case AlertScreenPosition.TopCenter:
                            return new Point(clientPos.X + barSize + (clientRect.Width / 2) - (SystemParameters.WorkArea.Width / 2),
                                clientPos.Y + barSize);
                        case AlertScreenPosition.TopRight:
                            return new Point(windowRect.TopRight.X - (barSize * 2) - SystemParameters.WorkArea.Width,
                                clientPos.Y + barSize);
                        case AlertScreenPosition.BottomLeft:
                            return new Point(clientPos.X + barSize,
                                windowRect.BottomLeft.Y - barSize - SystemParameters.WorkArea.Height);
                        case AlertScreenPosition.BottomCenter:
                            return new Point(clientPos.X + barSize + (clientRect.Width / 2) - (SystemParameters.WorkArea.Width / 2),
                                windowRect.BottomLeft.Y - barSize - SystemParameters.WorkArea.Height);
                        case AlertScreenPosition.BottomRight:
                            return new Point(windowRect.BottomRight.X - (barSize * 2) - SystemParameters.WorkArea.Width,
                                windowRect.BottomLeft.Y - barSize - SystemParameters.WorkArea.Height);
                    }
                }
            }
            return new Point(0, 0);
        }

        /// <summary>
        /// Windowの高さによってAlertの数を調整する
        /// </summary>
        void adjustAlertNum()
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(_owner);
            if (source != null && _manager.GetAllAlerts().Any())
            {
                Win32.RECT cr;
                if (Win32.GetClientRect(source.Handle, out cr))
                {
                    var alertHeight = _manager.GetAllAlerts().Sum(x => x.Height);
                    var clientRect = new Rect(cr.Left, cr.Top, cr.Right - cr.Left, cr.Bottom - cr.Top);
                    while ((clientRect.Height < alertHeight) && _manager.GetAllAlerts().Any())
                    {
                        var alert = _manager.GetAllAlerts().FirstOrDefault();
                        _manager.CloseAlert(alert, false);

                        var oldHeight = alertHeight;
                        alertHeight = _manager.GetAllAlerts().Sum(x => x.Height);
                        if(Math.Abs(oldHeight - alertHeight) < 0.001)
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
        static private class Win32
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
