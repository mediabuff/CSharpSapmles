using System;
using System.Windows;
using System.Windows.Controls;

namespace Telerik.Extensions.Controls
{
    /// <summary>
    /// NamedFlagEnumEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class NamedFlagEnumEditor : UserControl
    {
        NamedFlagEnumList _flagList;
        NamedFlagEnumList.FlagEnumListViewModel _vm;


        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register(nameof(EnumType), typeof(Type), typeof(NamedFlagEnumEditor),
                new PropertyMetadata(
                    (obj, args) =>
                    {
                        var value = (Type)args.NewValue;
                        var target = (NamedFlagEnumEditor)obj;
                        target._flagList = new NamedFlagEnumList(value);
                        target._vm = (target._flagList.DataContext as NamedFlagEnumList.FlagEnumListViewModel) ?? throw new NullReferenceException();
                        target.dropDownButton.DropDownContent = target._flagList;
                        target._vm.PropertyChanged += (s, e) =>
                        {
                            switch (e.PropertyName)
                            {
                                case nameof(NamedFlagEnumList.FlagEnumListViewModel.Value):
                                    target.Value = target._vm.Value;
                                    target.dropDownButton.Content = target._vm.ValueString;
                                    target.dropDownButton.ToolTip = target._vm.Tooltip;
                                    break;
                            }
                        };
                        target.Value = target._flagList.Value;
                    }));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Object), typeof(NamedFlagEnumEditor),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (obj, args) =>
                    {
                        var value = (Object)args.NewValue;
                        var target = (NamedFlagEnumEditor)obj;
                        target._flagList.Value = value;
                    }));


        /// <summary>
        /// EnumType
        /// </summary>
        public Type EnumType
        {
            get { return _vm?.EnumType; }
            set { SetValue(EnumTypeProperty, value); }
        }

        /// <summary>
        /// Value
        /// </summary>
        public Object Value
        {
            get { return _vm?.Value; }
            set { SetValue(ValueProperty, value); }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NamedFlagEnumEditor()
        {
            InitializeComponent();
        }
    }
}
