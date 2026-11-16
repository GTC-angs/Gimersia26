# Status — TASK-0021

## Current phase

Verification (Implementasi selesai, kompilasi 0 error; siap untuk smoke test di Unity Editor)

## Completed

- [x] Exploration: Baca GDD §14, PackagesDocumentation §5, dan Pretelan Sistem Mekanik §2 (SYS-20).
- [x] Planning: Membuat `owner.md`, `task-plan.md`, `task-status.md`, `task-review.md`, dan `compile-checking.md`.
- [x] Approval: Approved by User, 2026-11-16.
- [x] Implementation: Menulis script `SaveLoadSystem.cs` di `Assets/Resources/Scripts/System/`.
- [x] Compilation: `dotnet build Assembly-CSharp.csproj --no-restore` (0 errors).

## In progress

- [ ] Smoke test penyimpanan dan pemuatan data save di Unity Editor (lihat compile-checking.md).

## Blockers

None.

## Changed files

- `Assets/Resources/Scripts/System/SaveLoadSystem.cs`
- `Assembly-CSharp.csproj`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/owner.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/task-status.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/task-review.md`

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Decisions and deviations

- Menggunakan dual backend: File JSON fisik di `Application.persistentDataPath/savegame.json` dan backup ke `PlayerPrefs` (key `GIMERSIA_SAVE_DATA`) agar dapat langsung diinspeksi oleh tool `PlayerPrefs Editor`.

## Blockers

None.

## Changed files

- `Assets/Docs/Tasks/task0021-save-load-system-abdi/owner.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/task-status.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/task-review.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/compile-checking.md`

## Verification evidence

- Pending.

## Decisions and deviations

- Menggunakan penyimpanan JSON Native (`Application.persistentDataPath/savegame.json`) dengan backup sinkronisasi ke `PlayerPrefs` agar dapat langsung diinspeksi oleh `PlayerPrefs Editor`.
