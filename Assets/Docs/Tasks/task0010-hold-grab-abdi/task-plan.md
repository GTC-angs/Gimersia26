# TASK-0010 — Hold to Grab Item System (New Input System)

## Goal

Memperbaiki dan mengubah skema kontrol grab/interact item pada `PlayerInteractionController` agar menggunakan New Input System berbasis **Hold** (tahan tombol untuk memegang/membawa barang, lepas tombol untuk melepaskan/drop barang), mendukung Gamepad dan dual-keyboard (P1 & P2).

## User behavior

- Saat pemain menahan tombol Grab (`E` / `South Button` untuk P1, `Numpad 1` / `Keypad Enter` / `South Button` untuk P2):
  - Jika belum membawa barang: mendeteksi dan mengambil (*grab*) item terdekat yang valid.
  - Selama tombol ditahan: pemain tetap membawa (*carry*) barang tersebut.
- Saat tombol Grab dilepaskan (*button released / isPressed == false*):
  - Jika sedang membawa barang: otomatis melepaskan (*drop*) barang tersebut secara aman.
  - Jika sedang melakukan co-op lift: melepaskan pegangan dari furnitur co-op.

## Evidence

- Code/asset: `Assets/Script/CCC/PlayerInteractionController.cs` saat ini menggunakan `wasPressedThisFrame` (toggle klik untuk pick up / drop).
- Technical design: `Assets/Docs/Pretelan Seluruh System Mekanik.md` §1.3 (Smart Grab, Socket Carry & Weight System) dan §26 (Tabel Kontrol Resmi).
- Agent rules: `Assets/Docs/Agentrule.md` (Gunakan Unity New Input System, no direct scene/prefab edit, zero compile errors).
- Current gap: kontrol saat ini adalah toggle press, belum mendukung mekanisme *Hold to Grab & Release to Drop* yang lebih intuitif ala Moving Out.

## Scope

### In scope

- Modifikasi deteksi input pada `PlayerInteractionController.cs` untuk membaca status penekanan (*isPressed* / *wasReleasedThisFrame*).
- Logika state grab saat tombol mulai ditekan / ditahan dan drop saat tombol dilepas.
- Penanganan co-op lift saat tombol hold dilepas.
- Kompatibilitas dengan Keyboard P1 (E), Keyboard P2 (Numpad 1 / Numpad Enter), dan Gamepad per index player.

### Out of scope

- Perubahan visual aset, prefab, atau scene `.unity`.
- Modifikasi internal `PlayerCarryController` fisika socket (hanya integrasi pemanggilan `TryPickUp` dan `TryDrop`).
- Perubahan pada sistem throw atau stamina.

## Technical approach

1. Di `PlayerInteractionController`:
   - Ganti helper deteksi input dari `wasPressedThisFrame` menjadi `IsGrabHeld()` (mengembalikan true jika tombol sedang ditahan) dan `WasGrabReleased()` (mengembalikan true saat tombol baru saja dilepas).
   - Pada `Update()`:
     - Jika sedang membawa barang (`_carry.IsCarrying` atau `CurrentLift != null`) dan tombol grab dilepas (`WasGrabReleased()` atau `!IsGrabHeld()`): panggil `_carry.TryDrop()` atau `CurrentLift.TryInteract(_movement)`.
     - Jika belum membawa barang dan tombol grab sedang ditekan/ditahan (`IsGrabHeld()`): jalankan `GrabNearest()`.
2. Pastikan tidak terjadi re-trigger terus menerus jika tidak ada objek di sekitar saat tombol ditahan.

## Files expected to change

- `Assets/Script/CCC/PlayerInteractionController.cs`
- `Assets/Docs/Tasks/task0010-hold-grab-abdi/task-status.md`
- `Assets/Docs/Tasks/task0010-hold-grab-abdi/task-review.md`
- `Assets/Docs/Tasks/task0010-hold-grab-abdi/compile-checking.md`

## Interfaces and state

- Public API/type changes: Tidak merusak public API yang sudah ada (`CurrentLift`).
- Runtime state changes: Deteksi hold input per-frame.
- Save/load impact: None.
- Scene/Inspector impact: None.

## Invariants

- Tetap menggunakan Unity New Input System (`UnityEngine.InputSystem`).
- Tidak mengedit file `.unity`, `.prefab`, atau `.meta`.
- Zero compile error.

## Risks

- Jika pemain menahan tombol sambil berjalan di dekat objek lain, jangan sampai berganti grab objek lain tanpa sengaja (hanya grab jika `!_carry.IsCarrying`).
- Co-op lift perlu transisi lepas yang mulus saat salah satu atau kedua pemain melepas tombol.

## Approval gate

- Required: Yes
- Reason: Perubahan skema kontrol interaksi inti (CCC) pemain.
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Tombol Grab ditahan (`Hold`) ➔ Karakter mengambil dan terus membawa item.
- [ ] Tombol Grab dilepas (`Release`) ➔ Karakter melepaskan (drop) item.
- [ ] Berfungsi untuk Keyboard P1 (`E`), Keyboard P2 (`Numpad 1` / `Keypad Enter`), dan Gamepad (`South Button`).
- [ ] Tidak mengganggu sistem Co-Op Lift.
