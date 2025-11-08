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

## ローカルでのビルドとエラー対処手順 (初心者向け)

1. **準備:** Windows 環境に .NET SDK 8.0 以上をインストールします。公式サイトのインストーラーを使うと簡単です。
2. **コマンドを実行:** リポジトリのルートで次のコマンドを入力すると、プロジェクト全体をビルドして構文ミスなどをチェックできます。
   ```bash
   dotnet build src/TSUSHIN-DENSETSU.sln
   ```
   - このコマンドは WPF アプリの全コードをまとめてコンパイルし、エラーがないか確認するためのものです。
   - 成功すると「Build succeeded」と表示され、アプリを起動する準備が整ったことを意味します。
3. **エラーが出た場合の確認ポイント:**
   - `dotnet: command not found` と表示されたら .NET SDK が未インストールです。Microsoft の公式サイトから SDK を入れ直してください。
   - Windows 用のワークロード不足と表示されたら、以下で WPF 用コンポーネントを追加できます。
     ```bash
     dotnet workload install wpf
     ```
   - ソースコードのコンパイルエラーが出た場合は、表示されたファイル名と行番号をメモし、該当箇所のタイプミスや未定義クラスがないか確認します。

> **レビュー (高校生でもわかりやすいポイント):** 上の手順は「アプリの材料を全部集めて組み立てる」イメージです。エラー文をそのまま検索すると、原因と解決方法が見つけやすいです。
