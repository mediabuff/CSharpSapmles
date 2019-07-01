# WindowAlert
RadDesktopAlertManagerをWindow向けに拡張したRadWindowAlertManagerサンプルです  

使用するには以下のコンポーネントを参照に追加する必要があります  
- Telerik.Windows.Controls.dll
- Telerik.Windows.Controls.Navigation.dll
- Telerik.Windows.Data.dll

## 使い方
```cs
using Telerik.Windows.Controls;

manager = new RadWindowAlertManager(this, AlertScreenPosition.TopLeft);
manager.ShowAlert("messages...");
```
||||
|---|---|---|
|![](https://github.com/nosimo/CSharpSapmles/blob/image/images/window_alert_bottom_right1.png)|![](https://github.com/nosimo/CSharpSapmles/blob/image/images/window_alert_bottom_right3.png)|![](https://github.com/nosimo/CSharpSapmles/blob/image/images/window_alert_top_left3.png)|

## 参考
https://docs.telerik.com/devtools/wpf/controls/raddesktopalert/overview  
