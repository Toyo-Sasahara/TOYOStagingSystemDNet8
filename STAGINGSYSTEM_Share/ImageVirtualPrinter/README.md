# Sasa Image Printer

Windowsアプリケーションの印刷出力を、JPEGまたはTIFF画像ファイルとして保存する仮想プリンターです。

## ビルド環境とバージョン

- ビルドに使用した IDE：Microsoft Visual Studio Professional 2026 (18.7.4)
- AssemblyVersion：1.0.0.0
- FileVersion：1.0.0.0

インストールすると、次の2台のプリンターがWindowsに登録されます。

- `Sasa Image Printer (JPEG)`
- `Sasa Image Printer (TIFF)`

印刷した各ページは、400 DPIの個別画像ファイルとして保存されます。標準の保存先は、ユーザーの「ピクチャ」フォルダー内にある `Image Printer Output` です。

## 出力仕様

### JPEG

- 解像度：400 DPI
- カラー画像
- 複数ページ印刷時は、ページごとに個別のJPEGファイルを作成

### TIFF

- 解像度：400 DPI
- 白黒2値（1 bit）
- 圧縮方式：CCITT T.6／Group 4（G4）
- 2値化しきい値：128（128以上を白、128未満を黒として処理）
- 複数ページ印刷時は、ページごとに個別のTIFFファイルを作成

## 動作要件

- Windows 10 バージョン2004以降、またはWindows 11
- Windowsのオプション機能「Microsoft Print to PDF」が有効であること
- インストール時の管理者権限
- ループバック通信用TCPポート19101および19102が使用可能であること

## インストール方法

PowerShellを管理者として起動し、このフォルダーへ移動して次のコマンドを実行します。

```powershell
Start-Process powershell.exe -Verb RunAs -ArgumentList `
  "-NoProfile -ExecutionPolicy Bypass -File `"$PWD\Install.ps1`""
```
インストールが完了すると、Windowsのプリンター一覧にJPEG用とTIFF用の2台が追加され、通知領域に受信アプリのアイコンが表示されます。

## 使用方法

1. 印刷するアプリケーションで「印刷」を選択します。
2. 出力形式に応じて、`Sasa Image Printer (JPEG)`または`Sasa Image Printer (TIFF)`を選択します。
3. 用紙サイズや印刷方向などを設定して印刷します。
4. 変換された画像が出力フォルダーに保存されます。

通知領域のアイコンをダブルクリックすると、出力フォルダーを開けます。アイコンを右クリックして「Settings」を選ぶと、保存先フォルダーを変更できます。

## Visual Studioでのデバッグ方法

### 必要な開発環境

- Visual Studio Professional
- .NET 10 SDK
- Windows 10 SDK 10.0.19041以降
- Visual Studioの「.NETデスクトップ開発」ワークロード
- Windows Forms開発ツール

### 事前準備

F5デバッグで起動するのは印刷受信アプリだけです。Windowsへの仮想プリンター登録は行われないため、最初に管理者PowerShellで`Install.ps1`を実行してください。

`Install.ps1` は PowerShell の `PrintManagement` モジュールを使用します。環境によって `Add-PrinterForm` コマンドが利用できない場合がありますが、その場合は PowerShell での ISO A0/A1 フォーム登録をスキップし、アプリケーション起動時の Win32 API 登録に委ねます。

インストール版の受信アプリが通知領域で動作している場合は、アイコンを右クリックして「Exit」を選択し、終了させます。インストール版が動作したままだと、二重起動防止機能またはTCPポートの競合により、デバッグ版が起動できません。

### F5デバッグの開始

1. Visual Studioで`ImageVirtualPrinter.csproj`を開きます。
2. `ImageVirtualPrinter`をスタートアッププロジェクトに指定します。
3. 必要な箇所にブレークポイントを設定します。
4. F5キーを押してデバッグを開始します。
5. 通知領域に「Sasa Image Printer」のアイコンが表示されたことを確認します。
6. 任意のWindowsアプリケーションから、JPEG用またはTIFF用プリンターを選択して印刷します。

### F5起動時の動作

デバッグ版を起動すると、次の処理が実行されます。

1. 二重起動を防ぐMutexを取得します。
2. `%LOCALAPPDATA%\SasaImagePrinter\settings.json`から設定を読み込みます。設定ファイルがない場合は新規作成します。
3. Windowsの通知領域にアイコンを表示します。
4. 次のループバックTCPポートで印刷データを待ち受けます。
   - JPEG：`127.0.0.1:19101`
   - TIFF：`127.0.0.1:19102`
5. 仮想プリンターから受信したPDF印刷データを、400 DPIの画像へ変換します。
6. 変換した画像を出力フォルダーへ保存します。

F5デバッグを停止すると受信アプリも終了し、それ以降の印刷ジョブは受信できなくなります。F5を実行しただけでは、Windowsのプリンター登録内容は変更されません。

### 主なブレークポイント候補

- `PrintReceiver.cs`の`ProcessAsync`：印刷ジョブの受信、PDF抽出および変換処理の開始
- `PrintReceiver.cs`の`ExtractPdf`：RAW印刷データからのPDF抽出
- `PdfImageConverter.cs`の`ConvertAsync`：PDFページの画像化とファイル保存
- `PdfImageConverter.cs`の`SaveBinaryCcittGroup4TiffAsync`：白黒2値・CCITT Group 4 TIFFの保存
- `PdfImageConverter.cs`の`ConvertToOneBit`：TIFF用の2値化処理

### デバッグ版がすぐ終了する場合

次の項目を確認してください。

- タスクマネージャーに`SasaImagePrinter.exe`が残っていないこと
- インストール版の通知領域アイコンを終了していること
- TCPポート19101および19102を別のアプリケーションが使用していないこと
- Visual Studioの出力ウィンドウおよび例外設定にエラーが表示されていないこと

## アンインストール方法

PowerShellを管理者として起動し、次のコマンドを実行します。

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\Uninstall.ps1
```

アンインストールスクリプトは、登録したプリンター、ポートおよびスタートアップショートカットを削除します。誤って利用者のデータを失わないよう、プログラムファイルと出力済み画像は削除しません。

## システム構成

Windows標準の署名済み`Microsoft Print To PDF`ドライバーを使用して、印刷データをPDFとして生成します。ユーザーモードで動作する受信アプリがループバック通信でPDFを受け取り、Windows標準のPDF・画像処理APIを使用して各ページをJPEGまたはTIFFへ変換します。

独自のカーネルモードプリンタードライバーはインストールしません。

### プリンターフォーム登録

ISO A0・A1 などの大判用紙サイズをシステムに登録するため、以下の 2 つの方法を採用しています：

- **PowerShell 統合**（Install.ps1）：インストール時に `Add-PrinterForm` コマンドレットで登録
- **Win32 API 統合**（C# / winspool.drv）：アプリケーション起動時に `AddForm` API で再登録・確認

この 2 段階アプローチにより、インストール失敗時の自動回復と、ユーザーモードでの再登録が可能になります。

## A0サイズについて

画像変換処理はA0サイズを扱えるように設計されています。A0を400 DPIで出力した場合の画像寸法は、約13,244 × 18,724ピクセルです。

大判印刷では多くのメモリを使用します。また、印刷元アプリケーションおよびWindowsプリンタードライバー側で、対象の用紙サイズを選択できる必要があります。

### ISO A0・A1 自動登録

バージョン 2.0 以降、アプリケーション起動時に ISO A0・A1 フォーム（用紙サイズ）が自動的にシステムに登録されます。

- **ISO A0**：841 × 1189 mm
- **ISO A1**：594 × 841 mm

この登録は以下の 2 段階で行われます：

1. **インストール期間**（Install.ps1）：PowerShell の `Add-PrinterForm` コマンドレットで登録
2. **アプリケーション起動時**：C# の Win32 API（`winspool.drv`）経由で登録

既に登録されているフォームの重複登録は自動的にスキップされるため、安全です。

## 現在の制限事項

- JPEGとTIFFの選択は、印刷時に使用するプリンターを切り替えて行います。
- 複数ページTIFFには対応しておらず、1ページにつき1ファイルを作成します。
- 印刷を受信するには、サインイン中のユーザーセッションで受信アプリが起動している必要があります。
- プリンター共有およびWindows Server上での無人サービス運転には対応していません。
- A0・A1は登録されていますが、印刷元アプリケーションおよび Microsoft Print to PDF ドライバー側でこれらのサイズをサポートしている必要があります。
