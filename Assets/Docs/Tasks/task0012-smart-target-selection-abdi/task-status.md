# Status — TASK-0012

## Current phase

Verification (Implementasi selesai, kompilasi 0 error; siap untuk smoke test di Unity Editor)

## Completed

- [x] Exploration: Analisis `GrabNearest()` di `PlayerInteractionController.cs` dan spesifikasi SYS-11 di Pretelan Sistem Mekanik §2.4.
- [x] Planning: Membuat `owner.md`, `task-plan.md`, `task-status.md`, `task-review.md`, dan `compile-checking.md`.
- [x] Approval: Approved by User, 2026-11-16.
- [x] Implementation: Menambahkan sistem pembobotan prioritas (Tier 3 = Alat pembersih, Tier 2 = Barang ringan/sedang, Tier 1 = Furnitur berat) di `PlayerInteractionController.cs`.
- [x] Compilation: `dotnet build Assembly-CSharp.csproj --no-restore` (0 errors).

## In progress

- [ ] Smoke test seleksi target bertumpuk di Unity Editor (lihat compile-checking.md).

## Blockers

None.

## Changed files

- `Assets/Script/CCC/PlayerInteractionController.cs`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/owner.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/task-status.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/task-review.md`

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Decisions and deviations

- Menggunakan metode `GetItemPriority()`: Tier 3 (Alat pembersih / tool tag), Tier 2 (Barang ringan/medium), Tier 1 (Furnitur berat / heavy tag / CoOpLift). Target dengan tier lebih tinggi selalu terpilih lebih dahulu.

## Blockers

None.

## Changed files

- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/owner.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/task-status.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/task-review.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/compile-checking.md`

## Verification evidence

- Pending implementation.

## Decisions and deviations

- Menggunakan ranking tier prioritas (Tier 3 = Alat pembersih, Tier 2 = Barang ringan/sedang, Tier 1 = Perabot berat) dengan penentu jarak sekunder.
