# TASK-0007 — Drop-Zone Detection (SYS-06)

## Goal

Setiap furnitur memiliki area drop-zone transparan (mint #9FE3C2) yang bisa dideteksi sistem sebagai target penataan valid.

## User behavior

- Drop-zone tampak sebagai marker mint transparan di lantai/dinding.
- Sistem bisa menanyakan zona mana yang valid untuk sebuah barang (`DropZoneManager.FindZoneFor`).
- Zona terisi tidak menerima barang lain (`Occupant`).

## Evidence

- Code/asset: `Assets/Resources/Scripts/System/DropZone.cs`, `DropZoneManager.cs` (baru)
- Technical design: `Assets/Docs/Pretelan Seluruh System Mekanik.md` §2.2, `Art Direction.md` §9 (Mint #9FE3C2), `GDD.md` §5.2 poin 5
- GDD/PLGDD: Art Direction — drop-zone outline mint 1.5 px, marker transparan 30–40%.
- Current gap: belum ada komponen drop-zone atau deteksi radius/sudut.

## Scope

### In scope

- `DropZone`: radius (default 0.5 m), toleransi sudut Y (default 35°), snap point, warna marker mint, gizmo, status `Occupant`.
- `DropZoneManager`: singleton registry zona, `RegisterZone`/`UnregisterZone`, `FindZoneFor(Rigidbody)` (radius XZ + sudut), auto-find di Awake.

### Out of scope

- Snap/lock (task0008, task0009), VFX pulse (`VFX_DropZone_Pulse`, Fito), pembuatan prefab/marker/material (manual developer), scene edit.

## Technical approach

`DropZone` menyimpan konfigurasi zona; `DropZoneManager` (singleton scene) meregistrasi zona aktif dan menyediakan query "zona valid terdekat". Deteksi memakai jarak bidang XZ (`worldCenterOfMass`) dan `Mathf.DeltaAngle` pada sumbu Y. Zona terisi (`Occupant != null`) ditolak.

## Files expected to change

- `Assets/Resources/Scripts/System/DropZone.cs` (baru)
- `Assets/Resources/Scripts/System/DropZoneManager.cs` (baru)
- `Assembly-CSharp.csproj` (tambah 2 Compile item)

## Interfaces and state

- Public API/type changes: `DropZone` ( Radius, AngleTolerance, SnapPosition, SnapRotation, IsInRange, Occupant), `DropZoneManager.Instance`, `RegisterZone/UnregisterZone/FindZoneFor`, event `OnItemSnapped` (dipakai task0008).
- Runtime state changes: `Occupant` per zona.
- Save/load impact: none.
- Scene/Inspector impact: developer membuat marker (quad transparan mint) + komponen `DropZone` + opsional `DropZoneManager` (atau auto-find).
- Events and dependencies: `DropZoneManager.OnItemSnapped`.

## Invariants

- Tidak mengubah `.unity`, `.prefab`, `.meta`.
- Warna default marker = #9FE3C2 dengan alpha ~0.4.
- Semua angka via `[SerializeField]`.

## Risks

- Marker visual (quad + material transparan) harus dibuat manual di Editor; script hanya menyediakan gizmo + warna referensi.
- Zona ganda tumpang tindih: diselesaikan dengan memilih zona terdekat di `FindZoneFor`.

## Approval gate

- Required: Yes
- Reason: sistem penataan inti P0.
- Approval: Approved by User, 2026-11-16 (chat: "kerjain ini bang").

## Acceptance criteria

- [x] Compile bersih (0 errors).
- [ ] `FindZoneFor` mengembalikan zona terdekat dalam 0.5 m & sudut ≤ 35°.
- [ ] Zona terisi tidak mengembalikan hasil.
- [ ] Gizmo zona tampil di Scene view.
