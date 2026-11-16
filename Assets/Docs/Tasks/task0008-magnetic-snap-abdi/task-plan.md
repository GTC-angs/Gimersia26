# TASK-0008 — Magnetic Snap (SYS-07)

## Goal

Barang yang diletakkan dalam radius 0.5 m & sudut ±35° dari drop-zone otomatis tertarik (snap) ke posisi/rotasi ideal menggunakan DOTween.

## User behavior

- Saat pemain drop furnitur di atas zona valid, furnitur meluncur halus ke pose ideal + punch scale kecil (juice).
- Event `OnItemSnapped(item, zone)` dilempar setelah snap selesai.
- Barang di luar radius/sudut tidak kena snap.

## Evidence

- Code/asset: `Assets/Resources/Scripts/System/DropZoneManager.cs` (metode `TrySnap`)
- Technical design: `Assets/Docs/Pretelan Seluruh System Mekanik.md` §2.2, `VFX List.md` §3.2–3.3, `Animation Documentation.md` §7 (DOTween)
- GDD/PLGDD: GDD §5.2 poin 5 (Drop-Zone & Snapping).
- Current gap: drop hanya menaruh barang di titik aman; tidak ada snap magnetik ke pose ideal.

## Scope

### In scope

- `DropZoneManager.TrySnap(Rigidbody)`: cari zona valid (SYS-06), tween `DOMove` + `DORotateQuaternion` (Ease.OutQuad, default 0.25 s), lalu `DOPunchScale` 1.15.
- Integrasi ke `PlayerCarryController.TryDrop()`: setelah drop, coba snap.
- Event `OnItemSnapped`.

### Out of scope

- Lock kinematis pasca-snap (task0009), VFX sparkle/ring (Fito), SFX "Ting!" (Irfan), perubahan scene/prefab.

## Technical approach

Setelah `PlayerCarryController.TryDrop` memposisikan item, ia memanggil `DropZoneManager.Instance.TrySnap(item)`. Manager mencari zona valid terdekat; jika ada, item di-set kinematic sementara, tween DOTween menjalankan posisi+rotasi+punch, di akhir `LockItem` (task0009) dan event.

## Files expected to change

- `Assets/Resources/Scripts/System/DropZoneManager.cs`
- `Assets/Script/CCC/PlayerCarryController.cs` (panggil TrySnap di TryDrop)
- `Assembly-CSharp.csproj` (Compile item — shared dengan task0007)

## Interfaces and state

- Public API/type changes: `DropZoneManager.TrySnap`, event `OnItemSnapped`.
- Runtime state changes: item menjadi kinematic selama/ setelah snap; `DropZone.Occupant` terisi.
- Save/load impact: none.
- Scene/Inspector impact: DOTween terpasang di `Assets/Plugins/Demigiant` (sudah ada). Tidak ada wiring wajib di Inspector selain zona (task0007).
- Events and dependencies: `DG.Tweening`, `PlayerCarryController` → `DropZoneManager`.

## Invariants

- Tidak mengubah `.unity`, `.prefab`, `.meta`.
- Tween memakai `.SetLink`/Kill aman: `t.DOKill()` sebelum sequence baru.
- Snap hanya untuk zona valid (radius & sudut); tidak memaksa snap dari jauh.
- Item tetap kinematic setelah snap (SYS-08).

## Risks

- DOTween belum di-"Setup" (Tools > Demigiant > DOTween Utility Panel) dapat menyebabkan tween gagal jalan — wajib dicek developer.
- Jika item di-drop lalu langsung diambil sebelum snap selesai, `DOKill` + snap baru menimpa dengan aman.

## Approval gate

- Required: Yes
- Reason: mengubah perilaku drop (carry system).
- Approval: Approved by User, 2026-11-16 (chat: "kerjain ini bang").

## Acceptance criteria

- [x] Compile bersih (0 errors).
- [ ] Drop di zona valid → item tween ke pose ideal + punch scale.
- [ ] Drop di luar zona → tidak ada snap.
- [ ] `OnItemSnapped` terpanggil setelah snap selesai.
