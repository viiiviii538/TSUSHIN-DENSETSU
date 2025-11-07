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

## 手順 (ビルドと型チェック)

このプロジェクトは C# の WPF アプリケーションで構成されており、TypeScript は使用していません。したがって、Node.js の設定ファイルや `tsconfig.json` は不要で、C# のビルドと型チェックに集中できます。

1. **.NET SDK を準備する:** Microsoft 公式サイトから .NET SDK をインストールします。初めての方はインストーラーの指示に従うだけで大丈夫です。
2. **ソリューションをビルドする:** リポジトリのルートで次のコマンドを実行します。`dotnet build src/TSUSHIN-DENSETSU.sln`
   - このコマンドは C# コードをコンパイルし、同時に型チェックも行います。エラーが表示された場合は型の不一致などが原因なので、メッセージに従って修正してください。
3. **ビルド結果を確認する:** 「Build succeeded」と表示されれば型チェックも通過しています。失敗したときはエラーメッセージのファイル名と行番号を手がかりに、該当箇所を修正しましょう。

> **高校生でもわかるポイント:** `dotnet build` は「プログラムの文法や約束が正しいかどうか」をまとめて確認してくれるテストです。難しく考えず、「宿題を提出する前にスペルチェックする」くらいの感覚で使ってください。
