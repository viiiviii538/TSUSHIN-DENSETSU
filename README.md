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

このプロジェクトでは C# の WPF アプリと並行して、フロントエンドや自動化スクリプトのために TypeScript も利用しています。C# と TypeScript の両方をチェックできるよう、以下の順番で準備してください。

1. **Node.js と .NET SDK を準備する:**
   - Node.js は公式サイトから LTS 版をダウンロードし、画面の案内に従ってインストールします。
   - .NET SDK も Microsoft 公式サイトから取得し、同じく案内に従ってインストールします。
2. **JavaScript/TypeScript 依存関係を入れる:** プロジェクトのルートで `npm install` を実行します。`package.json` に書かれた TypeScript などのツールが `node_modules` に入り、`tsc` コマンドが使えるようになります。
3. **TypeScript の型チェックをする:** `npm run type-check` を実行します。`tsconfig.json` の設定にもとづき、`web/` や `scripts/` フォルダー内の TypeScript ファイルをチェックしてくれます。
   - エラーが出た場合はメッセージのファイル名と行番号を確認し、型が合っているか、スペルミスがないかを修正してください。
4. **C# のビルドと型チェックをする:** リポジトリのルートで `dotnet build src/TSUSHIN-DENSETSU.sln` を実行します。
   - このコマンドは C# コードをコンパイルし、同時に型チェックも行います。失敗した場合はエラーメッセージを読んで原因を探り、該当箇所を修正しましょう。
5. **結果を確認する:** `npm run type-check` と `dotnet build` の両方が成功すれば、TypeScript と C# の型チェックが完了です。

> **高校生でもわかるポイント:** `npm run type-check` と `dotnet build` はそれぞれ「国語の文章チェック」と「数学の計算チェック」のようなものです。TypeScript も C# も、宿題を提出する前にミスがないか確認するイメージで実行してください。
