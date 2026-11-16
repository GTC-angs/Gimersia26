# Status — TASK-0019

## Current phase

Verification (Implementasi selesai, kompilasi 0 error; siap untuk smoke test di Unity Editor)

## Completed

- [x] Exploration: Baca GDD §5, Pretelan Sistem Mekanik §2 (SYS-18), dan §4.1.1 (WallClockTimerUI).
- [x] Planning: Membuat `owner.md`, `task-plan.md`, `task-status.md`, `task-review.md`, dan `compile-checking.md`.
- [x] Approval: Approved by User, 2026-11-16.
- [x] Implementation: Menulis script `LevelTimeManager.cs` di `Assets/Resources/Scripts/System/`.
- [x] Compilation: `dotnet build Assembly-CSharp.csproj --no-restore` (0 errors).

## In progress

- [ ] Smoke test hitung mundur waktu level di Unity Editor (lihat compile-checking.md).

## Blockers

None.

## Changed files

- `Assets/Resources/Scripts/System/LevelTimeManager.cs`
- `Assembly-CSharp.csproj`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/owner.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/task-status.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/task-review.md`

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Decisions and deviations

- Countdown menggunakan event C# `OnTimeUpdated(remaining, total)` dan memicu `OnCriticalTimeEntered` saat sisa waktu ≤ 30 detik serta `OnTimeOut` saat mencapai 0.

## Blockers

None.

## Changed files

- `Assets/Docs/Tasks/task0019-level-timer-abdi/owner.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/task-status.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/task-review.md`
- `Assets/Docs/Tasks/task0019-level-timer-abdi/compile-checking.md`

## Verification evidence

- Pending.

## Decisions and deviations

- Menggunakan arsitektur event-driven `OnTimeUpdated(float remaining, float total)` agar UI tidak perlu melakukan polling setiap frame di `Update()`.
