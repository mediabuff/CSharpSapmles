# WindowAlert
RadDesktopAlertManagerをWindow向けに拡張したRadWindowAlertManagerサンプルです  

使用するには以下のコンポーネントを参照に追加する必要があります  
- Telerik.Windows.Controls.dll
- Telerik.Windows.Controls.Navigation.dll
- Telerik.Windows.Data.dll

## Features
RadDeskTopAlertがDesktopに対するAlert(トースト)なのに対して、こちらはWindowに対するAlertです。  
- Windowの高さいっぱいにAlertが埋まると次にAlertが表示された場合、古い順にAlertは削除されます
- OwnerWindowの位置を移動、サイズを変更すると表示されていたAlertは削除されます
- ShowAlert(String)の最小引数でAlertを追加できます
  
データの上書き保存やバックグランド処理完了時など、ログよりも強めのインフォを行いたい場合に使うと良いんじゃないでしょうか。  

## Usage
```cs
using Telerik.Windows.Controls;

manager = new RadWindowAlertManager(ownerWindow, AlertScreenPosition.TopLeft);
manager.ShowAlert("messages...");
```
|BottomRight Alert|BottomRight Alert3|TopLeft Alert3|
|---|---|---|
|![](https://github.com/nosimo/CSharpSapmles/blob/image/images/window_alert_bottom_right1.png)|![](https://github.com/nosimo/CSharpSapmles/blob/image/images/window_alert_bottom_right3.png)|![](https://github.com/nosimo/CSharpSapmles/blob/image/images/window_alert_top_left3.png)|

## Reference
https://docs.telerik.com/devtools/wpf/controls/raddesktopalert/overview  
