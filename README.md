# Ext-Chg-Detect

![動作画面](https://raw.githubusercontent.com/komugikotan/ext-chg-detect/main/picture.png "動作画面")

指定されたフォルダ内で特定のファイル（画像ファイルなど）の拡張子が変更されたことを検知して、自動的にファイル形式を変換してくれるアプリケーションです。

## 概要

主となる実行ファイルは、「ext-chg-detect.exe」です。

初回起動では、監視するフォルダの設定画面が表示されます。設定しない場合、アプリケーションが終了します。設定した場合、続けてスタートアップアプリに登録するかの設定が表示されます。

次の二つの設定を完了している場合は、次回以降の起動では設定画面が表示されません。設定は、dataフォルダの「info.ini」に記録されています。後から、「ext-change-detect.exe」を実行し設定画面を表示することで設定を変更できます。

設定が完了しアプリが起動すると、アプリが終了したかのように見えるかもしれませんが、<span style="font-size: 200%;"><b>バックグラウンドで動作しています</b></span>。アプリが終了したわけではありません。

「ext-change-detect.exe」は、[このリポジトリ](https://github.com/komugikotan/ext-chg-detect-settings)にMITライセンスでアップロードされています。こちらも使用は自己責任です。

## 対応ファイル

現在、次の3つの拡張子に対応しています。相互変換が可能です。（png->webp, png->jpg, jpg->png, webp->jpgなど）

・png
・jpg (jpeg)
・webp

## プロジェクトをダウンロードして開発する

開発環境はWindows 11 Pro上のVisual Studio Community 2022の、.NET デスクトップ開発環境です。

### Gitリポジトリをクローン

このGitリポジトリをクローンし、Visual Studioで開いてください。

### 必要フォルダ等の作成

DebugフォルダまたはReleaseフォルダ（要は、実行ファイルが生成される場所）に、次の二つのフォルダを作成してください。

・tmp
・data

作成した後、dataフォルダ内に次の内容のiniファイルを作成してください。ファイル名は、info.iniで、内容は以下です。

```
[Basic]
path=
startup=false
```

このiniファイルには、スタートアップに設定するかどうかと、監視するフォルダのパスが保存されています。

繰り返しになりますが、アプリが起動して設定も完了した段階では**バックグラウンドで**動作します。アプリが終了したわけではありません。

## Contribution

プルリクエストやイシューは、遠慮なく送ってください。マージするかもしれません。
