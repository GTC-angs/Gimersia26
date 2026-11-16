# Review — TASK-0010

## Result

PASS WITH RISKS

## What changed

- Pada `Assets/Script/CCC/PlayerInteractionController.cs`:
  - Mengubah pembacaan input dari toggle `wasPressedThisFrame` menjadi hold status `isPressed`.
  - Mengintegrasikan mekanisme:
    1. Tombol ditahan (`isHeld == true`): mencari dan mengangkat item terdekat (`GrabNearest()`).
    2. Tombol dilepas saat sedang membawa item (`!isHeld` dan `_carry.IsCarrying`): memicu peletakan/drop barang (`_carry.TryDrop()`).
    3. Tombol dilepas saat sedang co-op lift (`!isHeld` dan `CurrentLift != null`): melepaskan pegangan furnitur co-op (`CurrentLift.TryInteract(_movement)`).

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Tombol Grab ditahan (`Hold`) ➔ Karakter mengambil dan terus membawa item (menunggu smoke test Editor).
- [ ] Tombol Grab dilepas (`Release`) ➔ Karakter melepaskan (drop) item (menunggu smoke test Editor).
- [ ] Berfungsi untuk Keyboard P1 (`E`), Keyboard P2 (`Numpad 1` / `Keypad Enter`), dan Gamepad (`South Button`).
- [ ] Tidak mengganggu sistem Co-Op Lift.

## Regression check

- Related feature: `PlayerCarryController`, `CoOpLiftController`.
- Result: Logika pemanggilan drop dan co-op lift tetap menggunakan API yang sama, hanya di-trigger saat tombol dilepas.

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Known issues

- Perlu verifikasi playtest / smoke test langsung di Unity Editor.

## Follow-up tasks

None.
