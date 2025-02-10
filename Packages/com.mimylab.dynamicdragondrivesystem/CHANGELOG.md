# 更新履歴

このプロジェクトに対する注目すべきすべての変更は、このファイルに記録されます。  
[Keep a Changelog](https://keepachangelog.com/en/1.0.0/)のフォーマットと、
[Semantic Versioning](https://semver.org/spec/v2.0.0.html)の採番に則り更新されます。  

## [1.0.4] - 2025/2/10

- **Fixed**
  - PlayOneShot() 系の音量が過剰に小さくなる不具合を修正

## [1.0.3] - 2025/1/20

- **Fixed**
  - 高速飛行時のピッチ回転計算を更に見直し

## [1.0.2] - 2025/1/15

- **Fixed**
  - ピッチ回転の計算見直し、高速飛行時の入力が全てスムーズに繋がるように修正
  - ロール回転のデフォルトを 45°/sec → 60°/sec に調整
  - 加減速の計算見直し、壁にぶつかったとき等に過剰に再加速しようとするのを低減
    - この変更により、`Accerelate Limit` パラメーターは不要となったため削除

## [1.0.1] - 2025/1/9

- **Fixed**
  - 入力選択の初期化処理を見直し
  - TransferToParentSwitchで元の親に戻す時にTransform値も戻るように修正

## [1.0.0] - 2025/1/1

正式リリース

[1.0.4]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/1.0.4
[1.0.3]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/1.0.3
[1.0.2]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/1.0.2
[1.0.1]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/1.0.1
[1.0.0]: https://github.com/mimyquality/DynamicDragonDriveSystem/releases/tag/1.0.0
