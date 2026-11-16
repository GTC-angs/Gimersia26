# Review — TASK-0015

## Result

PASS WITH RISKS

## What changed

- Membuat script `Assets/Resources/Scripts/System/MoodChemistrySystem.cs`:
  - Mengelola `_currentMood` (0–100%) dengan baseline awal 60%.
  - 4 State Mood: Happy (≥75%), Normal (40–74%), Annoyed (20–39%), Exhausted (<20%).
  - Pemicu decay alami per interval waktu, penalti tabrakan collision keras (`ModifyMood(-_collisionMoodPenalty)`), dan penalti saat stamina terkuras habis.
  - Event `OnMoodChanged` untuk mengabari HUD / UI.

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Mood memiliki nilai awal 60% dan rentang 0–100% (menunggu smoke test Editor).
- [ ] Pengurangan mood dapat dipicu oleh tabrakan, stamina habis, atau waktu.
- [ ] Event `OnMoodChanged` tertembak setiap nilai mood berubah.

## Regression check

- Related feature: `PlayerMovementController`.
- Result: 0 build errors.

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Known issues

- Perlu smoke test di Editor.

## Follow-up tasks

None.
