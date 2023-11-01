# 更新履歴
このプロジェクトに対する注目すべきすべての変更は、このファイルに記録されます。  
[Keep a Changelog](https://keepachangelog.com/en/1.0.0/)のフォーマットと、
[Semantic Versioning](https://semver.org/spec/v2.0.0.html)の採番に則り更新されます。  
利用手順は[こちら](https://github.com/mimyquality/FukuroUdon/wiki)からご確認ください。

## [0.5.2] - 2023/11/1
- Changed
  - リスポーンスイッチをクソデカに変更
  - ドラゴンがリスポーン近くに居る間はリスポーンスイッチ非表示になるよう変更

## [0.5.1] - 2023/10/31
### Changed
- プラットフォームに合わせて入力操作の選択肢を制限するように変更

## [0.5.0] - 2023/10/30
### Add
- 操作入力モードにGazeを追加
  - プレイヤーの見ている方向にドラゴンの進行方向が一致する
  - この入力モードの時はFlight状態にならなくなる

### Known issues
- ClientSim上でのみGazeモードが動かない
- Gazeモードを選択してから別のモードを選択すると、一部の入力操作を受け付けなくなる
  - 更に別のモードを選択してから選択したいモードを再選択すると治る

## [0.4.2] - 2023/10/28
### Changed
- 衝突時の傾き判定をドラゴン自身の姿勢基準に変更

## [0.4.1] - 2023/10/27
### Changed
- 衝突相手の傾きを衝突判定の判断材料に追加、これに伴い地上走行中も衝突するように変更
- 初代Viveコントローラー用入力のデフォルト操作を一部変更
- 初代Viveコントローラー用入力の急ブレーキ操作を変更(左パッド←＋右パッド↓)
- 遷移可能な移動状態を取捨選択できるようにプロパティ追加

## [0.4.0] - 2023/10/23
### Add
- 操作系統に初代Viveコントローラー用の選択肢を追加

## [0.3.0] - 2023/10/16
### Add
- ジャンプと衝突アニメーションを追加
- 衝突アニメーション用にスクリプトを追加・調整
### Fixed
- メニューがドラゴンの初期値を反映するように修正

## [0.2.2] - 2023/10/13
### Fixed
- メニューから入力設定を切り替えても反映されなかったのを修正

## [0.2.1] - 2023/10/12
### Fixed
- メニューがうまく操作できなかったのを修正

## [0.2.0] - 2023/10/11
### Changed
- メニューを追加し、各種調整項目を集約

## [0.1.2] - 2023/10/7
### Fixed
- Velocity系の値がチャタるのを修正
- スイッチ類の見た目をUIレイヤーに変更(カメラに写さない対応)

## [0.1.1] - 2023/10/6
### Fixed
- Velocityが同期するように修正
- 着座位置が同期するように修正
- 操縦席は座った時だけ座高調節スイッチが出現し、スイッチを押して有効にしている間だけ座高調節可能になるよう修正

## [0.1.0] - 2023/10/5
### Add
- すべての座席に座高調整機能を追加
  - 操縦席はハンドル下のスイッチでドラゴンの操縦と切替式
  - DTモードはWASDで水平方向の調整、↑↓で座高の調整
  - VRモードは左スティックで水平方向の調整、右スティック上下で座高の調整

## [0.0.5] - 2023/10/4
### Changed
- Animator Controller更新

## [0.0.1] - 2023/10/1
### Add
- VPM化

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