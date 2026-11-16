# TASK-0014 — Anti-Pinch Rebound System (SYS-13)

## Goal

Mengimplementasikan protokol anti-terjepit (*Anti-Pinch Rebound System*) pada mekanisme pintu:
Ketika pintu sedang dalam proses menutup, jika ada pemain atau perabot yang terdeteksi berada di celah kusen pintu, pintu **otomatis membal terbuka kembali minimal selebar 30 cm (atau membatalkan penutupan)** untuk mencegah pemain terhimpit atau terjebak (*No Crushing, Zero Stuck*).

## User behavior

- Saat pemain berjalan melewati pintu (terutama sambil menggotong perabot besar bersama partner):
  - Jika pintu mulai menutup otomatis saat mereka masih di tengah kusen, pintu tidak menjepit mereka ke dinding.
  - Pintu menyentuh/mendeteksi rintangan ➔ membal terbuka kembali selebar 30 cm (*rebound*) ➔ memberi ruang dan waktu bagi pemain untuk lewat.

## Evidence

- Technical design:
  - `Assets/Docs/Pretelan Seluruh System Mekanik.md` §2.5 (*Door & Anti-Pinch Rebound System [P1 - ANTI-BUG & ANTI-STUCK]*): "Pintu ayun dan pintu geser Level 2 memiliki sensor overlap fisik. Jika ada pemain atau perabot di celah kusen saat pintu menutup, pintu otomatis membal terbuka kembali selebar 30 cm dan tidak akan menjepit/menghancurkan karakter."
  - `Assets/Docs/Pretelan Seluruh System Mekanik.md` §5 Master Checklist: "Pintu geser menutup saat pemain berada di tengah ➔ Anti-Pinch Rebound: Pintu membal terbuka, pemain tidak terjepit (NO CRUSHING)."
- Dependensi: Terintegrasi langsung dengan `DoorInteractionSystem.cs` (TASK-0013 / SYS-12).

## Scope

### In scope

- Menambahkan sensor overlap (Trigger Collider atau `Physics.OverlapBoxNonAlloc`) di area celah kusen pada `DoorInteractionSystem`.
- Logika state pintu: jika sedang bergerak menutup (`IsClosing`) dan sensor mendeteksi collider pemain atau perabot:
  1. Hentikan gerakan menutup seketika.
  2. Buka kembali pintu (atau rebound mundur sebesar 30 cm / sudut proporsional).
  3. Reset delay timer penutupan.
- Event / feedback saat rebound terjadi.

### Out of scope

- Mengubah model 3D aset pintu di scene/prefab secara langsung.
- Modifikasi damage (game ini cozy, tidak ada mekanisme nyawa/damage).

## Technical approach

Di `DoorInteractionSystem`:
Tambahkan `Transform _pinchSensorPoint` dan `Vector3 _pinchSensorHalfExtents` atau child Trigger Collider.
Saat penutupan berlangsung di `Update()`: lakukan cek overlap `Physics.OverlapBoxNonAlloc`. Jika terdeteksi collider bukan dinding (Pemain / CarriedItem / Furniture), jalankan `TriggerRebound()`.

## Files expected to change

- `Assets/Resources/Scripts/System/DoorInteractionSystem.cs`
- `Assets/Docs/Tasks/task0014-anti-pinch-rebound-abdi/task-status.md`
- `Assets/Docs/Tasks/task0014-anti-pinch-rebound-abdi/task-review.md`
- `Assets/Docs/Tasks/task0014-anti-pinch-rebound-abdi/compile-checking.md`

## Interfaces and state

- Public API: `OnDoorRebound` event, `IsRebounding` state.
- Inspector properties: `_antiPinchEnabled`, `_reboundDistance = 0.3f`, `_obstacleMask`.

## Invariants

- Tidak mengubah scene atau prefab langsung dari agent.
- Karakter tidak boleh tembus ke dalam geometri kusen.
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Fitur safety fisik pintu (SYS-13).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Pintu yang sedang menutup otomatis membal terbuka saat ada pemain di celah kusen.
- [ ] Pintu yang sedang menutup otomatis membal terbuka saat ada furnitur di celah kusen.
- [ ] Rebound membuka kembali minimal selebar 30 cm.
