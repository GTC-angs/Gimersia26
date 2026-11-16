# Review — TASK-0019

## Result

PASS WITH RISKS

## What changed

- Membuat script `Assets/Resources/Scripts/System/LevelTimeManager.cs`:
  - Hitung mundur waktu level `_levelDuration` (180s) dengan akurat.
  - Event `OnTimeUpdated`, `OnCriticalTimeEntered` (≤ 30s), dan `OnTimeOut` (00:00).
  - Metode `PauseTimer()`, `ResumeTimer()`, `AddBonusTime()`, dan `SetDuration()`.

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Timer menghitung mundur dengan benar dari durasi awal hingga 0 (menunggu smoke test Editor).
- [ ] `PauseTimer()` dan `ResumeTimer()` menghentikan dan melanjutkan countdown.
- [ ] Event kritis terpicu saat sisa waktu ≤ 30 detik.
- [ ] Event `OnTimeOut` terpicu tepat saat waktu mencapai 0.

## Regression check

- Related feature: UI HUD Jam Dinding (`WallClockTimerUI`).
- Result: 0 build errors.

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Known issues

- Perlu smoke test di Editor.

## Follow-up tasks

None.
