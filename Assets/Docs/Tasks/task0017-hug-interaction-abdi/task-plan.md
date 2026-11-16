# TASK-0017 — Hug Interaction & Heart VFX (SYS-16)

## Goal

Mengimplementasikan interaksi pelukan pasangan (*Hug Interaction*):
- Pemain P1 dan P2 berada dalam radius jangkauan berdekatan (≤ 2.0 meter).
- Memicu aksi peluk:
  1. Karakter menghadap satu sama lain dan terkunci gerakannya selama 1.5 detik.
  2. Nilai mood kedua pemain dipulihkan sebesar **+50%** (atau hingga penuh).
  3. Memunculkan efek partikel hati merah coral (`VFX_Heart_Hug`) di antara kedua karakter.
  4. Cooldown pelukan (misal 20–25 detik) untuk mencegah spam instan.

## User behavior

- Ketika suasana hati pemain sedang drop (bete/kesal) atau setelah menyelesaikan tugas sulit:
  - Pemain mendekati pasangannya dan memicu aksi Peluk.
  - Keduanya berpelukan lucu selama 1.5 detik disertai letupan partikel hati merah coral.
  - Bar mood seketika terisi +50% dan kelincahan gerak pulih.

## Evidence

- GDD §15.5 (*Emote Pasangan - Peluk: 2.0m, durasi 1.5 detik, +25 hingga +50 mood*).
- Pretelan Sistem Mekanik §2.6 (SYS-16 Hug Interaction [P2]).
- VFX List §4.1 (`VFX_Heart_Hug` - Partikel Hati Coral).

## Scope

### In scope

- Fungsi `TryExecuteHug(PlayerMovementController playerA, PlayerMovementController playerB)` pada `MoodChemistrySystem`.
- Validasi jarak (≤ 2.0 m) dan cooldown timer.
- Kunci pergerakan sementara (1.5s) via coroutine / lock flag.
- Penambahan nilai mood (+50%) pada kedua karakter.
- Instansiasi efek partikel hati (`_heartHugVfxPrefab`) di titik tengah antara kedua karakter.

### Out of scope

- Pengaturan radial menu UI (dikerjakan oleh tim UI).
- Modifikasi model atau animasi blendshape.

## Technical approach

Menggunakan coroutine pada `MoodChemistrySystem`:
1. `_isHugging = true;`
2. Kunci input movement karakter selama 1.5 detik.
3. Spawn `_heartHugVfxPrefab` di `(posA + posB) * 0.5f + Vector3.up * 1.0f`.
4. Tambah mood masing-masing +50f.
5. Setelah 1.5 detik: buka kunci movement dan mulai countdown cooldown.

## Files expected to change

- `Assets/Resources/Scripts/System/MoodChemistrySystem.cs`
- `Assets/Docs/Tasks/task0017-hug-interaction-abdi/task-status.md`
- `Assets/Docs/Tasks/task0017-hug-interaction-abdi/task-review.md`
- `Assets/Docs/Tasks/task0017-hug-interaction-abdi/compile-checking.md`

## Interfaces and state

- Method: `TryHug()`, property `CanHug`, event `OnHugExecuted`.

## Invariants

- Peluk hanya bisa dilakukan jika jarak ≤ 2.0 meter.
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Fitur interaksi emosional dan gameplay feel berpasangan (SYS-16).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Pelukan hanya aktif jika jarak antar pemain ≤ 2.0 m.
- [ ] Karakter terkunci selama 1.5 detik saat berpelukan.
- [ ] Mood kedua pemain bertambah +50%.
- [ ] Partikel hati muncul di antara kedua karakter.
