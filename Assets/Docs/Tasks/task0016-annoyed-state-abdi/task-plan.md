# TASK-0016 — Annoyed State Debuff (SYS-15)

## Goal

Mengimplementasikan efek gameplay saat karakter berada di kondisi suasana hati buruk (*Annoyed / Bete State*):
Ketika nilai Mood pemain **< 25%**:
- Kecepatan bergerak (*Base Move Speed*) pemain otomatis berkurang sebesar **15%** (pengali `x0.85`).
- Jika mood kembali pulih di atas ambang batas (≥ 25%), kecepatan berjalan kembali normal (100%).

## User behavior

- Saat mood pemain drop di bawah 25% (karena kelelahan atau tabrakan berulang), karakter terasa sedikit melambat dan letih.
- Begitu pemain dipeluk atau melakukan toss dan mood naik kembali ≥ 25%, kelincahan gerak normal kembali pulih seketika.

## Evidence

- GDD §15.3 (Efek Gameplay Mood) & Pretelan Sistem Mekanik §2.6 (SYS-15 Annoyed State [P2]): "Ketika mood di bawah 25%, kecepatan berjalan berkurang 15%."
- Dependensi: `MoodChemistrySystem.cs` (TASK-0015) dan `PlayerMovementController.cs`.

## Scope

### In scope

- Expose property / modifier multiplier di `MoodChemistrySystem.cs`:
  `public float MoveSpeedMultiplier => CurrentMood < 25f ? 0.85f : 1.0f;`
- Mengalikan multiplier mood ini pada kalkulasi kecepatan di `PlayerMovementController.cs`.
- Memicu event / update realtime tanpa merusak stat dasar pemain.

### Out of scope

- Pengali efisiensi kerja lainnya (radius pel/lempar) disimpan untuk polish Tier B berikutnya.

## Technical approach

Di `PlayerMovementController.cs`, baca multiplier dari komponen `MoodChemistrySystem` pemain jika terpasang:
`float moodMultiplier = _moodSystem != null ? _moodSystem.MoveSpeedMultiplier : 1f;`
`float targetSpeed = _baseSpeed * moodMultiplier;`

## Files expected to change

- `Assets/Resources/Scripts/System/MoodChemistrySystem.cs`
- `Assets/Script/CCC/PlayerMovementController.cs`
- `Assets/Docs/Tasks/task0016-annoyed-state-abdi/task-status.md`
- `Assets/Docs/Tasks/task0016-annoyed-state-abdi/task-review.md`
- `Assets/Docs/Tasks/task0016-annoyed-state-abdi/compile-checking.md`

## Interfaces and state

- `MoveSpeedMultiplier`: float (0.85f saat < 25%, 1.0f saat normal).

## Invariants

- Tidak pernah mengurangi speed menjadi 0.
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Mengubah dinamika kecepatan pergerakan karakter saat mood rendah (SYS-15).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Saat mood < 25%, kecepatan berjalan berkurang tepat 15% (pengali 0.85x).
- [ ] Saat mood ≥ 25%, kecepatan berjalan kembali 100% normal.
