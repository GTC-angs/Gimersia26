# Compile Checking & Manual Test — TASK-0007 (SYS-06)

## Goal

Panduan lengkap wiring manual di Unity Editor + cara test Drop-Zone Detection.

## Prasyarat

- Unity 6000.1.7f1, project compile bersih (`dotnet build Assembly-CSharp.csproj --no-restore` → 0 errors).
- Scene test (mis. `testing_placement_lvl1.unity`) dengan 1 furnitur (Rigidbody + Collider).
- Marker mint: buat Quad 1×1 m, rotasi -90° X (datar di lantai), material transparan warna `#9FE3C2` (alpha ~0.35). Atau pakai plane sederhana.

## Wiring (manual di Editor)

1. Buat Empty GameObject `DropZone_Kulkas` di posisi ideal kulkas (origin di lantai, menghadap arah yang benar).
2. Tambahkan komponen `DropZone`:
   - `Snap Point`: kosongkan (pakai transform sendiri) atau drag child Empty sebagai target pose.
   - `Radius`: 0.5, `AngleTolerance`: 35, `Marker Color`: default mint.
   - `Marker Renderer`: drag Quad marker child.
3. Pastikan ada satu `DropZoneManager` di scene (atau biarkan auto-find di `DropZone.OnEnable` — manager harus ada lebih dulu; buat Empty `DropZoneManager` + komponen `DropZoneManager`).
4. Furnitur: pastikan punya `Rigidbody` (non-kinematic) + `Collider`.
5. Arahkan gizmo: `DropZone` menggambar wire sphere radius 0.5 di Scene view saat object terpilih/aktif.

## Langkah tes

1. Play. Ambil furnitur (sistem grab Raffa) dan letakkan di dalam radius 0.5 m dari zona, sudut menghadap ±35° dari `SnapRotation`.
2. Lepas — (di task0007 belum ada snap; cukup amati `DropZone.Occupant` kosong dan `FindZoneFor` via debug):
   - Tambahkan sementara `Debug.Log(DropZoneManager.Instance.FindZoneFor(item))` di titik drop, atau
   - cek Gizmos radius di Scene view.
3. Letakkan furnitur DI LUAR radius / sudut > 35° → `FindZoneFor` harus mengembalikan null.
4. Isi zona manual: panggil `zone.Occupy(item)` via debug → `FindZoneFor` tidak mengembalikan zona itu lagi.

## Output yang diharapkan

- Zona valid terdeteksi hanya dalam radius 0.5 m & sudut ≤ 35°.
- Zona terisi ditolak.
- Tidak ada error di Console; gizmo mint tampak di Scene view.

## Jika gagal

- **Zona tidak terdeteksi:** pastikan `DropZoneManager` ada di scene SEBELUM zona aktif (OnEnable order), atau set `Auto Find Zones On Awake = true` dan zona non-aktif saat start.
- **Sudut selalu gagal:** cek sumbu Y `SnapRotation`; gunakan `transform.rotation` yang flat (jangan miring).
- **Gizmo tidak muncul:** pastikan `DropZone` aktif; gizmo digambar di `OnDrawGizmos`.
- **Marker tidak transparan:** material Quad harus Transparent/Unlit dengan alpha 0.3–0.4.
