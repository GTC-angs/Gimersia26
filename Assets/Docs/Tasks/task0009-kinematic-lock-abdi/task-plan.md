# TASK-0009 — Static Kinematic Lock (SYS-08)

## Goal

Furnitur yang sudah masuk drop-zone (tersnap) dikunci kinematis: tidak bisa diambil pemain, tidak bisa tertendang/tergeser, tidak bisa berantakan kembali.

## User behavior

- Setelah snap (task0008), furnitur tidak bisa di-grab lagi.
- Furnitur mengabaikan tumbukan fisika (kinematic) — tidak terdorong pemain/barang lain.
- `DropZoneManager.UnlockItem(item)` membuka kunci untuk reset level/testing.

## Evidence

- Code/asset: `Assets/Resources/Scripts/System/DropZoneManager.cs` (`LockItem`, `UnlockItem`, `DropZoneLock`)
- Technical design: `Assets/Docs/Pretelan Seluruh System Mekanik.md` §2.2 (Static Kinematic Lock), §2.2 checklist "NO STUCK"
- GDD/PLGDD: GDD §5.2 poin 5 (drop-zone terkunci setelah rapi).
- Current gap: furnitur hasil snap masih bisa di-grab/tertendang.

## Scope

### In scope

- `DropZoneManager.LockItem(Rigidbody)`: `isKinematic = true`, nol velocity, pasang `DropZoneLock`.
- `DropZoneManager.UnlockItem(Rigidbody)`: `isKinematic = false`, hapus `DropZoneLock`, lepas `Occupant` zona.
- `DropZoneLock` marker component (IsLocked).
- `PlayerCarryController.TryPickUp` menolak item dengan `DropZoneLock`.
- Dipanggil otomatis di akhir snap (task0008).

### Out of scope

- Reset level/save integration (LevelTimeManager), UI indikator terkunci, perubahan scene/prefab.

## Technical approach

`LockItem` dipanggil dari `TrySnap` on-complete. Pemain tidak bisa mengambil karena `TryPickUp` mengecek `DropZoneLock` lebih dulu. Kinematic membuat objek kebal tumbukan dinamis. `UnlockItem` memulihkan untuk testing/reset.

## Files expected to change

- `Assets/Resources/Scripts/System/DropZoneManager.cs`
- `Assets/Script/CCC/PlayerCarryController.cs` (tolak item terkunci)
- `Assembly-CSharp.csproj` (shared)

## Interfaces and state

- Public API/type changes: `DropZoneManager.LockItem/UnlockItem`, komponen `DropZoneLock` (`IsLocked`, `MarkLocked()`).
- Runtime state changes: item kinematic + `DropZoneLock`; `DropZone.Occupant` tetap terisi hingga `UnlockItem`.
- Save/load impact: none.
- Scene/Inspector impact: tidak ada wiring wajib.
- Events and dependencies: dipanggil dari `TrySnap` on-complete.

## Invariants

- Tidak mengubah `.unity`, `.prefab`, `.meta`.
- Item terkunci tidak bisa di-grab lewat jalur `TryPickUp`.
- `UnlockItem` selalu melepas `Occupant` zona yang menunjuk item tersebut.

## Risks

- Jika barang sudah terkunci lalu pemain butuh memindahkan (mis. salah snap) — disediakan `UnlockItem` untuk debug/reset, belum ada UI "buka kunci" untuk pemain.
- Item kinematic tidak merespons fisika: bila kursi harus bisa di-push, matikan lock (gunakan unlock).

## Approval gate

- Required: Yes
- Reason: mengubah gameplay (grab rules).
- Approval: Approved by User, 2026-11-16 (chat: "kerjain ini bang").

## Acceptance criteria

- [x] Compile bersih (0 errors).
- [ ] Setelah snap, item tidak bisa di-grab (`TryPickUp` return false).
- [ ] Item kinematic tidak terdorong tumbukan.
- [ ] `UnlockItem` mengembalikan item ke dynamic & melepas zona.
