using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace WindowAlert
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        RadWindowAlertManager _manager;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                _manager = new RadWindowAlertManager(this);
            };
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _manager.ShowAlert("Test1\nTest2\nTest3", "Header1_1");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _manager.ShowAlert("Test1\nTest2\nTest3", "Header2_1");
            _manager.ShowAlert("Test4\nTest5\nTest6", "Header2_2");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _manager.ShowAlert("Test1\nTest2\nTest3", "Header3_1");
            _manager.ShowAlert("Test4\nTest5\nTest6", "Header3_2");
            _manager.ShowAlert("Test7", "Header3_3");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _manager.ShowAlert("Test1\nTest2\nTest3", "Heade4_1");
            _manager.ShowAlert("Test4\nTest5\nTest6", "Heade4_2");
            _manager.ShowAlert("Test7", "Header4_3");
            _manager.ShowAlert("Test8", "Header4_4");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.OfType<ComboBoxItem>().FirstOrDefault() is ComboBoxItem comboBoxItem)
            {
                _manager?.Dispose();
                switch ((String)comboBoxItem.Content)
                {
                    case "TopLeft":
                        _manager = new RadWindowAlertManager(this, Telerik.Windows.Controls.AlertScreenPosition.TopLeft);
                        break;
                    case "TopCenter":
                        _manager = new RadWindowAlertManager(this, Telerik.Windows.Controls.AlertScreenPosition.TopCenter);
                        break;
                    case "TopRight":
                        _manager = new RadWindowAlertManager(this, Telerik.Windows.Controls.AlertScreenPosition.TopRight);
                        break;
                    case "BottomLeft":
                        _manager = new RadWindowAlertManager(this, Telerik.Windows.Controls.AlertScreenPosition.BottomLeft);
                        break;
                    case "BottomCenter":
                        _manager = new RadWindowAlertManager(this, Telerik.Windows.Controls.AlertScreenPosition.BottomCenter);
                        break;
                    case "BottomRight":
                    default:
                        _manager = new RadWindowAlertManager(this, Telerik.Windows.Controls.AlertScreenPosition.BottomRight);
                        break;
                }
            }
        }
    }
}
