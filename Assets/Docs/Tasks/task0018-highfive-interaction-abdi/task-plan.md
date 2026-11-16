# TASK-0018 — High-Five Interaction & Sparkle VFX (SYS-17)

## Goal

Mengimplementasikan interaksi tos tangan (*High-Five Interaction*):
- Pemain P1 dan P2 berada dalam jangkauan berhadapan (≤ 2.0 meter).
- Memicu aksi tos tangan (High-Five):
  1. Aksi cepat berlangsung singkat (durasi 0.7 detik).
  2. Nilai mood kedua pemain dipulihkan sebesar **+25%**.
  3. Memunculkan efek partikel kilauan bintang (*sparkle* `VFX_HighFive_Spark`) di titik kontak tangan.
  4. Cooldown tos tangan 10–12 detik.

## User behavior

- Setelah menyelesaikan suatu tugas ruangan atau saat berpapasan:
  - Pemain menekan tos ke rekannya.
  - Tangan keduanya melakukan toss cepat (0.7 detik) disertai suara renyah dan letupan kilau bintang.
  - Mood kedua karakter bertambah +25%.

## Evidence

- GDD §15.5 (*Emote Pasangan - Tos: 2.0m, durasi 0.7 detik, +12 hingga +25 mood*).
- Pretelan Sistem Mekanik §2.6 (SYS-17 High-Five Interaction [P2]).
- VFX List §4.2 (`VFX_HighFive_Spark` - Percikan Bintang Tos Tangan).

## Scope

### In scope

- Fungsi `TryExecuteHighFive(PlayerMovementController playerA, PlayerMovementController playerB)` pada `MoodChemistrySystem`.
- Validasi jarak (≤ 2.0 m) dan status berhadapan.
- Kunci pergerakan sekejap (0.7 detik).
- Penambahan nilai mood (+25%) pada kedua pemain.
- Instansiasi efek partikel sparkle (`_highFiveVfxPrefab`) di antara kedua tangan karakter.

### Out of scope

- Pengaturan radial emote menu UI.
- Modifikasi model 3D.

## Technical approach

Menggunakan coroutine pada `MoodChemistrySystem`:
1. Validasi jarak (≤ 2.0m).
2. Kunci singkat input movement (0.7s).
3. Spawn `_highFiveVfxPrefab` di antara kedua pemain.
4. Tambah mood masing-masing +25f.
5. Selesai setelah 0.7 detik.

## Files expected to change

- `Assets/Resources/Scripts/System/MoodChemistrySystem.cs`
- `Assets/Docs/Tasks/task0018-highfive-interaction-abdi/task-status.md`
- `Assets/Docs/Tasks/task0018-highfive-interaction-abdi/task-review.md`
- `Assets/Docs/Tasks/task0018-highfive-interaction-abdi/compile-checking.md`

## Interfaces and state

- Method: `TryHighFive()`, event `OnHighFiveExecuted`.

## Invariants

- Validasi jarak ≤ 2.0 meter.
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Fitur interaksi perayaan berpasangan (SYS-17).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Tos tangan hanya aktif jika jarak ≤ 2.0 m.
- [ ] Durasi aksi berlangsung cepat (0.7 detik).
- [ ] Mood kedua pemain bertambah +25%.
- [ ] Partikel sparkle muncul di titik kontak tangan.
