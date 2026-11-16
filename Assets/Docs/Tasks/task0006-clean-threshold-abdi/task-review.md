# Review — TASK-0006

## Result

PASS WITH RISKS

## What changed

- `GrimeSpot`: `_autoCompleteThreshold` (0.95) + auto-tuntas di `ApplyCleaning` — per individu spot.
- `CleaningSystemManager`: logika global dihapus (tidak ada `OnRoomCleaned`/`AutoCompleted` lagi).

## Acceptance criteria

- [x] Compile bersih.
- [ ] Smoke test di Editor (menunggu developer).

## Regression check

- Related feature: Sanitation System (task0005).
- Result: event lama tidak berubah; threshold hanya mempercepat penyelesaian spot.

## Verification evidence

- Compile: 0 errors, 2026-11-16.
- Tests: belum dijalankan.
- Manual flow: belum dijalankan.

## Known issues

- Belum diverifikasi di Unity Editor.

## Follow-up tasks

- Reset level tetap via `CleaningSystemManager.ResetAllSpots()`.
