# Dynamic Dragon Drive Sytstem

## 概要

VRChatには乗り物は数あれど、生物に乗って、更に空まで飛べるギミックって少ないよね。という事で、作りました。ドラゴンに乗って地を駆け、空を飛ぶ事が出来るギミックです。  
各種入力デバイスに合わせた操作方法を取りそろえているので、VR、デスクトップ、モバイルどのプラットフォームからでもシンプルで直感的な操作が可能です。  
VR酔い対策にも力を入れており、軽量で快適な空の旅をお楽しみいただけます。  
モデルとアニメーションを差し替える事で、様々な生物/無機物を乗り物にすることができます。また、飛行機能をオフにして地上走行専用の乗り物とする等も可能です。  

> [!WARNING]
> ドラゴンのモデルやアニメーションは付属しません。各自ご用意ください。  

## 導入手順

### リポジトリーのインポート

> [!NOTE]
> 既に MimyLab のリポジトリーをインポートしている場合はこの手順をスキップできます。[VPM パッケージのインポート](#vpmパッケージのインポート)へ進んでください。

VCC(VRChat Creator Companion)または [ALCOM](https://vrc-get.anatawa12.com/ja/alcom/) から、以下の**どちらか一つ**の手順を行うことで MimyLab のリポジトリーをインポートできます。  
(ALCOM 利用者は適宜読み替えてください)  

- <https://vpm.mimylab.com/> へアクセスし、「Add to VCC」から `https://vpm.mimylab.com/index.json` を追加
- VCC のウィンドウで `Setting -> Packages -> Add Repository` の順に開き、`https://vpm.mimylab.com/index.json` を追加

<img width="1863" height="1112" alt="image" src="https://github.com/user-attachments/assets/712b1736-0467-44b7-9646-0fe0a30add56" />

[VPM CLI](https://vcc.docs.vrchat.com/vpm/cli/) を使用してインポートする場合、コマンドラインを開き以下のコマンドを入力してください。

```text
vpm add repo https://vpm.mimylab.com/index.json
```

### VPMパッケージのインポート

VCC から任意のプロジェクトを選択し、「Manage Project」から Manage Packages 画面に移動します。  
読み込んだパッケージが一覧に出てくるので、 **Dynamic Dragon Drive Sytstem** の右にある「＋」ボタンを押すか「Installed Version」から直接バージョンを選ぶことで、プロジェクトにインポートします。  
このとき一覧に出てこない場合は、右上の [Selected Repos] から MimyLab リポジトリーのチェックが外れていないか確認してください。  

リポジトリーを使わずに導入したい場合は、[Release](https://github.com/mimyquality/DynamicDragonDriveSystem/releases) から unitypackage ファイルをダウンロードして、プロジェクトにインポートしてください。  

### セットアップ手順

公式取説本を以下にて販売しています。  
利用にあたって購入は必須ではありませんが、セットアップに関するサポートはBOOTHの方での対応のみとなります。  
※機能要望やバグ報告はこのGitHubリポジトリーの Issue や Pull requests へお願いします。  

[Dynamic Dragon Drive System 取扱説明書](https://mimyquality.booth.pm/items/5624579)

## ライセンス

[LICENSE](LICENSE.md)

## 更新履歴

[CHANGELOG](CHANGELOG.md)

## クレジット

### UIデザイン、素材協力

ポテト <https://x.com/animekki>  

### 発案、モデル・アニメーション協力

すくりゅー <https://twitter.com/screw0u0>
