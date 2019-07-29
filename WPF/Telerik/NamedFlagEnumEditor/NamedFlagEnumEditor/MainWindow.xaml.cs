using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls.Data.PropertyGrid;

namespace NamedFlagEnumEditor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        static Dictionary<Type, IValueConverter> _enumConverterMap = new Dictionary<Type, IValueConverter>();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void RadPropertyGrid_AutoGeneratingPropertyDefinition(object sender, AutoGeneratingPropertyDefinitionEventArgs e)
        {
            var prop = e.PropertyDefinition.SourceProperty;
            if (prop != null)
            {
                var propType = prop.PropertyType;
                if (e.PropertyDefinition.EditorTemplate == null)
                {
                    // プロパティの型がEnumFlagsかつPropertySetで設定する場合には特殊対応が必要なので対応.
                    if (propType.IsEnum && propType.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                    {
                        var dataTemple = createFlagEnumEditorDataTemplate(prop);
                        e.PropertyDefinition.EditorTemplate = dataTemple;
                    }
                }
            }
        }

        private DataTemplate createFlagEnumEditorDataTemplate(ItemPropertyInfo itemProperty)
        {
            var flagEnumEditor = new FrameworkElementFactory(typeof(FlagEnumEditor));
            flagEnumEditor.SetValue(FlagEnumEditor.EnumTypeProperty, itemProperty.PropertyType);
            //!@note RadPropertyGridのSourceItemsにEnumerableを渡す場合はCurrentPropertySet[{propertyName}].
            // 単一のインスタンスの場合は{propertyName}をバインドターゲットにする必要がある.
            IValueConverter enumConverter;
            if (!_enumConverterMap.TryGetValue(itemProperty.PropertyType, out enumConverter))
            {
                enumConverter = new EnumConverter(itemProperty.PropertyType);
            }
#if true
            flagEnumEditor.SetBinding(FlagEnumEditor.ValueProperty, new Binding($"CurrentPropertySet[{ itemProperty.Name }]") { Mode = BindingMode.TwoWay, Converter = enumConverter });
#else
            flagEnumEditor.SetBinding(FlagEnumEditor.ValueProperty, new Binding($"{ propertyName }") { Mode = BindingMode.TwoWay, Converter = enumConverter });
#endif

            var dataTemplate = new DataTemplate();
            dataTemplate.VisualTree = flagEnumEditor;
            dataTemplate.Seal();

            return dataTemplate;
        }

        /// <summary>
        /// Enumの値を変換するコンバータ.
        /// PropertySetMode=Intercept or Unionの場合はBindingで特殊対応が必要みたいなので定義.
        /// https://www.telerik.com/forums/flagenumeditor-does-not-work-if-mutlple-item-is-set-to-radpropertygrid-sourceitems
        /// </summary>
        internal class EnumConverter : IValueConverter
        {

            Type _enumType;

            public EnumConverter(Type enumType) => _enumType = enumType;

            public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                try
                {
                    var strVal = value.ToString() == "0" ? "" : value.ToString();
                    // enumに変換できるかチェック
                    var enumVal = Enum.Parse(_enumType, strVal);
                    return strVal;
                }
                catch (Exception) { }

                // 空文字を返す.
                return "";
            }

            public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                try
                {
                    var strVal = String.IsNullOrEmpty(value.ToString()) ? "None" : value.ToString();
                    var enumVal = Enum.Parse(_enumType, strVal);
                    return enumVal;
                }
                catch (Exception) { }

                return Enum.ToObject(_enumType, 0);
            }
        }
    }
}
