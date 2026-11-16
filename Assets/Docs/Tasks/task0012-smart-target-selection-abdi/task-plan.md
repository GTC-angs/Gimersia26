# TASK-0012 — Smart Target Selection (SYS-11)

## Goal

Mengimplementasikan seleksi target cerdas (*Smart Target Selection*) pada `PlayerInteractionController.cs` saat mendeteksi objek di sekitar pemain, dengan sistem pembobotan prioritas:
1. **Alat Pembersih (Cleaning Tools: Sapu, Pel, Kemoceng)**
2. **Barang Ringan / Sedang (Light & Medium Items: Buku, Piring, Kardus, Sampah)**
3. **Furnitur Berat (Heavy Furniture: Kulkas, Sofa, Kasur, Lemari)**

## User behavior

- Ketika pemain menekan/menahan tombol Grab di area di mana beberapa objek berada dalam radius jangkauan sekaligus (misal: alat pel tersandar di dekat sofa besar, atau kardus kecil di samping kulkas):
  - Sistem tidak hanya memilih objek dengan jarak terdekat murni.
  - Sistem mengutamakan alat pembersih terlebih dahulu, kemudian barang ringan, dan terakhir perabot berat.
  - Hal ini mempermudah pemain mengambil sapu/pel tanpa frustrasi tersangkut mengambil sofa atau furnitur berat di sekitarnya.

## Evidence

- Technical design:
  - `Assets/Docs/Pretelan Seluruh System Mekanik.md` §2.4 (*Smart Target Selection (Prioritas Jangkauan) [P1 - ANTI-BUG]*): "Raycast kerucut interaksi memilih objek dengan bobot prioritas: 1) Alat pembersih, 2) Barang ringan, 3) Furnitur berat. Mencegah salah ambil objek saat barang bertumpuk."
- Script terkait:
  - `Assets/Script/CCC/PlayerInteractionController.cs` (`GrabNearest()`).
  - `Assets/Script/CCC/PlayerCarryController.cs` (kategori bobot: `Light`, `Medium`, `Heavy`).
  - `Assets/Resources/Scripts/System/CleaningTool.cs` / Tagging `CleaningTool`.

## Scope

### In scope

- Evaluasi array hasil overlap sphere di `PlayerInteractionController.cs`.
- Penentuan tingkat prioritas objek berdasarkan tag / komponen / layer:
  - Skor/Priority 3: Alat pembersih (`CleaningTool` / tag `CleaningTool` / `Tool`).
  - Skor/Priority 2: Barang ringan / sedang (tanpa tag `Heavy` / bobot Light/Medium).
  - Skor/Priority 1: Furnitur berat (`CoOpLiftController` / tag `Heavy`).
- Algoritma pemeringkatan: Prioritas lebih tinggi selalu menang; jika prioritas sama, objek terdekat yang dipilih.

### Out of scope

- Perubahan pada scene `.unity`, `.prefab`, atau `.meta`.
- Mengubah logika carry socket dan drop.

## Technical approach

Modifikasi `GrabNearest()` di `PlayerInteractionController.cs`:
Hitung skor gabungan: `Score = PriorityWeight - (distance * distanceWeight)` atau kelompokkan kandidat berdasarkan tier prioritas tertinggi, kemudian ambil yang terdekat di tier tersebut.

## Files expected to change

- `Assets/Script/CCC/PlayerInteractionController.cs`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/task-status.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/task-review.md`
- `Assets/Docs/Tasks/task0012-smart-target-selection-abdi/compile-checking.md`

## Interfaces and state

- Public API/type changes: None.
- Runtime state changes: Logika filter & ranking di `GrabNearest()`.
- Save/load impact: None.
- Scene/Inspector impact: Menyediakan field tag / layer prioritas yang dapat disesuaikan di Inspector jika diperlukan.

## Invariants

- Tidak mengedit file `.unity` atau `.prefab` via teks.
- Objek `isKinematic` atau yang memiliki `DropZoneLock` tetap diabaikan.
- Zero compile errors.

## Risks

- Tag nama harus konsisten dengan yang digunakan proyek (misal `"Heavy"` untuk furnitur berat).

## Approval gate

- Required: Yes
- Reason: Mengubah logika seleksi interaksi objek pemain (SYS-11).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Saat alat pembersih dan furnitur berat berada di dekat pemain, alat pembersih yang terangkat terlebih dahulu.
- [ ] Saat barang ringan dan furnitur berat berdekatan, barang ringan yang terangkat terlebih dahulu.
- [ ] Jika tipe objek sama, objek terdekat yang terpilih.
