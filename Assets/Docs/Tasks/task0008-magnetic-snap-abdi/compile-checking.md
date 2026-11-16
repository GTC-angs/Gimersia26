# Compile Checking & Manual Test — TASK-0008 (SYS-07)

## Goal

Panduan lengkap wiring + test Magnetic Snap di Unity Editor.

## Prasyarat

- Unity 6000.1.7f1, project compile bersih.
- DOTween ter-setup: `Tools > Demigiant > DOTween Utility Panel` → klik **Setup DOTween** bila belum (pastikan selesai tanpa error).
- Scene dengan `DropZoneManager` + ≥ 1 `DropZone` + furnitur (Rigidbody + Collider) dan pemain dengan `PlayerCarryController` (atau pemanggilan manual `TrySnap`).
- Zona valid: jarak ≤ 0.5 m, sudut Y item vs `DropZone` ≤ 35°.

## Langkah tes

1. Play. Dekati furnitur, tekan grab (E / Numpad 1), bawa ke zona.
2. Lepas (drop) di dalam radius & sudut → amati furnitur **meluncur halus** ke posisi/rotasi ideal + membal kecil (punch scale).
3. Ulangi dengan drop di luar radius → furnitur tetap di tempat, tidak snap.
4. Ulangi dengan sudut > 35° → tidak snap.
5. Drop kedua barang di zona yang sama → zona terisi ditolak (barang kedua tidak snap).
6. Cek Console: `OnItemSnapped` (bila di-subscribe/log) setelah tween selesai (~0.45 s).

## Output yang diharapkan

- Snap: smooth tween ~0.25 s ke pose ideal, lalu punch scale 1.15x → kembali normal.
- `OnItemSnapped(item, zone)` terpanggil sekali per snap.
- Tidak ada tween error / warning DOTween di Console.

## Jika gagal

- **Tween tidak jalan / error "DOTween not initialized":** buka `Tools > Demigiant > DOTween Utility Panel` → Setup DOTween.
- **Barang tidak snap:** cek `DropZoneManager` ada di scene; cek radius/sudut zona; tambahkan `Debug.Log` di `TrySnap`.
- **Barang snap tapi melayang/miring:** `SnapRotation` miring — pastikan rotasi zona flat (hanya Y).
- **Barang tetap bisa diambil setelah snap:** itu ranah task0009 (`DropZoneLock`) — cek komponen `DropZoneLock` terpasang.
- **Compile error:** `dotnet build Assembly-CSharp.csproj --no-restore` di root project.
