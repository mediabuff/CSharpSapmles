using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PropertyGridVector3
{
    /// <summary>
    /// Vector3.xaml の相互作用ロジック
    /// </summary>
    public partial class Vector3Editor : UserControl
    {
        Int32 _valueChanged = 0;


        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Vector3), typeof(Vector3Editor),
                new PropertyMetadata(
                    (obj, args) =>
                    {
                        var target = (Vector3Editor)obj;
                        target.Value = (Vector3)args.NewValue;
                    }));

        public Vector3 Value
        {
            get { return (Vector3)GetValue(ValueProperty); }
            set {
                SetValue(ValueProperty, value);
                _valueChanged++;
                XNumericUpDown.Value = value.X;
                YNumericUpDown.Value = value.Y;
                ZNumericUpDown.Value = value.Z;
                _valueChanged--;
            }
        }


        public Vector3Editor()
        {
            InitializeComponent();
        }


        private void XNumericUpDown_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (_valueChanged == 0)
            {
                Value = new Vector3() {
                    X = e.NewValue ?? default,
                    Y = YNumericUpDown.Value ?? default,
                    Z = ZNumericUpDown.Value ?? default,
                };
            }
        }

        private void YNumericUpDown_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (_valueChanged == 0)
            {
                Value = new Vector3() {
                    X = XNumericUpDown.Value ?? default,
                    Y = e.NewValue ?? default,
                    Z = ZNumericUpDown.Value ?? default,
                };
            }
        }

        private void ZNumericUpDown_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (_valueChanged == 0)
            {
                Value = new Vector3() {
                    X = XNumericUpDown.Value ?? default,
                    Y = YNumericUpDown.Value ?? default,
                    Z = e.NewValue ?? default,
                };
            }
        }
    }
}
