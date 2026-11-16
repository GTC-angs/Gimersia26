# Status — TASK-0011

## Current phase

Verification (Implementasi selesai, kompilasi 0 error; siap untuk smoke test di Unity Editor)

## Completed

- [x] Exploration: Baca referensi GDD, Pretelan Sistem Mekanik §2.3 (SYS-09 & SYS-10), dan VFX List §3.6.
- [x] Planning: Membuat `owner.md`, `task-plan.md`, `task-status.md`, `task-review.md`, dan `compile-checking.md`.
- [x] Approval: Approved by User, 2026-11-16.
- [x] Implementation: Menulis script `ItemOutOfBoundsRecoverer.cs` di `Assets/Resources/Scripts/System/`.
- [x] Compilation: `dotnet build Assembly-CSharp.csproj --no-restore` (0 errors, 0 warnings).

## In progress

- [ ] Smoke test deteksi jatuh dan recovery di Unity Editor (lihat compile-checking.md).

## Blockers

None.

## Changed files

- `Assets/Resources/Scripts/System/ItemOutOfBoundsRecoverer.cs`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/owner.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/task-status.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/task-review.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/compile-checking.md`

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ **Build succeeded (0 Error(s), 0 Warning(s))**.

## Decisions and deviations

- Menggunakan dua metode deteksi: Trigger Collider (OnTriggerEnter) + Height Check interval (`Y < _deathYThreshold`) agar objek yang menembus collider saat jatuh kencang tetap tertangkap.
- Posisi dan rotasi spawn awal dicatat otomatis saat `Start()` via `TrackAllSceneRigidbodies()`.

## Blockers

None.

## Changed files

- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/owner.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/task-status.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/task-review.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/compile-checking.md`

## Verification evidence

- Pending implementation.

## Decisions and deviations

- Menggabungkan SYS-09 (deteksi OOB) dan SYS-10 (pemulihan & VFX asap) ke dalam satu script terpadu `ItemOutOfBoundsRecoverer.cs` sesuai spesifikasi pretelan mekanik.
