# 更新履歴

このプロジェクトに対する注目すべきすべての変更は、このファイルに記録されます。  
[Keep a Changelog](https://keepachangelog.com/en/1.0.0/)のフォーマットと、
[Semantic Versioning](https://semver.org/spec/v2.0.0.html)の採番に則り更新されます。  

## [0.12.1] - 2024/6/13

- **Fixed**
  - VR Handleの後方加速入力が前方加速になっていたのを修正

## [0.12.0] - 2024/6/12

- **Added**
  - 操作方法にVR Handleを追加
    - これに合わせて、VRHandsをVR Leverに改名

## [0.11.0] - 2024/5/12

- **Changed**
  - iOS版に対応 (recommend VRCSDK 3.5.2)
    - 副次的に、プレイヤーの入力デバイスに応じた入力モード選択が増えるようになった

## [0.10.7] - 2024/5/6

- **Changed**
  - Legacy, Thumbsticksのデフォルトの上下入力設定を反転

- **Fixed**
  - 上記に合わせてインプットメニューの上下入力の表記を修正
  - VRHandsの首振り入力の挙動を改善

## [0.10.6] - 2024/5/2

- **Fixed**
  - 首のスムージング処理を更に改善
  - Ownerの速度・加速度変数にスムージング処理追加

## [0.10.5] - 2024/4/30

- **Fixed**
  - ネットワーク同期がスパム状態になっていたのを修正
  - 首振り周りの処理を全体的に見直し
    - VRHandsの首振り入力を等速に変更
    - 他人(ドラゴンの非Owner)視点の首振りのスムージング処理改善
    - 地上走行時の首の縦振り挙動改善

## [0.10.4] - 2024/4/23

- **Fixed**
  - Input Configのダブり対策

## [0.10.3] - 2024/4/22

- **Added**
  - ワールド設置用の操作手順書Prefabを同梱

- **Fixed**
  - ドラゴンの設置量が多いと通信負荷が跳ね上がるのを対策

## [0.10.2] - 2024/4/2

- **Changed**
  - Splash Effectの法面(Normal)指定をローカル座標からの計算に変更
  - 法面指定のデフォルトが(0, 1, 0)になってないのを修正

## [0.10.0] - 2024/3/31

- **Added**
  - `Splash Effect by Player` と `Splash Effect by Dragon` を追加
  - ↑のサンプルプレハブを追加

## [0.9.2] - 2024/3/20

- **Fixed**
  - リスポーンスイッチのマテリアルがサードパーティ製シェーダーになっていたのを修正
  - DragonDriverの初期値変更

## [0.9.1] - 2024/2/20

- **Fixed**
  - velocity/angularVelocityの受け渡し処理を見直し
  - 誰も乗っていない時にドラゴンの状態が更新されないのを修正

## [0.9.0] - 2024/2/18

- **Added**
  - 座席が(他人から見た時だけ)指定したボーンに追従できる機能を追加
  - 一部パラメーターをVRC内からでもチューニングできる機能を追加

## [0.8.4] - 2024/2/18

- **Fixed**
  - DragonActorの同期負荷を見直し最適化

## [0.8.3] - 2024/2/17

- **Fixed**
  - 高速飛行時、真上・真下を向いたときに過剰に横旋回が効くのを修正
  - DragonActorがAnimatorに存在しないパラメーターを無視するように修正

## [0.8.2] - 2024/2/16

- **Changed**
  - 同期周りを最適化、これを受けて一部パラメーターをより適切なコンポーネントに移動

## [0.8.1] - 2024/2/14

- **Added**
  - DragonActorに `IsOwner` と `IsLocal` を追加

- **Fixed**
  - 座席から降りる時に座位置調整機構がトグルしないよう調整
  - `IsGrounded` 判定をローカルで見るよう修正

## [0.8.0] - 2024/2/14

- **Changed**
  - ジャンプボタン短押しで座位置調整機構をオンオフできるように変更
  - 高速飛行時のエレベーター入力を、ドラゴン基準からワールド基準に変更

## [0.7.12] - 2024/2/12

- **Fixed**
  - Input Configの表示を微修正

## [0.7.11] - 2024/1/22

- **Fixed**
  - ビルドし直した時に接地判定が消失する事があるバグを修正

## [0.7.10] - 2024/1/20

- **Fixed**
  - DebugDragonの追加修正
  - DebugDragon用のAnimationClipが含まれていなかったのを追加

## [0.7.9] - 2024/1/19

- **Added**
  - Dragon Actorに新パラメーター追加(Speed, Angular系)

- **Fixed**
  - Dragon Actorの一部パラメーターの同期を見直し

## [0.7.8] - 2024/1/17

- **Added**
  - Dragon Songコンポーネント追加

- **Fixed**
  - 同期系コンポーネントと一緒に付く補助コンポーネントのBehaviourSyncModeをNoVariableSyncに変更

## [0.7.7] - 2024/1/15

- **Fixed**
  - Gaze入力モードの安定性向上

- **Changed**
  - VRHandsモードの回転入力は、首振り操作に対して回転速度では無く回転量を直接反映するように変更

## [0.7.6] - 2024/1/11

- **Fixed**
  - VRHands入力モードの安定性向上(これにより、急ブレーキが利かないことがあるバグも修正)

## [0.7.5] - 2024/1/10

- **Fixed**
  - ~~ビルドし直した時に接地判定が消失する事があるバグをたぶん修正~~
  - AddComponentメニューにDynamicDragonDriveSystemのカテゴリーを用意

## [0.7.4] - 2024/1/9

- **Added**
  - デバッグ用兼サンプルPrefab追加

- **Fixed**
  - Velocity系の計算式見直し、安定性向上

## [0.7.3] - 2024/1/8

- **Fixed**
  - ジャンプ中にドラゴンから降りるとアニメーションが停止するバグを修正

## [0.7.2] - 2024/1/4

- **Changed**
  - メニューにジャンプ操作、急ブレーキ操作についてTIPSを追加

## [0.7.1] - 2024/1/3

- **Added**
  - キャノピー実装

- **Changed**
  - Vive Wand入力の初期配置を変更
  - メニューの表記をより分かりやすく変更

- **Fixed**
  - 入力設定をGazeから他に切り替えた時に回転操作が効かなくなるバグを修正

## [0.7.0] - 2023/12/30

- **Changed**
  - Unity2022.3以降、VRCSDK3.5.0以降にサポートバージョンを引き上げ

## [0.6.5] - 2023/12/20

- **Fixed**
  - スマホ版でドラゴンから降りられないのを対処(ジャンプボタンをダブルタップで降りる)
    既知の不具合：1回目のタップ時点で小ジャンプしてしまうため、ドラゴンが地面から浮いた状態になる

## [0.6.4] - 2023/12/8

- **Fixed**
  - インプット設定メニューのアイコンを変更

## [0.6.3] - 2023/12/7

- **Added**
  - インプット設定メニューのUIをビジュアルリッチに更新

## [0.6.2] - 2023/12/4

- **Fixed**
  - 速度パラメーターの単位をm/sで統一
  - 座高調節機能の入力を配置変更(どのモードでも使えるようにする為の措置)
  - メニューのオンオフをドラゴンへの騎乗と連動するように変更

## [0.6.1] - 2023/11/23

- **Fixed**
  - 座っている間、Sit判定が無効になるよう修正

## [0.6.0] - 2023/11/22

- **Added**
  - メニューに追加メニュー呼び出し用の機構を追加

- **Fixed**
  - 環境によってネームスペースが干渉する可能性があるのに対応

## [0.5.2] - 2023/11/1

- **Changed**
  - リスポーンスイッチをクソデカに変更
  - ドラゴンがリスポーン近くに居る間はリスポーンスイッチ非表示になるよう変更

## [0.5.1] - 2023/10/31

- **Changed**
  - プラットフォームに合わせて入力操作の選択肢を制限するように変更

## [0.5.0] - 2023/10/30

- **Added**
  - 操作入力モードにGazeを追加
    - プレイヤーの見ている方向にドラゴンの進行方向が一致する
    - この入力モードの時はFlight状態にならなくなる

- **Known issues**
  - ClientSim上でのみGazeモードが動かない
  - Gazeモードを選択してから別のモードを選択すると、一部の入力操作を受け付けなくなる
    - 更に別のモードを選択してから選択したいモードを再選択すると治る

## [0.4.2] - 2023/10/28

- **Changed**
  - 衝突時の傾き判定をドラゴン自身の姿勢基準に変更

## [0.4.1] - 2023/10/27

- **Changed**
  - 衝突相手の傾きを衝突判定の判断材料に追加、これに伴い地上走行中も衝突するように変更
  - 初代Viveコントローラー用入力のデフォルト操作を一部変更
  - 初代Viveコントローラー用入力の急ブレーキ操作を変更(左パッド←＋右パッド↓)
  - 遷移可能な移動状態を取捨選択できるようにプロパティ追加

## [0.4.0] - 2023/10/23

- **Added**
  - 操作系統に初代Viveコントローラー用の選択肢を追加

## [0.3.0] - 2023/10/16

- **Added**
  - ジャンプと衝突アニメーションを追加
  - 衝突アニメーション用にスクリプトを追加・調整

- **Fixed**
  - メニューがドラゴンの初期値を反映するように修正

## [0.2.2] - 2023/10/13

- **Fixed**
  - メニューから入力設定を切り替えても反映されなかったのを修正

## [0.2.1] - 2023/10/12

- **Fixed**
  - メニューがうまく操作できなかったのを修正

## [0.2.0] - 2023/10/11

- **Changed**
  - メニューを追加し、各種調整項目を集約

## [0.1.2] - 2023/10/7

- **Fixed**
  - Velocity系の値がチャタるのを修正
  - スイッチ類の見た目をUIレイヤーに変更(カメラに写さない対応)

## [0.1.1] - 2023/10/6

- **Fixed**
  - Velocityが同期するように修正
  - 着座位置が同期するように修正
  - 操縦席は座った時だけ座高調節スイッチが出現し、スイッチを押して有効にしている間だけ座高調節可能になるよう修正

## [0.1.0] - 2023/10/5

- **Added**
  - すべての座席に座高調整機能を追加
    - 操縦席はハンドル下のスイッチでドラゴンの操縦と切替式
    - デスクトップモードはWASDで水平方向の調整、↑↓で座高の調整
    - VRモードは左スティックで水平方向の調整、右スティック上下で座高の調整

## [0.0.5] - 2023/10/4

- **Changed**
  - Animator Controller更新

## [0.0.1] - 2023/10/1

- **Added**
  - VPM化

[0.12.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.12.1
[0.12.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.12.0
[0.11.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.11.0
[0.10.7]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.10.7
[0.10.6]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.10.6
[0.10.5]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.10.5
[0.10.4]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.10.4
[0.10.3]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.10.3
[0.10.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.10.2
[0.10.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.10.0
[0.9.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.9.2
[0.9.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.9.1
[0.9.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.9.0
[0.8.4]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.8.4
[0.8.3]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.8.3
[0.8.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.8.2
[0.8.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.8.1
[0.8.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.8.0
[0.7.12]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.12
[0.7.11]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.11
[0.7.10]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.10
[0.7.9]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.9
[0.7.8]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.8
[0.7.7]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.7
[0.7.6]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.6
[0.7.5]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.5
[0.7.4]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.4
[0.7.3]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.3
[0.7.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.2
[0.7.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.1
[0.7.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.7.0
[0.6.5]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.6.5
[0.6.4]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.6.4
[0.6.3]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.6.3
[0.6.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.6.2
[0.6.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.6.1
[0.6.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.6.0
[0.5.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.5.2
[0.5.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.5.1
[0.5.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.5.0
[0.4.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.4.2
[0.4.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.4.1
[0.4.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.4.0
[0.3.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.3.0
[0.2.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.2.2
[0.2.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.2.1
[0.2.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.2.0
[0.1.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.1.2
[0.1.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.1.1
[0.1.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.1.0
[0.0.5]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.0.5
[0.0.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/0.0.1
