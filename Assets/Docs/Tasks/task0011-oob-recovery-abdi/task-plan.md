# TASK-0011 — Out-of-Bounds Detection & Item Recovery (SYS-09 & SYS-10)

## Goal

Mengimplementasikan sistem deteksi objek jatuh ke luar batas peta (khususnya saat Y < -10 m atau menyentuh trigger area OOB) dan memulihkan objek tersebut (mereset velocity ke 0, me-respawn ke titik spawn/tengah ruangan asal, dan memicu efek kepulan asap kartun) untuk mencegah *softlock* kehilangan objek objektif game.

## User behavior

- Jika suatu perabot atau barang objektif (buku, piring, kardus, sofa, kulkas, dll.) terlempar atau jatuh ke luar batas peta (melewati trigger volume OOB atau mencapai koordinat `transform.position.y < -10f`):
  1. Kecepatan fisik (linear velocity dan angular velocity) objek seketika dinolkan (`Vector3.zero`).
  2. Posisi objek di-respawn kembali ke posisi awal / titik tengah ruangan asalnya (`spawnPosition` / `roomCenter`).
  3. Memunculkan efek visual kepulan asap kartun (`VFX_OOB_RespawnPuff`) di lokasi respawn untuk menyamarkan teleportasi.
  4. Game tetap berjalan lancar tanpa kehilangan barang penting (*Zero Softlock*).

## Evidence

- Technical design:
  - `Assets/Docs/Pretelan Seluruh System Mekanik.md` §2.3 (*Emergency Out-of-Bounds (OOB) Item Recovery [P1 - ANTI-BUG & ANTI-STUCK]*).
  - `Assets/Docs/Pretelan Seluruh System Mekanik.md` §5 (Checklist Zero-Bug: *NO LOST ITEM*).
  - `Assets/Docs/VFX List.md` §3.6 (`VFX_OOB_RespawnPuff` - Awan Respawn OOB [P1]).
- Agent rules: `Assets/Docs/Agentrule.md` (1 Feature = 1 Script, folder `Assets/Resources/Scripts/System/`, no scene/prefab direct edit, zero compile errors).
- Current gap: Belum ada script `ItemOutOfBoundsRecoverer.cs` di proyek.

## Scope

### In scope

- Membuat script baru `Assets/Resources/Scripts/System/ItemOutOfBoundsRecoverer.cs`.
- Deteksi OOB via dua metode:
  1. Trigger Collider callback (`OnTriggerEnter` pada bounding trigger volume).
  2. Fallback threshold ketinggian global (misal `transform.position.y < _deathYThreshold`, default `-10f`).
- Logika pemulihan item:
  - Mendukung pencatatan posisi spawn awal secara otomatis (`initialPosition` & `initialRotation`) atau titik referensi ruangan tertentu.
  - Reset `velocity` dan `angularVelocity` pada Rigidbody.
- Integrasi event/instansiasi efek asap kartun (VFX prefab slot atau trigger callback).
- Registrasi otomatis / tracking item di scene tanpa perlu setup rumit per item.

### Out of scope

- Mengubah file scene `.unity` atau `.prefab` langsung dari agent.
- Modifikasi internal sistem grab atau drop-zone yang sudah ada.

## Technical approach

1. Buat `ItemOutOfBoundsRecoverer.cs` di `Assets/Resources/Scripts/System/`:
   - Bisa dipasang sebagai Trigger Boundary di scene (Zone Tracker) ATAU sebagai Manager singleton/komponen yang memantau Rigidbody terdaftar/terdeteksi.
   - Pola paling modular & anti-stuck:
     - Komponen `ItemOutOfBoundsRecoverer` bertindak sebagai boundary detector (Trigger Collider) sekaligus fallback height checker (`Update` / `FixedUpdate` memeriksa list tracked Rigidbody atau objek yang masuk trigger).
     - Menyediakan juga helper tracking otomatis / component helper `RecoverableItem` atau dictionary cache posisi awal saat Awake/Start.
     - Saat recover:
       ```csharp
       rb.linearVelocity = Vector3.zero;
       rb.angularVelocity = Vector3.zero;
       rb.position = targetRespawnPosition;
       rb.rotation = targetRespawnRotation;
       if (_smokeVfxPrefab != null) Instantiate(_smokeVfxPrefab, targetRespawnPosition, Quaternion.identity);
       ```
2. Pastikan aman untuk objek yang sedang digrab (jika ada kasus ekstrim player ikut jatuh atau melempar barang).

## Files expected to change

- `Assets/Resources/Scripts/System/ItemOutOfBoundsRecoverer.cs` (file baru)
- `Assets/Resources/Scripts/System/ItemOutOfBoundsRecoverer.cs.meta` (dihasilkan oleh Unity)
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/task-status.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/task-review.md`
- `Assets/Docs/Tasks/task0011-oob-recovery-abdi/compile-checking.md`

## Interfaces and state

- Public API/type changes:
  - `ItemOutOfBoundsRecoverer`: `Recover(Rigidbody rb, Vector3 respawnPos)`, `RegisterItem(Rigidbody rb, Vector3 respawnPos)`.
  - Event `OnItemRecovered(Rigidbody rb, Vector3 respawnPos)`.
- Runtime state changes: Cache posisi awal tiap item.
- Save/load impact: None.
- Scene/Inspector impact: Komponen dipasang pada Trigger Boundary atau GameManager di scene oleh developer.

## Invariants

- Kecepatan fisik wajib dinolkan saat respawn.
- Tidak mengubah scene, prefab, atau .meta secara langsung via teks.
- Zero compile error.

## Risks

- Jika item tidak menyimpan posisi spawn awal, fallback ke spawn point terdekat atau center ruangan.
- Jika objek sedang di-parent/grab saat jatuh, lepas terlebih dahulu.

## Approval gate

- Required: Yes
- Reason: Fitur sistem baru (SYS-09 & SYS-10).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Deteksi OOB berfungsi saat item jatuh melewati `Y < -10f` atau masuk ke trigger zone OOB.
- [ ] Rigidbody linear & angular velocity dinolkan total saat respawn.
- [ ] Item kembali ke posisi spawn awal / titik tengah yang ditentukan.
- [ ] Efek asap kartun (`_smokeVfxPrefab`) muncul di titik respawn.
- [ ] Kompatibel dengan semua Rigidbody item interaktif.
