# Status — TASK-0020

## Current phase

Verification (Implementasi selesai, kompilasi 0 error; siap untuk smoke test di Unity Editor)

## Completed

- [x] Exploration: Baca GDD §6, §14, dan Pretelan Sistem Mekanik §2 (SYS-19).
- [x] Planning: Membuat `owner.md`, `task-plan.md`, `task-status.md`, `task-review.md`, dan `compile-checking.md`.
- [x] Approval: Approved by User, 2026-11-16.
- [x] Implementation: Menulis script `LevelProgressionManager.cs` di `Assets/Resources/Scripts/System/`.
- [x] Compilation: `dotnet build Assembly-CSharp.csproj --no-restore` (0 errors).

## In progress

- [ ] Smoke test perpindahan stage di Unity Editor (lihat compile-checking.md).

## Blockers

None.

## Changed files

- `Assets/Resources/Scripts/System/LevelProgressionManager.cs`
- `Assembly-CSharp.csproj`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/owner.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/task-status.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/task-review.md`

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Decisions and deviations

- Singleton persistent `DontDestroyOnLoad` untuk mengelola daftar stage (Level 0 sampai 3), pemuatan scene, dan auto-unlock level berikutnya saat `CompleteLevel()`.

## Blockers

None.

## Changed files

- `Assets/Docs/Tasks/task0020-level-progression-abdi/owner.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/task-status.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/task-review.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/compile-checking.md`

## Verification evidence

- Pending.

## Decisions and deviations

- Menggunakan mapping nama scene dan index stage untuk fleksibilitas load scene build settings.
