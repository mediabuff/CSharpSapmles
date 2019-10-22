using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Telerik.Extensions.Controls
{
    /// <summary>
    /// NamedFlagEnumList.xaml の相互作用ロジック
    /// </summary>
    public partial class NamedFlagEnumList : UserControl
    {
        /// <summary>
        /// Value
        /// </summary>
        public Object Value
        {
            get { return (DataContext as FlagEnumListViewModel)?.Value; }
            set
            {
                if(DataContext is FlagEnumListViewModel vm)
                {
                    vm.Value = value;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NamedFlagEnumList(Type enumType)
        {
            InitializeComponent();

            DataContext = new FlagEnumListViewModel(enumType);
        }


        /// <summary>
        /// 項目クリックイベント
        /// </summary>
        private void RadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = ((RadButton)sender).Parent.ChildrenOfType<CheckBox>().FirstOrDefault();
            if (checkBox != null)
            {
                checkBox.IsChecked = !checkBox.IsChecked;
            }
        }
    }

    public partial class NamedFlagEnumList
    {
        internal class FlagEnumListItemViewModel : ViewModelBase
        {
            public String Header
            {
                get { return _header; }
                set
                {
                    if (_header != value)
                    {
                        _header = value;
                        RaisePropertyChanged();
                    }
                }
            }
            String _header = "";

            public Object Value
            {
                get { return _value; }
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        RaisePropertyChanged();
                    }
                }
            }
            Object _value;

            public String Tooltip
            {
                get { return _toolTip; }
                set
                {
                    value = String.IsNullOrEmpty(value) ? null : value;
                    if (_toolTip != value)
                    {
                        _toolTip = value;
                        RaisePropertyChanged();
                    }
                }
            }
            String _toolTip = null;

            public Boolean IsChecked
            {
                get { return _isChecked; }
                set
                {
                    if (_isChecked != value)
                    {
                        _isChecked = value;
                        RaisePropertyChanged();
                    }
                }
            }
            Boolean _isChecked = false;


            /// <summary>
            /// ToString
            /// </summary>
            public override String ToString() => Header;
        }


        internal class FlagEnumListViewModel : ViewModelBase
        {
            Dictionary<String, String> _flag2DispNameMap = new Dictionary<String, String>();
            Dictionary<String, String> _dispName2FlagMap = new Dictionary<String, String>();
            Dictionary<String, UInt32> _enumName2FlagMap = new Dictionary<String, UInt32>();
            ReadOnlyObservableCollection<FlagEnumListItemViewModel> _comboBoxItems = new ReadOnlyObservableCollection<FlagEnumListItemViewModel>(new ObservableCollection<FlagEnumListItemViewModel>());
            Int32 _valueChanging = 0;


            public ReadOnlyObservableCollection<FlagEnumListItemViewModel> Items
            {
                get { return _comboBoxItems; }
                private set
                {
                    _comboBoxItems = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(Value));
                    RaisePropertyChanged(nameof(Tooltip));
                }
            }

            public Type EnumType
            {
                get { return _enumType; }
                private set
                {
                    if (_enumType == value) { return; }

                    _enumType = value;

                    _flag2DispNameMap.Clear();
                    _dispName2FlagMap.Clear();
                    _enumName2FlagMap.Clear();

                    var items = new List<FlagEnumListItemViewModel>();
                    foreach (var flag in Enum.GetValues(value))
                    {
                        var name = GetEnumValueDisplayName((Enum)flag);
                        var desc = GetEnumValueDescription((Enum)flag);

                        _flag2DispNameMap.Add(flag.ToString(), name);
                        _dispName2FlagMap.Add(name, flag.ToString());
                        _enumName2FlagMap.Add(flag.ToString(), (UInt32)(Int32)flag);
                        var vm = new FlagEnumListItemViewModel
                        {
                            Header = name,
                            Tooltip = desc,
                            Value = flag,
                        };
                        vm.PropertyChanged += (s, e) =>
                        {
                            var sender = ((FlagEnumListItemViewModel)s);
                            switch (e.PropertyName)
                            {
                                case "IsChecked":
                                    if (_valueChanging <= 0)
                                    {
                                        if ((Int32)sender.Value == 0)
                                        {
                                            Value = 0;
                                        }
                                        else if (sender.IsChecked)
                                        {
                                            var intValue = (Int32)Value | ((Int32)sender.Value);
                                            Value = intValue;
                                        }
                                        else
                                        {
                                            var intValue = (Int32)Value & (~(Int32)sender.Value);
                                            Value = intValue;
                                        }

                                        RaisePropertyChanged(nameof(Value));
                                        RaisePropertyChanged(nameof(Tooltip));
                                    }
                                    break;
                            }
                        };
                        items.Add(vm);
                    }
                    Items = new ReadOnlyObservableCollection<FlagEnumListItemViewModel>(
                        new ObservableCollection<FlagEnumListItemViewModel>(items));
                }
            }
            Type _enumType;

            public Object Value
            {
                get
                {
                    var value = 0U;
                    foreach (var item in Items.Where(x => x.IsChecked))
                    {
                        value |= _enumName2FlagMap[_dispName2FlagMap[item.Header]];
                    }
                    return Enum.ToObject(_enumType, value);
                }
                set
                {
                    if (Enum.Equals(Value, value))
                    {
                        return;
                    }

                    try
                    {
                        var strFlag = ConvertEnumValueToDispName(value);
                        var strFlags = strFlag.Split(',').Select(x => x.Trim());
                        if (String.IsNullOrEmpty(strFlag) || strFlags.Contains("None") || strFlags.Contains("なし"))
                        {
                            value = Enum.ToObject(_enumType, 0); ;
                        }

                        ++_valueChanging;
                        foreach (var item in Items)
                        {
                            item.IsChecked = strFlags.Contains(item.Header);
                        }
                        --_valueChanging;
                    }
                    catch (Exception) { }

                    RaisePropertyChanged(nameof(Value));
                    RaisePropertyChanged(nameof(Tooltip));
                }
            }

            public String ValueString
            {
                get { return ConvertEnumValueToDispName(Value); }
            }

            public String Tooltip
            {
                get
                {
                    var enumValue = Value;
                    if (enumValue != null && 0 < (Int32)enumValue)
                    {
                        var selectedItemNames = Items.Where(x => x.IsChecked).Select(x => x.Header);
                        if (selectedItemNames.Any())
                        {
                            var ret = String.Join(Environment.NewLine, selectedItemNames);
                            ret = String.Join(Environment.NewLine, $"[0x{((Int32)enumValue).ToString("x8")}]", ret);
                            return ret;
                        }
                    }
                    return null;
                }
            }


            /// <summary>
            /// コンストラクタ
            /// </summary>
            public FlagEnumListViewModel(Type enumType)
            {
                EnumType = enumType;
            }


            private String ConvertEnumValueToDispName(Object value)
            {
                try
                {
                    var strVal = value?.ToString();
                    if (String.IsNullOrEmpty(strVal))
                    {
                        return "";
                    }

                    // enumに変換できるかチェック
                    var enumVal = Enum.Parse(_enumType, strVal);

                    // 複合フラグ対応
                    var enumIntVal = (Int32)enumVal;
                    var enumValues = new List<Object>();
                    foreach (var val in Enum.GetValues(_enumType))
                    {
                        var andVal = (Int32)val & enumIntVal;
                        if (andVal == (Int32)val)
                        {
                            enumValues.Add(val);
                        }
                    }
                    if (enumValues.Count != enumValues.Where(x => (Int32)x == 0).Count())
                    {
                        enumValues.RemoveAll(x => (Int32)x == 0);
                    }

                    var dispNames = new List<String>();
                    foreach (var str in enumValues.Select(x => x.ToString()))
                    {
                        if (_flag2DispNameMap.TryGetValue(str, out var disp))
                        {
                            dispNames.Add(disp);
                        }
                    }
                    strVal = String.Join(", ", dispNames);
                    return strVal;
                }
                catch (Exception) { }

                return "";
            }

            private Object ConvertStringToEnumValue(String str)
            {
                if (!String.IsNullOrEmpty(str))
                {
                    try
                    {
                        var isNone = false;
                        var flagNames = new List<String>();
                        foreach (var s in str.Split(',').Select(x => x.Trim()))
                        {
                            if (_dispName2FlagMap.TryGetValue(s, out var flag))
                            {
                                if ("None" == flag)
                                {
                                    isNone = true;
                                    break;
                                }
                                flagNames.Add(flag);
                            }
                        }
                        str = isNone ? "" : String.Join(", ", flagNames);

                        if (!String.IsNullOrEmpty(str))
                        {
                            var val = Enum.Parse(_enumType, str);
                            return val;
                        }
                    }
                    catch (Exception) { }
                }
                return Enum.ToObject(_enumType, 0);
            }

            /// <summary>
            /// Enumの値からDisplayAttributeを取得する
            /// </summary>
            private static DisplayAttribute GetEnumValueDisplayAttribute<T>(T enumValue) where T : Enum
            {
                var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
                var displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false).OfType<DisplayAttribute>().ToArray();
                if (displayAttributes.Any())
                {
                    return displayAttributes.First();
                }
                return null;
            }

            /// <summary>
            /// Enumの値から表示名を取得する
            /// </summary>
            private static String GetEnumValueDisplayName<T>(T enumValue) where T : Enum
            {
                var displayAttr = GetEnumValueDisplayAttribute(enumValue);
                return displayAttr?.Name ?? enumValue.ToString();
            }

            /// <summary>
            /// Enumの値から説明文を取得する
            /// </summary>
            private static String GetEnumValueDescription<T>(T enumValue) where T : Enum
            {
                var displayAttr = GetEnumValueDisplayAttribute(enumValue);
                return displayAttr?.Description ?? enumValue.ToString();
            }
        }
    }
}
