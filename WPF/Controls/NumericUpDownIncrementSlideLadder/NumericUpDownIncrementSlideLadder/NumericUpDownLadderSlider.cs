using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.Toolkit;

namespace NumericUpDownIncrementSlideLadder
{
    public class NumericUpDownLadderSlider : DoubleUpDown
    {
        Popup _popUp;
        IncrementSlideLadder _ladder;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NumericUpDownLadderSlider() : base()
        {
            // ladder
            _ladder = new IncrementSlideLadder();
            _ladder.Incremented += (s, e) => Value = (Double)((Decimal)Value + e.Increment);

            // popup
            _popUp = new Popup()
            {
                PlacementTarget = this,
                AllowsTransparency = true,
            };
            _popUp.Child = _ladder;

            // mousedown
            PreviewMouseDown += (s, e) =>
            {
                if (e.MiddleButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    _ladder.TargetHeight = RenderSize.Height;
                    _popUp.Placement = PlacementMode.Custom;
                    _popUp.CustomPopupPlacementCallback = (popupSize, targetSize, offset) =>
                    {
                        var mousePos = e.GetPosition(this);
                        var pos = new Point(
                            mousePos.X - (popupSize.Width * 0.5),
                            (targetSize.Height * 0.5) - (popupSize.Height * 0.5)
                            );

                        return new CustomPopupPlacement[] {
                            new CustomPopupPlacement(pos, PopupPrimaryAxis.Vertical),
                        };
                    };

                    _popUp.IsOpen = true;
                    e.Handled = true;
                }
            };
        }
    }
}
