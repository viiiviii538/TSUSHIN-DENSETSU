# 通信電設

## アプリケーション構成概要

- **ソリューション:** `src/TSUSHIN-DENSETSU.sln` に WPF アプリケーション プロジェクトをまとめています。
- **プロジェクト:** `TsushinDensetsu.App` は `ViewModels`、`Views`、`Services`、`Domain`、`Infrastructure` などのレイヤー別フォルダーで整理され、UI とロジックを分離します。
- **依存性注入 (DI):** `Infrastructure/ServiceCollectionExtensions.cs` で `Microsoft.Extensions.DependencyInjection` を利用し、`ISpeedTestService` などのインターフェース実装を登録して UI 層が抽象に依存するようにしています。
- **サービス インターフェース:** `Services` フォルダーにネットワーク速度、セキュリティ診断、トポロジー情報を提供する 3 つの基本インターフェースを定義し、テストや実装差し替えが容易です。
- **ドメイン モデル:** `Domain/NetworkDevice.cs` のレコードでネットワーク機器の情報を扱い、ViewModel から UI へ表示します。

## 開発メモ (初心者向け)

- ソリューション (`.sln`) を開くと Visual Studio や Rider でプロジェクト全体を管理できます。
- `ViewModels/MainViewModel.cs` はサービスから取得した文字列やリストをプロパティにまとめ、画面 (`Views/MainWindow.xaml`) でデータバインディングしています。
- DI コンテナは `App.xaml.cs` の起動時にセットアップし、必要なクラスを自動で組み立てます。手動で `new` する代わりにコンテナへ登録するのがポイントです。
