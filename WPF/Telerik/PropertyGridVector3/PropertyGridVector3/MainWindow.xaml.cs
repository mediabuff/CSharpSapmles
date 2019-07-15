using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls.Data.PropertyGrid;

namespace PropertyGridVector3
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
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
                    if (propType == typeof(Vector3))
                    {
                        var dataTemple = createVector3DataTemplate(prop);
                        e.PropertyDefinition.EditorTemplate = dataTemple;
                    }
                }
            }
        }

        private DataTemplate createVector3DataTemplate(ItemPropertyInfo itemProperty)
        {
            FrameworkElementFactory editor = new FrameworkElementFactory(typeof(Vector3Editor));
            //!@note RadPropertyGridのSourceItemsにEnumerableを渡す場合はCurrentPropertySet[{propertyName}].
            // 単一のインスタンスの場合は{propertyName}をバインドターゲットにする必要がある.
#if true
            editor.SetBinding(Vector3Editor.ValueProperty, new Binding($"CurrentPropertySet[{itemProperty.Name}]") { Mode = BindingMode.TwoWay, Converter = new Vector3Converter() });
#else
            editor.SetBinding(Vector3Editor.ValueProperty, new Binding($"{itemProperty.Name}") { Mode = BindingMode.TwoWay, Converter = new Vector3Converter() });
#endif

            var dataTemplate = new DataTemplate();
            dataTemplate.VisualTree = editor;
            dataTemplate.Seal();

            return dataTemplate;
        }
    }
}
