﻿# DialogServiceInjectionサンプル
Prism入門としてDialogServiceをインジェクションするサンプルです  

- BootStrapperの使い方
- ViewModelLocatorの使い方
- Unityの使い方

あたりを実装  

IDialogServiceを実装するDialogServiceとConfirmDialogServiceの2つを用意して  
MainWindowViewModelにはDialogService  
UserControl1ViewModelにもDialogService  
UserControl2ViewModelにはConfirmDialogService  
をそれぞれDIしています  
ViewModel自体もViewModelLocatorにより自動設定する  

## 環境
VisualStudio2017 15.6.7  
windows10  

## メモ
IoCコンテナにはUnityを使用しています  
→メンテされなくなったらしいですが、まぁまだUnityで良い気がします  
→もし乗り換えが必要ならMicroResolverあたりを見てみたい(Autofacはなんか使いにくそう)  

以下のxamlコードは  
```xml
xmlns:prism="http://prismlibrary.com/"
prism:ViewModelLocator.AutoWireViewModel="True"
```  
コードビハインドに
```cs
Prism.MvvM.ViewModelLocator.SetAutoWireViewModel(this, true);
```
と書くのと同じ  

ViewModelLocatorというかUnityを使ってアプリが終了しなくなったとかの場合、  
IoCコンテナに登録している型でコンストラクタのデフォルト引数があるやつは要注意  
意図しない引数が渡ってきてる可能性があるのでそこらへんを確認する  
（コンストラクタにデフォルト引数があるのが良くないかも）  

そういえばなんでPrism.WPFにダイアログサービスってないんでしょうね…  
InteractionRequest推しなんでしょうかね。XamarinのほうにはPageDialogService的なものがあって便利そう  

## 参考
今回はBootstrapperを使おう、ViewModelLocatorを使おうを参考  
https://github.com/runceel/PrismEdu  
同じ型に複数のインスタンスを注入する方法（⇒名前付きで対応）  
http://nosu.hatenadiary.jp/entry/2017/09/04/000345  
ResolverOverrideの使い方（⇒コンストラクタインジェクションの場合はPramaeterOverrideを使う）  
http://d.hatena.ne.jp/toshi_m/20100924/1285323581  
