# Sasa PDF Relay Printer

PDFデータを受け付ける仮想共有プリンターです。受信した印刷データ（PDF）を、サービス動作PCにインストールされている実プリンターへ転送印刷します。

## ビルド環境とバージョン

- ビルドに使用した IDE：Microsoft Visual Studio Professional 2026 (18.10.4)
- AssemblyVersion：1.0.0.0
- FileVersion：1.0.0.0

## 構成

インストール後、次の構成が作成されます。

- 仮想プリンター名：`Sasa PDF Relay Printer`
- 共有名：`\\<PC名>\SasaPdfRelay`
- 受信ポート：`127.0.0.1:19111`
- 常駐アプリ：`SasaPdfRelayPrinter.exe`

印刷クライアントは共有プリンターへ出力し、サーバーPCでは常駐アプリが PDF を受信して、設定された実プリンターへ印刷します。

## 動作要件

- Windows 10 バージョン2004以降、または Windows 11
- .NET 10 SDK（ビルド時）
- Windowsオプション機能「Microsoft Print to PDF」が有効
- 管理者権限（インストール/アンインストール時）
- Print Spooler サービスが起動していること

## インストール

管理者 PowerShell で次を実行します。

```powershell
Start-Process powershell.exe -Verb RunAs -ArgumentList `
  "-NoProfile -ExecutionPolicy Bypass -File `"$PWD\Install.ps1`""
```

## 初期設定

1. 通知領域の `Sasa PDF Relay Printer` アイコンを右クリック
2. `Select target printer` を選択
3. 転送先の実プリンターを選択して保存

## 使用方法

1. クライアントPCから `\\<サーバーPC名>\SasaPdfRelay` を追加
2. 印刷時に共有プリンター `Sasa PDF Relay Printer` を選択
3. サーバーPC上の実プリンターで出力されることを確認

## アンインストール

管理者 PowerShell で次を実行します。

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\Uninstall.ps1
```

## 制限事項

- 常駐アプリ方式のため、サーバーPCのログオンセッションで受信アプリが起動している必要があります。
- 印刷品質・用紙設定の最終結果は転送先実プリンターのドライバー設定に依存します。
- 本実装はRAW印刷データ中の PDF 本体抽出を前提としています。
