using System;
using System.Windows;

namespace OtherProcessDcoked
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            processDockControl.ProcessFullPaths = new String[] {
                "notepad.exe",
                // Chromeはうまくいかない.マルチプロセスアプリケーションはダメ.
                @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                // 電卓もうまくいかない.UWPアプリはダメ.
                "calc.exe",
            };
        }
    }
}
