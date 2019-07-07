# RestoreTabControlSample(作成途中)
Tabの状態を復元するサンプル  
TabItemViewModelをJsonでデシリアライズして復元するだけです  

- Prism
- Json.NET
を使用しています  

# Todo
- Json.NETでのTabItemViewModelのシリアライズ/デシリアライズ  
- ソース整理(TabITemControlViewModelの生成、MainWindowViewModelからの移動)  
<----ここまで最低限  
- アイコン表示とか  
- TabItemContentのPrism.Region対応  
→別のプロジェクトにするかも  



# Referemces
- タブアイテムの右側にAddとCloseボタンを追加する  
https://docs.telerik.com/devtools/wpf/controls/radtabcontrol/howto/how-to-add-close-button-to-the-tab-headers-mvvm  
- タブの右エリアに＋ボタンを追加する（大変そうなので対応してません）  
https://www.telerik.com/forums/how-to-add-button-and-textbox-in-itemspanel-of-radtabcontrol  