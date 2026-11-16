# TASK-0019 — Level Timer System (SYS-18)

## Goal

Mengimplementasikan sistem pengatur waktu level (`LevelTimeManager.cs`):
- Hitung mundur durasi level (misal 180 detik) secara akurat.
- Mendukung fungsi `PauseTimer()`, `ResumeTimer()`, dan `AddBonusTime(float seconds)`.
- Memicu event perubahan waktu per detik: `OnTimeUpdated(float remainingSeconds, float totalSeconds)`.
- Memicu event peringatan kritis saat waktu tersisa ≤ 30 detik: `OnCriticalTimeEntered`.
- Memicu kondisi timeout (waktu habis 00:00): `OnTimeOut`.

## User behavior

- Di awal level, waktu mulai menghitung mundur dari durasi awal (misal 3 menit).
- Nilai waktu di-update realtime ke HUD Jam Dinding.
- Saat sisa waktu ≤ 30 detik, sistem memberi sinyal state kritis (lampu/timer berkedip dan BGM bertransisi tegang).
- Saat waktu mencapai 00:00, level berakhir dan tamu tiba.

## Evidence

- GDD §5.1 (Core Loop), §9 (Audio Direction <30s), §13 (Win/Lose condition).
- Pretelan Sistem Mekanik §2 (SYS-18 Level Timer [P0 - CORE MVP]) dan §4.1.1 (`WallClockTimerUI`).
- Agent rules: `Assets/Docs/Agentrule.md`.

## Scope

### In scope

- Membuat script `Assets/Resources/Scripts/System/LevelTimeManager.cs`.
- Durasi level konfigurabel di Inspector.
- Unscaled time support saat game di-pause.
- C# Events: `OnTimeUpdated`, `OnCriticalTimeEntered`, `OnTimeOut`, `OnTimerStateChanged`.
- Singleton instance opsional untuk kemudahan akses.

### Out of scope

- Rendering grafis jarum jam / TextMeshPro (di-handle oleh `WallClockTimerUI.cs`).

## Technical approach

Menggunakan `Update()` dengan `Time.deltaTime` saat `_isRunning` aktif:
- Hitung sisa waktu `_remainingTime = Mathf.Max(0f, _remainingTime - Time.deltaTime)`.
- Pantau ambang kritis `_criticalThreshold = 30f`.

## Files expected to change

- `Assets/Resources/Scripts/System/LevelTimeManager.cs`
- `Assembly-CSharp.csproj`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/task-status.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/task-review.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/compile-checking.md`

## Interfaces and state

- Properties: `RemainingTime`, `TotalDuration`, `IsRunning`, `IsPaused`, `IsCriticalTime`.
- Methods: `StartTimer()`, `PauseTimer()`, `ResumeTimer()`, `AddBonusTime()`, `StopTimer()`.

## Invariants

- Waktu tidak pernah bernilai negatif.
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Sistem inti gameplay loop dan timer (SYS-18).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Timer menghitung mundur dengan benar dari durasi awal hingga 0.
- [ ] `PauseTimer()` dan `ResumeTimer()` menghentikan dan melanjutkan countdown.
- [ ] Event kritis terpicu saat sisa waktu ≤ 30 detik.
- [ ] Event `OnTimeOut` terpicu tepat saat waktu mencapai 0.
