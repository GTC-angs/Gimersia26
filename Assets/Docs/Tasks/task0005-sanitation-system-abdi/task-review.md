# Review — TASK-0001

## Result

Belum direview — menunggu verifikasi di Unity (PASS / PASS WITH RISKS / BLOCKED)

## What changed

- Sistem sanitasi baru: `GrimeLevel`, `CleaningToolType`, `GrimeSpot`, `CleaningSystemManager`, `PlayerCleaner`.
- `CleaningSimulationTester` untuk simulasi test otomatis.
- Panduan pasang dan test: `setup-and-test-guide.md`.

## Acceptance criteria

- [ ] Compile bersih tanpa error/warning dari script baru
- [ ] Debu bersih dengan Broom/Duster; Mop tidak berpengaruh
- [ ] Tumpahan hanya bersih dengan Mop
- [ ] Genangan 2 tahap dengan Mop (50% lalu bersih)
- [ ] P1 0,9 m / 3,0 s; P2 1,5 m / 0,8 s
- [ ] Spot di luar radius tidak ikut bersih
- [ ] `OnCleanlinessRatioChanged` benar (0..1)
- [ ] `CleaningSimulationTester` hijau semua
- [ ] `CleanStep` GC Alloc = 0 B
- [ ] Hanya New Input System; semua angka dari Inspector

## Regression check

- Related feature: belum ada fitur lain di project yang bergantung pada sistem ini. Nanti: `CleanlinessMeterUI`, Grab/Carry (tool), 95% threshold, Mood.
- Result: belum dicek.

## Verification evidence

- Compile: _belum_
- Tests: _belum_ (`CleaningSimulationTester`: PASS __ / FAIL __)
- Manual flow: _belum_ (checklist B2 di `setup-and-test-guide.md`)

## Known issues

- Belum ada yang terverifikasi. Risiko terbuka: kompatibilitas `.asmdef`, wiring `PlayerInput`, interpretasi "pel dua tahap".

## Follow-up tasks

- Ambang 95% auto-clean (P1).
- Integrasi tool dengan sistem Grab/Carry.
- `CleanlinessMeterUI` berlangganan `OnCleanlinessRatioChanged`.
- Pengali mood untuk kecepatan dan radius bersih (P2).
- VFX/SFX dan animasi ngepel.
