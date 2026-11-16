# TASK-0006 — 95% Clean Threshold per Spot (SYS-05)

## Goal

Ketika progres sebuah noda (spot) mencapai ≥ 95%, spot itu otomatis dianggap tuntas 100% — per individu spot, bukan keseluruhan ruangan.

## User behavior

- Pemain membersihkan satu noda; begitu noda itu mencapai 95%, sisa noda tersebut langsung hilang tanpa perlu pixel hunting.
- Spot lain di ruangan tidak terpengaruh.
- Ruangan selesai bila semua spot `IsClean` (progress 100%).

## Evidence

- Code/asset: `Assets/Resources/Scripts/System/GrimeSpot.cs`
- Technical design: `Assets/Docs/Pretelan Seluruh System Mekanik.md` §2.1 (95% Clean Threshold, Anti-Pixel Hunting)
- GDD/PLGDD: `Assets/Docs/GDD.md` §5.2 poin 4 (Cleaning)
- Current gap: tidak ada threshold; spot 5% terakhir mengharuskan pixel hunting.

## Scope

### In scope

- Field `_autoCompleteThreshold` (default 0.95) di `GrimeSpot` (per individu spot).
- Di `ApplyCleaning`: jika progres ≥ threshold, spot langsung tuntas (`_work = StageCount`).

### Out of scope

- Threshold level/global (ditolak: spec berlaku per individu spot).
- UI cleanliness meter, VFX, perubahan scene/prefab.

## Technical approach

Cek threshold inline di `ApplyCleaning` setelah penambahan work: `if (!IsClean && Progress01 >= _autoCompleteThreshold) _work = StageCount;`. Event `OnProgressChanged`/`OnCleaned` tetap dilempar normal dengan delta yang benar. `CleaningSystemManager` tidak berubah (rasio global sudah mencerminkan spot yang tuntas).

## Files expected to change

- `Assets/Resources/Scripts/System/GrimeSpot.cs`

## Interfaces and state

- Public API/type changes: field `_autoCompleteThreshold` di `GrimeSpot`.
- Runtime state changes: tidak ada flag baru global; threshold dicek inline per spot.
- Save/load impact: none.
- Scene/Inspector impact: threshold bisa diatur per-spot di Inspector (default 0.95).
- Events and dependencies: tidak ada event baru.

## Invariants

- Tidak mengubah `.unity`, `.prefab`, `.meta`.
- Threshold hanya menyelesaikan spot itu sendiri; spot lain tidak tersentuh.
- Semua angka via `[SerializeField]`.

## Risks

- Interpretasi "95% area": dipertegas user (2026-11-16) berlaku **per individu spot**, bukan rasio global ruangan.

## Approval gate

- Required: Yes
- Reason: mengubah perilaku sistem inti.
- Approval: Approved by User, 2026-11-16 (chat: "kerjain ini bang" + klarifikasi per-individu).

## Acceptance criteria

- [x] Compile bersih (`dotnet build Assembly-CSharp.csproj --no-restore`, 0 errors).
- [ ] Spot mencapai 95% → otomatis 100%, `OnCleaned` terpanggil.
- [ ] Spot lain tidak ikut bersih.
- [ ] Spot Genangan (2 tahap) juga tuntas dari 95% progres total.
