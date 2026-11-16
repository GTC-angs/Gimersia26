# Compile Checking & Manual Test — TASK-0009 (SYS-08)

## Goal

Panduan lengkap wiring + test Static Kinematic Lock di Unity Editor.

## Prasyarat

- Unity 6000.1.7f1, project compile bersih.
- SYS-07 (task0008) berfungsi: snap otomatis di drop-zone.
- Scene: `DropZoneManager` + `DropZone` + furnitur + pemain dengan `PlayerCarryController`.

## Langkah tes

1. Play. Ambil furnitur, snap ke zona (SYS-07).
2. Setelah snap selesai (~0.45 s), coba grab furnitur lagi (E / Numpad 1):
   - **Harus gagal** — `TryPickUp` return false karena `DropZoneLock` terpasang.
3. Dorong furnitur dengan pemain/barang lain:
   - Furnitur **tidak bergerak** (kinematic).
4. Cek Inspector furnitur: `Is Kinematic == true`, komponen `DropZoneLock` terpasang.
5. (Debug) Panggil `DropZoneManager.Instance.UnlockItem(item)`:
   - `Is Kinematic == false`, `DropZoneLock` hilang, zona kosong lagi (`Occupant == null`), furnitur bisa di-grab lagi.

## Output yang diharapkan

- Item pasca-snap: kinematic, tidak bisa di-grab, tidak terdorong.
- `UnlockItem` memulihkan sepenuhnya.
- Tidak ada error di Console.

## Jika gagal

- **Bisa di-grab setelah snap:** `DropZoneLock` tidak terpasang — pastikan `LockItem` dipanggil (otomatis di akhir tween) atau cek `TrySnap` selesai; item harus non-null.
- **Item tetap terdorong:** `isKinematic` false — `LockItem` tidak jalan; cek `OnComplete` sequence DOTween (butuh DOTween Setup, lihat task0008 compile-checking.md).
- **Zona tetap "penuh" setelah unlock:** bug — laporkan; `UnlockItem` harus melepas `Occupant` yang cocok.
- **Compile error:** `dotnet build Assembly-CSharp.csproj --no-restore` di root project.
