# TASK-0013 — Door Interaction System (SYS-12)

## Goal

Mengimplementasikan sistem interaksi pintu (`DoorInteractionSystem.cs`) yang mendukung dua tipe pintu:
1. **Pintu Ayun (Swing Door):** Berputar membuka/menutup pada pivot engsel tertentu (0° ke 90°).
2. **Pintu Geser (Sliding Door):** Bergeser translasi membuka/menutup sepanjang sumbu lokal tertentu (misal translasi X sejauh 1.0 m).

## User behavior

- Ketika pemain mendekati pintu atau berinteraksi dengannya (atau trigger sensor jarak / tombol interact):
  - Pintu ayun berayun terbuka secara halus (menggunakan animasi tween atau rotasi terkontrol).
  - Pintu geser bergeser terbuka ke samping memberi jalan bagi pemain yang membawa perabot antar ruangan di Level 2.
- Pintu memiliki opsi auto-close dengan delay yang dapat diatur, atau interaksi manual buka/tutup.

## Evidence

- Technical design:
  - `Assets/Docs/GDD.md` §6.3 (Level 2: Relokasi Antar Ruang - "pintu geser/ayun yang mengharuskan kerja sama menahan pintu").
  - `Assets/Docs/Art Direction.md` §11 (Modular Kit - "Pintu: frame + daun pintu 1,0 m; pintu geser untuk Level 2").
  - `Assets/Docs/Pretelan Seluruh System Mekanik.md` §2.5 (SYS-12 Door Interaction).
- Agent rules: `Assets/Docs/Agentrule.md` (1 Feature = 1 Script di `Assets/Resources/Scripts/System/`).

## Scope

### In scope

- Membuat script `Assets/Resources/Scripts/System/DoorInteractionSystem.cs`.
- Mendukung mode enum `DoorType { Swing, Sliding }`.
- Konfigurasi open angle, open offset, animation speed, dan auto-close timer.
- Public method: `Open()`, `Close()`, `Toggle()`.
- Deteksi trigger proksimitas pemain atau panggilan interaksi langsung.

### Out of scope

- Sensor anti-pinch rebound (akan dibuat terpisah pada TASK-0014 / SYS-13).
- Modifikasi scene atau prefab secara langsung.

## Technical approach

Menggunakan interpolasi rotasi (untuk swing) dan posisi lokal (untuk sliding) menggunakan `DOTween` atau `Vector3.MoveTowards` / `Quaternion.RotateTowards` yang mulus dan bebas stutter.

## Files expected to change

- `Assets/Resources/Scripts/System/DoorInteractionSystem.cs` (file baru)
- `Assets/Docs/Tasks/task0013-door-interaction-abdi/task-status.md`
- `Assets/Docs/Tasks/task0013-door-interaction-abdi/task-review.md`
- `Assets/Docs/Tasks/task0013-door-interaction-abdi/compile-checking.md`

## Interfaces and state

- Public API: `DoorInteractionSystem.Open()`, `Close()`, `Toggle()`, property `IsOpen`, event `OnDoorStateChanged(bool isOpen)`.
- Runtime state: State terbuka/tertutup, progress interpolasi.

## Invariants

- Tidak mengubah `.unity` atau `.prefab` langsung dari agent.
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Fitur mekanik sistem pintu baru (SYS-12).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Pintu tipe Swing dapat membuka dan menutup dengan rotasi engsel yang mulus.
- [ ] Pintu tipe Sliding dapat bergeser membuka dan menutup secara presisi.
- [ ] Terintegrasi dengan trigger pemain atau panggilan API `Open()`/`Close()`.
