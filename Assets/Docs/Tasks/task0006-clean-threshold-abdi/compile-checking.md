# Compile Checking & Manual Test — TASK-0006 (SYS-05)

## Goal

Panduan lengkap wiring + test 95% Clean Threshold **per individu spot** di Unity Editor.

## Prasyarat

- Unity 6000.1.7f1, project compile bersih (`dotnet build Assembly-CSharp.csproj --no-restore` → 0 errors).
- Scene test dengan `CleaningSystemManager` + minimal 2 `GrimeSpot` (DecalProjector) — mis. `Assets/Scenes/testing_cleaning.unity`.

## Langkah tes

1. Buka scene, Play.
2. Bersihkan spot A sampai ~95% (perhatikan decal memudar) — spot A harus **langsung lenyap total** tanpa perlu membersihkan 5% terakhir.
3. Periksa spot B — harus **masih kotor**, tidak ikut hilang.
4. Bersihkan spot B sampai 95% — spot B juga langsung tuntas.
5. Genangan (Puddle): progres total 2 tahap; saat progres mencapai 95% dari 2.0 work, spot tuntas seketika.
6. Restart/test via `CleaningSystemManager.ResetAllSpots()` — semua spot kembali kotor, threshold tetap aktif.

## Output yang diharapkan

- Spot dengan progres ≥ 0.95 langsung `IsClean == true`, decal nonaktif.
- Spot lain tidak terpengaruh.
- `CleanlinessRatio` global naik normal seiring spot tuntas.
- Tidak ada error di Console.

## Jika gagal

- **Spot tidak auto-tuntas:** cek `_autoCompleteThreshold` di Inspector spot (default 0.95); pastikan nilai 0–1.
- **Semua spot ikut bersih:** bug — pastikan logika global sudah dihapus dari `CleaningSystemManager` (revisi 2026-11-16).
- **Decal tidak hilang:** `GrimeSpot._decal` tidak ter-assign; Awake auto-cari `DecalProjector` di child.
- **Compile error:** `dotnet build Assembly-CSharp.csproj --no-restore`.
