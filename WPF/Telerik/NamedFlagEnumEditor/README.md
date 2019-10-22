# NamedFlagEnumEditor
RadPropertyGridのFlagEnumEditorはDisplayName属性に対応していないので対応するサンプル  
動くところまではとりあえず作成  
![](https://github.com/nosimo/CSharpSapmles/blob/image/images/flag_enum_editor1.png)

## Todo
- コード整理  
ViewModelPropertyChangedを購読する代わりにFlagEnumList.ValueChangedを追加したほうが良いかも  
- パフォーマンス調査  
Boxingとか？

## References
- FlagEnumEditorがDisplay nameに対応していない  
https://www.telerik.com/forums/flagenumeditor-display-enum-values-display-name-in-the-checkbox-list  
- Telerik RadComboBox+CheckBox  
https://www.telerik.com/forums/radcombobox-with-checkbox-with-multi-select-(using-mvvm)  
