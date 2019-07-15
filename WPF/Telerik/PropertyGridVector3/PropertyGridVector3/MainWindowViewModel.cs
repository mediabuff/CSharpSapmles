using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace PropertyGridVector3
{
    public class Vector3
    {
        public Double X { get; set; }
        public Double Y { get; set; }
        public Double Z { get; set; }

#if false// とりあえず不要
        internal static Boolean TryParse(string v, out Vector3 tmp)
        {
            try
            {
                var array = v.Split(',').ToArray();
                Single.TryParse(array[0], out var x);
                Single.TryParse(array[0], out var y);
                Single.TryParse(array[0], out var z);
                tmp = new Vector3();
                tmp.X = x;
                tmp.Y = y;
                tmp.Z = z;
                return true;
            }
            catch (Exception)
            {
                /* */
            }
            tmp = null;
            return false;
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }
#endif
    }

    public class Vector3Converter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return value;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class TreeItemViewModel : ViewModelBase
    {
        Vector3 _vec3 = new Vector3();
        String _header = "Header";
        Boolean _selected;
        MainWindowViewModel _owner;

        public Vector3 Vec3 {
            get { return _vec3; }
            set
            {
                _vec3 = value;
                RaisePropertyChanged();
            }
        }

        public String Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged();
            }
        }

        public Boolean Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RaisePropertyChanged();
            }
        }


        public TreeItemViewModel(MainWindowViewModel owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            PropertyChanged +=
                (_, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(Selected):
                            _owner.TreeItemSelectionChanged();
                            break;
                    }
                };
        }
    }


    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<TreeItemViewModel> TreeItems { get; } = new ObservableCollection<TreeItemViewModel>();

        public IEnumerable<TreeItemViewModel> SelectedTreeItems
        {
            get { return TreeItems.Where(x => x.Selected); }
        }

        public void TreeItemSelectionChanged()
        {
            RaisePropertyChanged(nameof(SelectedTreeItems));
        }


        public MainWindowViewModel()
        {
            TreeItems.Add(new TreeItemViewModel(this));
            TreeItems.Add(new TreeItemViewModel(this));
            TreeItems.Add(new TreeItemViewModel(this));
        }
    }
}
