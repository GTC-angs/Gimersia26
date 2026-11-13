# TASK-0004 — Dynamic 3-Mode Camera & Screen Leash

## Goal

Mengimplementasikan perpindahan Overview → Shared → Vertical Split secara halus, serta mekanisme anti-straggle yang aman untuk dua pemain.

## User behavior

- Level dimulai dengan overview camera selama 3 detik pada jarak 22 m dan FOV 30°.
- Setelah overview, kamera memakai shared midpoint ketika pemain masih berada di area/ruangan yang sama.
- Kamera berubah ke vertical split ketika jarak pemain > 9 m atau pemain berada di ruangan berbeda.
- Transisi mode tidak snap dan setiap viewport tetap mengikuti pemain yang benar.
- Jika pemain tertinggal lebih dari 15 detik, indikator partner muncul.
- Pemain dapat menahan tombol interact selama 3 detik untuk teleport ke posisi aman di dekat partner.
- Camera harus berada di puncak (bird eye + isometric view)

## Evidence

- Code/asset: `Assets/Script/CCC/DynamicCameraController.cs` masih berupa stub.
- Technical design: `Assets/Docs/Pretelan Seluruh System Mekanik.md` §1.6 dan §4.2.4.
- GDD/PLGDD: `Assets/Docs/Art Direction.md` §3.1 dan `Assets/Docs/GDD.md` §5–§6.
- Current gap: belum ada mode camera, split viewport, partner leash, atau teleport safety.

## Scope

### In scope

- Implementasi runtime `DynamicCameraController.cs`.
- Overview, Shared, dan Vertical Split camera mode.
- Smooth camera transitions dan viewport assignment.
- Deteksi jarak/ruangan berbeda dengan API konfigurabel.
- Straggle timer 15 detik, partner direction/visibility state, dan teleport hold 3 detik.
- Safe teleport position di samping partner memakai probe collision.
- Input New Input System melalui keyboard/gamepad yang sudah dipakai controller proyek.

### Out of scope

- Perubahan langsung pada `.unity` atau `.prefab`.
- Pembuatan UI compass artistik atau prefab UI baru.
- Sistem room manager baru; room separation menggunakan callback/konfigurasi opsional.
- Perubahan movement, stamina, save/load, atau gameplay interaction controller.

## Technical approach
Satu controller mengelola dua camera child dan dua `Camera.rect` viewport. Overview dan Shared memakai kamera gabungan; Split memakai kamera per-player. Semua perubahan posisi, orthographic/perspective settings, FOV, dan rect diinterpolasi. Room separation disediakan melalui `Func`/komponen marker sederhana yang dapat dipasang di Inspector tanpa mengubah scene dari agent. Teleport hanya dilakukan ketika target probe bebas dan tombol interact ditahan penuh selama 3 detik.

## Files expected to change

- `Assets/Script/CCC/DynamicCameraController.cs`
- `Assets/Docs/Tasks/task0004-dynamic-camera-abdi/owner.md`
- `Assets/Docs/Tasks/task0004-dynamic-camera-abdi/task-plan.md`
- `Assets/Docs/Tasks/task0004-dynamic-camera-abdi/task-status.md`
- `Assets/Docs/Tasks/task0004-dynamic-camera-abdi/task-review.md`

## Interfaces and state

- Public API/type changes: expose camera mode, partner compass state, and teleport state untuk UI/diagnostic.
- Runtime state changes: active mode, transition progress, last-near-partner timestamp, interact hold duration.
- Save/load impact: none.
- Scene/Inspector impact: membutuhkan referensi dua player, camera, optional room resolver, dan optional compass/teleport callbacks.
- Events and dependencies: Unity `Camera`, `Physics`, dan New Input System API.

## Invariants

- Tidak mengubah posisi gameplay player selama camera transition.
- Tidak teleport ke collider solid atau posisi di luar ground probe.
- Tidak mengaktifkan split jika salah satu player/camera belum valid.
- Tidak menggunakan legacy `Input.GetKey` atau API input lama.
- Tidak mengubah scene, prefab, atau `.meta` melalui agent.

## Risks

- Tanpa scene wiring, fitur perlu dipasang manual oleh developer melalui Inspector.
- Deteksi “ruangan berbeda” membutuhkan marker/room resolver dari scene.
- Camera controller yang terlalu banyak memiliki tanggung jawab dapat menyulitkan UI; state akan diekspos sebagai read-only API.

## Approval gate

- Required: Yes
- Reason: task menambah perilaku camera runtime dan public integration state.
- Approval: Approved by User, 2026-11-13.
- Approved scope:
  - Dynamic overview/shared/vertical split camera.
  - Straggle indicator state after 15 seconds.
  - Safe teleport after 3 seconds of interact hold.
  - Runtime-only implementation; no direct scene/prefab/meta edits.
- Conditions:
  - Manual Unity Inspector wiring is documented for the developer.
  - Room separation can be supplied by room roots or `SetRoomDifferent(bool)`.

## Acceptance criteria

- [ ] Overview aktif 3 detik, jarak 22 m, FOV 30°.
- [ ] Shared camera mengikuti midpoint dan split aktif pada jarak > 9 m atau room berbeda.
- [ ] Split-screen vertikal memakai viewport kiri/kanan dan transisi halus.
- [ ] Partner indicator state aktif setelah 15 detik tertinggal.
- [ ] Teleport hanya terjadi setelah interact ditahan 3 detik dan posisi aman ditemukan.
- [ ] Script tidak menghasilkan compile error dan tidak mengubah scene/prefab/meta.
