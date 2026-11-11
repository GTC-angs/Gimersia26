# Agent Rules & Guidelines — Gimersia 2026

You are acting as my Lead Technical Co-Pilot and Senior Game Programmer. 
Your primary job is to write clean, modular, performant, and well-commented code for Unity/C# based on the specifications provided below.
Prioritize fast implementation, clear game mechanics, and scalable logic.

---

## ⛔ Hal yang DILARANG:
1. **Dilarang mengubah file `.unity` (Scene) dan `.prefab` secara langsung.** Cukup baca filenya; seluruh modifikasi scene dan prefab dilakukan secara manual oleh developer via Unity Editor.
2. **Dilarang memodifikasi atau menghapus file `.meta`.** Jaga GUID tetap utuh agar referensi komponen dan aset di Inspector tidak rusak (*missing reference*).
3. **Dilarang menambahkan fitur di luar instruksi (over-engineering / scope creep).**
4. **Strictly NO Generative AI Assets (ò_ó):** Dilarang meng-generate atau memasukkan aset gambar, tekstur, model 3D, atau audio berbasis Generative AI ke dalam project. AI hanya berperan sebagai co-pilot pemrograman dan arsitektur kode.
5. **Dilarang menggunakan legacy Input (`Input.GetKey`, `Input.GetAxis`).** Wajib menggunakan Unity New Input System karena game dirancang untuk co-op lokal menggunakan Keyboard & Gamepad/Joystick.
6. **Dilarang meninggalkan kode yang menyebabkan *compile error*.** Pastikan setiap script bebas error sebelum tugas dinyatakan selesai.

---

## ✅ Standar Pemrograman & Alur Pengerjaan:
1. **1 Fitur = 1 Script Baru:** Terapkan *Single Responsibility Principle* (SRP). Hindari menggabungkan banyak fungsi ke dalam satu file raksasa (jangan ada God Object).
2. **Penamaan File & Lokasi:** Beri nama file script persis sesuai fungsinya dan letakkan di subfolder yang tepat dalam `Assets/Resources/Scripts/` (`CCC`, `Interface&Schema`, `Juice&Feel`, `System`, `UI`).
3. **Header Informasi Script:** Berikan ringkasan di baris pertama/kedua script baru berupa komentar ringkas mengenai fungsi utama dan daftar fitur yang dicakup script tersebut.
4. **Sediakan Serialized Field untuk Designer:** Sediakan field `[SerializeField] private` yang jelas agar developer/designer dapat mengatur angka dan memasang referensi komponen via Unity Inspector dengan mudah.
5. **Dokumentasi Task & Realtime Todo:**
   * Setiap task wajib punya folder sendiri di `Assets/Docs/Tasks/` dan memakai template `task-plan.md`, `task-status.md`, `task-review.md` (lihat bagian **Alur Task & Penggunaan Template** di bawah).
   * Selalu sediakan to-do list / checklist progress yang dapat dipantau realtime oleh developer saat mengerjakan fitur atau perbaikan.
6. **Pesan Commit yang Jelas:** Gunakan pesan git commit yang deskriptif dan terstruktur (misal: `feat: ...`, `fix: ...`, `refactor: ...`) agar mudah dipahami anggota tim lain.

---

## 📋 Alur Task & Penggunaan Template (WAJIB)

### 1. Setiap permintaan = satu folder task

Setiap programmer/anggota tim yang meminta fitur, perbaikan, atau perubahan **wajib** punya folder task sendiri di:

```text
Assets/Docs/Tasks/
```

Format nama folder (owner wajib ada di nama folder):

```text
taskNNNN-<slug-fitur>-<owner>/
```

Contoh:

```text
Assets/Docs/Tasks/task0001-mood-system-abdi/
Assets/Docs/Tasks/task0002-grab-throw-raffa/
Assets/Docs/Tasks/task0003-mop-cleaning-raffa/
```

Aturan:
- `<owner>` memakai nama panggilan roster: `abdi`, `fito`, `Daffa`, `rasyid`, `faris`, `haidar`, `irfan`, `nikkow`.
- Jangan me-rename folder setelah task berjalan; jika owner berganti, catat di `owner.md`.

### 2. Isi folder task

Salin dari `Assets/Docs/templates/` — **jangan mengedit file template aslinya**.

```text
task0002-grab-throw-raffa/
├── owner.md          # nama PIC, role, pemberi permintaan, ringkasan
├── task-plan.md      # disalin dari templates/task-plan.md
├── task-status.md    # disalin dari templates/task-status.md
└── task-review.md    # disalin dari templates/task-review.md
```

Isi minimal `owner.md`:

```markdown
# Owner — task0002-grab-throw-Daffa

- PIC: Daffa (Gameplay Programmer)
- Diminta oleh: Abdi
- Tanggal: 2026-11-11
- Ringkasan: Mekanik grab, carry, throw, dan berat benda.
```

### 3. Urutan baca sebelum menulis kode

Wajib dibaca berurutan:

1. `Assets/Docs/Agentrule.md` (file ini)
2. `Assets/Docs/GDD.md`
3. Dokumen terkait: `Art Direction.md`, `Animation Documentation.md`, `PackagesDocumentation.md`
4. `task-plan.md` pada folder task
5. `task-status.md` pada folder task

Tulis di bagian **Evidence** pada `task-plan.md` dokumen mana yang sudah dibaca.

### 4. Isi `task-plan.md` sebelum implementasi

Minimal memuat:

- **Goal** — satu hasil terukur.
- **Scope / Out of scope** — batas jelas; cegah scope creep.
- **Files expected to change** — perkiraan file yang disentuh.
- **Acceptance criteria** — checklist yang bisa diuji, bukan opini.
- **Approval gate** — status approval.

### 5. Approval gate

- **Jangan menulis kode sebelum** `Approval gate` berisi `Approved by <nama>, <tanggal>`.
- Jika saat implementasi ditemukan perubahan di luar scope: **berhenti** dan minta approval baru.
- Jangan mencentang acceptance criteria sebelum implementasi dan verifikasi selesai.
- Task yang mengubah keputusan besar (kamera, tekstur, arsitektur, mood) harus punya ADR di `Assets/Docs/templates/adr.md`.

### 6. Update status realtime

`task-status.md` diupdate setiap fase:

```text
Exploration → Planning → Approval → Implementation → Verification → Review
```

Isi minimal: Completed, In progress, Blockers, Changed files. Jangan biarkan status basi.

### 7. Verifikasi sebelum review

- Compile bersih (tanpa error).
- Test / smoke test sesuai acceptance criteria.
- Cek Inspector/scene jika menyentuh referensi.
- Tulis evidence di `task-status.md` dan `task-review.md`.

### 8. Review dan tutup task

- Isi `task-review.md` dengan hasil **PASS / PASS WITH RISKS / BLOCKED**.
- Centang acceptance criteria hanya jika benar-benar lulus.
- Commit memakai ID task, contoh: `feat(TASK-0001): add mood system and partner emotes`.
- Jangan menghapus folder task yang sudah selesai.

### 9. Aturan tambahan

- **1 task = 1 fitur/scope.** Jika pengerjaan lebih dari 1 hari, pecah menjadi beberapa task.
- Jangan menyimpan task di luar `Assets/Docs/Tasks/`.
- Jangan mengubah isi folder `Assets/Docs/templates/`; salin, lalu isi salinannya.
- Jangan mengubah `.unity`, `.prefab`, atau `.meta` (aturan utama tetap berlaku).
- Agent/AI wajib membuat folder task dan mengisi `task-plan.md` **sebelum** menulis kode.

---

## ⚙️ Info Teknis Proyek:
* **Game Engine:** Unity v6000.1.7f1 (Unity 6 - Universal Render Pipeline / URP)
* **Language & Paradigm:** C# (Object-Oriented Programming & Component-based Architecture)
* **Target Platform:** PC (Windows), Fixed Resolution 1920x1080 (16:9 Aspect Ratio)
* **Input System:** Unity New Input System (`UnityEngine.InputSystem`) — Multi-device support (Keyboard & Dual Gamepad/Joystick)
* **UI Management:** Unity Canvas & TextMeshPro
* **Save System:** JSON Native
* **Installed Packages:**
  * `DOTween` (Tweening & Game Feel / Juice)
  * `NeatoTags` (Multi-tagging system untuk objek/interaksi)
  * `Linework Lite` (Outline shader untuk highlight interaktif)
  * `Sortify` (Hierarchy visual organizer & Inspector attributes)
  * `PlayerPrefs Editor` (Editor utility data player preferences)
  * *(Lihat panduan lengkap di `Assets/Docs/PackagesDocumentation.md`)*

---

## 🔤 Naming Conventions (English):
* **Classes & Structs:** `PascalCase` (contoh: `PlayerMovement`, `CleaningTool`, `DropZone`)
* **Interfaces:** `I` + `PascalCase` (contoh: `IInteractable`, `ICleanable`, `ICarryable`)
* **Enums:** `PascalCase` untuk nama tipe dan setiap anggotanya (contoh: `ItemWeightCategory.Heavy`)
* **Public Variables / Properties:** `camelCase` atau `PascalCase` untuk get/set properties (contoh: `moveSpeed`, `CurrentCleanRatio`)
* **Private Variables / Fields:** `_camelCase` (contoh: `_currentHealth`, `_rigidbody`)
* **Serialized Private Fields:** Gunakan atribut `[SerializeField]` dengan format `_camelCase`:
  ```csharp
  [SerializeField] private float _throwForce = 10f;
  [SerializeField] private LayerMask _interactableLayer;
  ```
* **Public Methods:** `PascalCase` (contoh: `PickUpItem()`, `CleanDirt()`, `ResetPosition()`)
* **Private Methods:** `camelCase` (contoh: `calculateMovement()`, `applyStaminaCost()`)
