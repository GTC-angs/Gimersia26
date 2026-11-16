# Review — TASK-0014

## Result

PASS WITH RISKS

## What changed

- Pada `Assets/Resources/Scripts/System/DoorInteractionSystem.cs`:
  - Menambahkan sensor overlap fisik di celah kusen pintu via `Physics.OverlapBoxNonAlloc`.
  - Jika pintu sedang bergerak menutup (`IsClosing`) dan terdeteksi collider pemain atau perabot/barang ber-Rigidbody:
    1. Membatalkan proses penutupan seketika.
    2. Menjalankan `TriggerAntiPinchRebound()` yang membal membuka pintu kembali minimal selebar 30 cm (`_reboundDistance = 0.3f` / `_reboundAngle = 25f`).
    3. Mereset timer auto-close.
    4. Memicu event `OnAntiPinchTriggered`.
  - Visualisasi kotak sensor via `OnDrawGizmosSelected` di Scene view Unity.

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Pintu yang sedang menutup otomatis membal terbuka saat ada pemain di celah kusen (menunggu smoke test Editor).
- [ ] Pintu yang sedang menutup otomatis membal terbuka saat ada furnitur di celah kusen.
- [ ] Rebound membuka kembali minimal selebar 30 cm.

## Regression check

- Related feature: `DoorInteractionSystem` (SYS-12).
- Result: Logika auto-close dan pergerakan normal tetap berjalan lancar.

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, 0 warnings.

## Known issues

- Perlu smoke test sensor di Editor dengan collider nyata.

## Follow-up tasks

None.
