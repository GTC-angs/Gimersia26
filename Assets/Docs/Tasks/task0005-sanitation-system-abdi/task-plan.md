# TASK-0001 — Sanitation System (URP Decal Grime + Cleaning Tools)

## Goal

Pemain bisa membersihkan noda berbasis URP Decal dengan 3 level grime (Debu, Tumpahan, Genangan) memakai tool yang sesuai, dengan radius dan durasi per petak yang berbeda untuk P1 dan P2, dan level mengeluarkan rasio kebersihan lewat event.

Cakupan ID: SYS-01, SYS-02, SYS-03, SYS-04.

## User behavior

- Menahan tombol Clean di dekat noda membuat decal memudar bertahap sampai hilang.
- Debu hanya bersih dengan Sapu/Kemoceng; Tumpahan dan Genangan hanya dengan Pel; tool yang salah tidak berefek.
- Genangan butuh 2 tahap (pudar ke 50%, lalu hilang).
- P1: radius 0,9 m, 3,0 detik per petak. P2: radius 1,5 m, 0,8 detik per petak.
- Melepas tombol tidak mengembalikan noda; progres bertahan.

## Evidence

Dokumen yang sudah dibaca:
- [x] `Agentrule.md`
- [x] `GDD.md` (§5.2 poin 4 Cleaning, §6, §13)
- [x] `Pretelan_Seluruh_System_Mekanik.md` (§2.1, acuan utama)
- [x] Template `task-plan.md`, `task-status.md`, `task-review.md`
- [ ] `Art Direction.md`, `Animation Documentation.md`, `PackagesDocumentation.md` — belum diterima

Rincian:
- Code/asset: belum ada script sanitasi di project (6 file baru, lihat Files).
- Technical design: Pretelan §2.1. Tabel menulis semua SYS-01..04 di `CleaningSystemManager.cs`; dipecah per SRP (Agentrule).
- GDD/PLGDD: GDD §5.2 (Cleaning: noda sampai 0%), Pretelan menggantikan angka bila berbeda.
- Current gap: belum ada sistem noda, tool rule, maupun event rasio kebersihan untuk `CleanlinessMeterUI`.

## Scope

### In scope

- Enum `GrimeLevel` dan `CleaningToolType`.
- `GrimeSpot`: 1 petak = 1 `DecalProjector`; progres, fade decal, 2 tahap untuk Genangan.
- `CleaningSystemManager`: registry spot, query radius, rasio kebersihan, event.
- `PlayerCleaner`: input Clean (New Input System), cek tool, radius dan durasi per pemain (Inspector).
- `CleaningSimulationTester` dan panduan pasang/test (diminta pemberi task).

### Out of scope

- Mengambil/mengganti tool (menunggu Grab/Carry; sementara via Inspector atau `SetTool()`).
- Ambang 95% auto-clean (P1, task terpisah).
- Pengali mood (P2), VFX/SFX, animasi ngepel, UI meter.
- Membuat material decal, prefab, scene, dan Input Actions (manual developer).
- Aset decal Generative AI (dilarang).

## Technical approach

1 petak = 1 `GrimeSpot` dengan `DecalProjector`. Saat Clean ditahan, `PlayerCleaner` meminta ke manager semua spot belum bersih dalam radius (jarak bidang XZ dari titik ayunan di depan pemain), lalu menambah `work = deltaTime / secondsPerCell` jika tool cocok. Debu/Tumpahan selesai di work 1.0, Genangan di 2.0 (tahap 1 berakhir di 50%). `DecalProjector.fadeFactor = 1 - progress`. Manager menjumlah progres semua spot dan mengirim `OnCleanlinessRatioChanged(float)`.

| Grime | Tool valid |
|---|---|
| Debu | Broom, Duster |
| Tumpahan | Mop |
| Genangan | Mop (2 tahap) |

## Files expected to change

Semua file baru; tidak ada file existing yang diubah:
- `Assets/Resources/Scripts/Interface&Schema/GrimeLevel.cs`
- `Assets/Resources/Scripts/Interface&Schema/CleaningToolType.cs`
- `Assets/Resources/Scripts/System/GrimeSpot.cs`
- `Assets/Resources/Scripts/System/CleaningSystemManager.cs`
- `Assets/Resources/Scripts/System/PlayerCleaner.cs`
- `Assets/Resources/Scripts/System/CleaningSimulationTester.cs`
- `Assets/Docs/Tasks/task0001-sanitation-system-abdi/setup-and-test-guide.md`

## Interfaces and state

- Public API/type changes:
  - `CleaningSystemManager.Instance`, `CleanlinessRatio`, `TotalSpots`, `CleanedSpots`, `RegisterSpot/UnregisterSpot`, `GetSpotsInRadius`, `ResetAllSpots`
  - `GrimeSpot.ApplyCleaning`, `ResetGrime`, `Initialize`, `Progress01`, `IsClean`, `CurrentStage`, static `CanToolClean`
  - `PlayerCleaner.OnClean(CallbackContext)`, `SetCleaningHeld`, `SetTool`, `Configure`, `CleanStep`
- Runtime state changes: progres per `GrimeSpot` (`_work`), jumlah progres di manager. Tidak disimpan antar-level.
- Save/load impact: tidak ada.
- Scene/Inspector impact (manual): Decal Renderer Feature di URP, material decal, prefab GrimeSpot ×3, 1 `CleaningSystemManager` per scene, `PlayerCleaner` di P1 dan P2, action `Clean` di Input Actions.
- Events and dependencies:
  - Event keluar: `OnCleanlinessRatioChanged(float)`, `OnSpotCleaned(GrimeSpot)`, `GrimeSpot.OnProgressChanged/OnCleaned`.
  - Dependensi: URP `DecalProjector`, `UnityEngine.InputSystem`.

## Invariants

- Tidak mengubah `.unity`, `.prefab`, `.meta`.
- Hanya New Input System; tidak ada legacy Input.
- Semua angka (radius, durasi, offset) lewat `[SerializeField]`, tidak di-hardcode.
- Tool salah tidak menambah progres dan tanpa penalti; tidak ada softlock.
- `CleanStep` tanpa alokasi GC per frame.
- Tidak menyentuh pengali mood.

## Risks

- Script belum dikompilasi di Unity 6000.1.7f1 (cek wajib).
- Project mungkin memakai `.asmdef` yang tidak mereferensi URP Runtime, sehingga `DecalProjector` tidak terbaca.
- Wiring input bergantung pada setup `PlayerInput` milik controller pemain (Raffa).
- Decal tidak muncul jika Decal Renderer Feature belum aktif atau Size Z tidak menjangkau lantai.
- Interpretasi "pel dua tahap" (waktu total 2×, tanpa syarat lepas tombol di antara tahap) perlu dikonfirmasi desainer.
- Event rasio dikirim tiap perubahan ≥ threshold; UI sebaiknya tidak membuat alokasi per event.

## Approval gate

- Required: Yes
- Reason: Implementasi sistem inti P0 dengan keputusan teknis baru (Decal, pemecahan script dari tabel Pretelan).
- Approval: **Approved by Abdi, 2026-11-14** (konfirmasi via chat "gas buat bang"; nama diasumsikan sama dengan PIC di `owner.md`).

## Acceptance criteria

- [ ] Compile bersih tanpa error/warning dari script baru.
- [ ] Debu bersih dengan Broom atau Duster; Mop tidak berpengaruh.
- [ ] Tumpahan hanya bersih dengan Mop.
- [ ] Genangan butuh 2 tahap dengan Mop (50% lalu bersih).
- [ ] P1: radius 0,9 m dan 3,0 s per petak; P2: radius 1,5 m dan 0,8 s per petak (diukur dengan stopwatch).
- [ ] Spot di luar radius tidak ikut bersih.
- [ ] `OnCleanlinessRatioChanged` terpanggil dengan nilai 0..1 yang benar.
- [ ] `CleaningSimulationTester` hijau semua.
- [ ] `CleanStep` GC Alloc = 0 B di Profiler.
- [ ] Hanya New Input System; semua angka bisa diatur dari Inspector.
