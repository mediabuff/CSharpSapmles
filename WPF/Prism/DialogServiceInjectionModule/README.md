# DialogServiceInjectionModuleサンプル
DialogServiceInjectionをモジュール化したサンプルです  

- Moduleの使い方(コードで登録＋ロードを行う)

を実装  


Moduleの自前登録はBootstrapperのConfigureModuleCatalog関数で行う  
ModuleCAtalog::AddModule関数を使う  
```cs:Bootstrapper.cs
catalog.AddModule(typeof(Module1.Module).FullName, typeof(Module1.Module).AssemblyQualifiedName, InitializationMode.OnDemand);
```

Moduleの読み込みはIModuleManager::LoadModuleを使って行う  
```cs:MainWindowViewModel.cs
moduleManager?.LoadModule(typeof(Module1.Module).FullName);
```



## 環境
VisualStudio2017 15.6.7  
windows10  

## メモ
ModuleCatalog.AddModuleのモジュール名が被ると後勝ち  
⇒被らないようにType.FullNameを使うのが良いかも  

ModuleでPrism.Coreのバージョンが違うとうまく動かないので注意が必要  
⇒ログに例外が出ているのでそれを見つけるか、例外設定を良い感じにしていないと見つけにくい  

ViewModelにModuleViews(object[])を持つのはどうなのか？(MVVM的に)  
⇒Object2UIElementConverterにて表示データへ変換している  

## 参考
今回は「03.Module」を参考  
https://github.com/runceel/PrismEdu  
ConverterにQuickConverterを使おうかと思ったけどエラーで断念  
http://blog.danskingdom.com/dont-write-wpf-converters-write-c-inline-in-your-xaml-instead-using-quickconverter/  
