using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace NumericUpDownIncrementSlideLadder
{
    /// <summary>
    /// IncrementSlideLadder.xaml の相互作用ロジック
    /// </summary>
    public partial class IncrementSlideLadder : UserControl
    {
        Point _prevMousePos = default;
        Double _addUnit;
        readonly Dictionary<Border, Tuple<Border, Double>> unitMap = new Dictionary<Border, Tuple<Border, Double>>();


        /// <summary>
        /// インクリメントイベント引数
        /// </summary>
        public class IncrementedEventArgs : EventArgs
        {
            /// <summary>
            /// インクリメント値
            /// </summary>
            public Decimal Increment { get; set; } = 0.0M;
        }

        /// <summary>
        /// インクリメントイベント
        /// </summary>
        public event EventHandler<IncrementedEventArgs> Incremented;


        /// <summary>
        /// Ladderを表示するターゲットの高さ
        /// </summary>
        public Double TargetHeight
        {
            get { return (Double)this.GetValue(TargetHeightProperty); }
            set { this.SetValue(TargetHeightProperty, value); }
        }
        public static readonly DependencyProperty TargetHeightProperty =
            DependencyProperty.Register("TargetHeight", typeof(Double), typeof(IncrementSlideLadder), new UIPropertyMetadata(20.0));

        /// <summary>
        /// Ladderの高さ
        /// </summary>
        public Double LadderHeight
        {
            get { return (Double)this.GetValue(LadderHeightProperty); }
            set { this.SetValue(LadderHeightProperty, value); }
        }
        public static readonly DependencyProperty LadderHeightProperty =
            DependencyProperty.Register("LadderHeight", typeof(Double), typeof(IncrementSlideLadder), new UIPropertyMetadata(35.0));

        /// <summary>
        /// Ladderの背景色
        /// </summary>
        public Brush LadderForeground
        {
            get { return (Brush)this.GetValue(LadderForegroundProperty); }
            set { this.SetValue(LadderForegroundProperty, value); }
        }
        public static readonly DependencyProperty LadderForegroundProperty =
            DependencyProperty.Register("LadderForeground", typeof(Brush), typeof(IncrementSlideLadder), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Ladderの背景色
        /// </summary>
        public Brush LadderBackground
        {
            get { return (Brush)this.GetValue(LadderBackgroundProperty); }
            set { this.SetValue(LadderBackgroundProperty, value); }
        }
        public static readonly DependencyProperty LadderBackgroundProperty =
            DependencyProperty.Register("LadderBackground", typeof(Brush), typeof(IncrementSlideLadder), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// マウスオーバー時のLadderの背景色
        /// </summary>
        public Brush MouseOverLadderBackground
        {
            get { return (Brush)this.GetValue(MouseOverLadderBackgroundProperty); }
            set { this.SetValue(MouseOverLadderBackgroundProperty, value); }
        }
        public static readonly DependencyProperty MouseOverLadderBackgroundProperty =
            DependencyProperty.Register("MouseOverLadderBackground", typeof(Brush), typeof(IncrementSlideLadder), new UIPropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Ladderのボーダー色
        /// </summary>
        public Brush LadderBorderBrush
        {
            get { return (Brush)this.GetValue(LadderBorderBrushProperty); }
            set { this.SetValue(LadderBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty LadderBorderBrushProperty =
            DependencyProperty.Register("LadderBorderBrush", typeof(Brush), typeof(IncrementSlideLadder), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// LadderのボーダーThickness
        /// </summary>
        public Int32 LadderBorderThickness
        {
            get { return (Int32)this.GetValue(LadderThicknessProperty); }
            set { this.SetValue(LadderThicknessProperty, value); }
        }
        public static readonly DependencyProperty LadderThicknessProperty =
            DependencyProperty.Register("LadderThickness", typeof(Int32), typeof(IncrementSlideLadder), new UIPropertyMetadata(0));

        /// <summary>
        /// インクリメントイベントを発生させるマウス移動の閾値
        /// </summary>
        public Int32 IncrementMouseMoveThreshold
        {
            get { return (Int32)this.GetValue(IncrementMouseMoveThresholdProperty); }
            set { this.SetValue(IncrementMouseMoveThresholdProperty, value); }
        }
        public static readonly DependencyProperty IncrementMouseMoveThresholdProperty =
            DependencyProperty.Register("IncrementMouseMoveThreshold", typeof(Int32), typeof(IncrementSlideLadder), new UIPropertyMetadata(10));

        /// <summary>
        /// インクリメント時にマウスのXを固定するか
        /// </summary>
        public Boolean IncrementMouseMoveXFixed
        {
            get { return (Boolean)this.GetValue(IncrementMouseMoveXFixedProperty); }
            set { this.SetValue(IncrementMouseMoveXFixedProperty, value); }
        }
        public static readonly DependencyProperty IncrementMouseMoveXFixedProperty =
            DependencyProperty.Register("IncrementMouseMoveXFixed", typeof(Boolean), typeof(IncrementSlideLadder), new UIPropertyMetadata(true));


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IncrementSlideLadder()
        {
            InitializeComponent();

            // unitMap生成.
            {
                unitMap.Add(x1000, Tuple.Create(x1000, 1000.0));
                unitMap.Add(x100, Tuple.Create(x100, 100.0));
                unitMap.Add(x10, Tuple.Create(x10, 10.0));
                unitMap.Add(x1_, Tuple.Create(x1_, 1.0));
                unitMap.Add(x_1, Tuple.Create(x_1, 1.0));
                unitMap.Add(x0_1, Tuple.Create(x0_1, 0.1));
                unitMap.Add(x0_01, Tuple.Create(x0_01, 0.01));
                unitMap.Add(x0_001, Tuple.Create(x0_001, 0.001));
            }

            IsVisibleChanged += (s, e) =>
            {
                if (IsVisible)
                {
                    if (NativeMethods.GetCursorPos(out var point))
                    {
                        _prevMousePos = new Point(point.X, point.Y);
                    }
                    CompositionTarget.Rendering += Update;
                }
                else
                {
                    CompositionTarget.Rendering -= Update;
                }
            };
        }

        /// <summary>
        /// 更新処理(Renderingで処理される)
        /// </summary>
        private void Update(Object sender, EventArgs e)
        {
            if (0 <= NativeMethods.GetKeyState(NativeMethods.VK_MIDDLE))
            {
                ((Popup)this.Parent).IsOpen = false;
                return;
            }

            if (NativeMethods.GetCursorPos(out var point))
            {
                var currentMousePos = new Point(point.X, point.Y);
                var move = (currentMousePos - _prevMousePos).X;
                if (IncrementMouseMoveThreshold < Math.Abs(move))
                {
                    var inc = (move < 0.0) ? -_addUnit : _addUnit;
                    if (IncrementMouseMoveXFixed)
                    {
                        NativeMethods.SetCursorPos((Int32)_prevMousePos.X, (Int32)currentMousePos.Y);
                    }
                    else
                    {
                        _prevMousePos = currentMousePos;
                    }
                    Incremented?.Invoke(this, new IncrementedEventArgs() { Increment = (Decimal)inc });
                }
            }
        }

        /// <summary>
        /// Ladder領域にマウスが入った時の処理
        /// </summary>
        private void Ladder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (unitMap.TryGetValue((Border)sender, out var v))
            {
                var defaultBrush = LadderBackground;
                var overBrush = MouseOverLadderBackground;
                foreach (var border in unitMap.Keys)
                {
                    if (v.Item1 == border)
                    {
                        border.Background = overBrush;
                    }
                    else
                    {
                        border.Background = defaultBrush;
                    }
                }

                _addUnit = v.Item2;
            }
        }

        /// <summary>
        /// Win32API
        /// </summary>
        private static class NativeMethods
        {
            public static readonly Int32 VK_MIDDLE = 0x04;

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern Int16 GetKeyState(Int32 nVirtKey);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern Boolean GetCursorPos(out POINT lpPoint);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool SetCursorPos(Int32 x, Int32 y);

            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public struct POINT
            {
                public Int32 X { get; set; }
                public Int32 Y { get; set; }
            }
        }
    }
}
