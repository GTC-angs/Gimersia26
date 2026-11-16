# Review — TASK-0020

## Result

PASS WITH RISKS

## What changed

- Membuat script `Assets/Resources/Scripts/System/LevelProgressionManager.cs`:
  - Mengelola daftar stage Level 0–3 dengan nama scene target.
  - Metode `LoadStage()`, `LoadNextLevel()`, `RestartCurrentLevel()`, `CompleteLevel()`.
  - Properties `CurrentStageIndex`, `TotalStages`, `IsStageUnlocked()`.
  - Event `OnLevelStarted` dan `OnLevelCompleted`.

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Daftar stage Level 0–3 terkonfigurasi dengan benar (menunggu smoke test Editor).
- [ ] Fungsi `LoadNextLevel()` dan `RestartCurrentLevel()` bekerja memuat scene target.
- [ ] Menyediakan API untuk membaca status unlock stage.

## Regression check

- Related feature: Scene navigation.
- Result: 0 build errors.

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Known issues

- Pastikan scene pengujian terdaftar di Build Settings Unity.

## Follow-up tasks

None.
