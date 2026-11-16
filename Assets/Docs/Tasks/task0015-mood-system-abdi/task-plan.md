# TASK-0015 — Mood System Core (SYS-14)

## Goal

Mengimplementasikan core system mood pemain (`MoodChemistrySystem.cs`) dengan rentang nilai **0–100%**:
- Baseline awal: 60% (Nyaman).
- Pengurangan mood dinamis: tabrakan antar pemain/perabot, kelelahan stamina habis, dan waktu berjalan di ruangan kotor.
- Event C# untuk integrasi UI/HUD, VFX, dan audio.

## User behavior

- Setiap karakter (P1 Suami & P2 Istri) memiliki nilai Mood masing-masing.
- Saat bermain buruk (sering menabrak, stamina terkuras habis, atau membiarkan ruangan kotor): mood berkurang perlahan.
- Perubahan mood memicu event untuk memperbarui UI HUD bar dan indikator mood.

## Evidence

- GDD §15 (*Mood & Chemistry System*): 4 State Mood (Semangat 80–100, Nyaman 50–79, Kesal 20–49, Bete 0–19).
- Pretelan Sistem Mekanik §2.6 (SYS-14 Mood System [P2]).
- Agent rules: `Assets/Docs/Agentrule.md`.

## Scope

### In scope

- Membuat script `Assets/Resources/Scripts/System/MoodChemistrySystem.cs`.
- Pengelolaan `CurrentMood` (0–100f), `MoodState` (Happy, Normal, Annoyed, Exhausted).
- Fungsi perubahan mood: `ModifyMood(float delta, string reason)`.
- Listener / hook pengurangan: tabrakan, stamina habis (`OnStaminaExhausted`), dan time decay.
- Event C#: `OnMoodChanged(PlayerID, float ratio01, MoodState)`.

### Out of scope

- Pengurangan move speed Annoyed State (dikerjakan di TASK-0016 / SYS-15).
- Logika interaksi Hug dan High-Five (TASK-0017 dan TASK-0018).

## Technical approach

Menggunakan pola arsitektur Component / Manager di mana `MoodChemistrySystem` dipasang pada karakter pemain (atau mengelola instance P1 dan P2), menyediakan API public `GetMood(player)` dan `ModifyMood(player, delta)`.

## Files expected to change

- `Assets/Resources/Scripts/System/MoodChemistrySystem.cs` (file baru)
- `Assets/Docs/Tasks/task0015-mood-system-abdi/task-status.md`
- `Assets/Docs/Tasks/task0015-mood-system-abdi/task-review.md`
- `Assets/Docs/Tasks/task0015-mood-system-abdi/compile-checking.md`

## Interfaces and state

- Public API: `CurrentMood`, `State`, `ModifyMood()`, `OnMoodChanged`.

## Invariants

- Mood dibatasi antara 0% hingga 100%.
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Sistem inti emosional baru pemain (SYS-14).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Mood memiliki nilai awal 60% dan rentang 0–100%.
- [ ] Pengurangan mood dapat dipicu oleh tabrakan, stamina habis, atau waktu.
- [ ] Event `OnMoodChanged` tertembak setiap nilai mood berubah.
