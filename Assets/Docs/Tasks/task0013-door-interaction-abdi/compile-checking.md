# Compile Checking & Manual Test — TASK-0013 (Door Interaction)

## Goal

Panduan setup komponen dan pengujian sistem pintu ayun & geser (`DoorInteractionSystem.cs`) di Unity Editor.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene pengujian dengan frame pintu dan daun pintu (mesh sederhana Quad/Cube).

---

## 🛠️ Setup di Unity Editor

1. Buat GameObject Frame Pintu `Door_Swing`.
2. Buat child GameObject `Door_Hinge` (pivot engsel di tepi kusen).
3. Buat child di bawah `Door_Hinge` berupa Cube tipis sebagai daun pintu (misal ukuran: 1.0m lebar, 1.8m tinggi, 0.05m tebal).
4. Tambahkan komponen **`DoorInteractionSystem`** pada `Door_Swing` atau `Door_Hinge`:
   - `Door Type`: Pilih `Swing` atau `Sliding`.
   - `Door Transform`: Drag `Door_Hinge` (atau daun pintu).
   - `Open Angle`: `90` (untuk Swing).
   - `Open Offset`: `(1.0, 0, 0)` (untuk Sliding).
   - `Auto Close`: Centang jika ingin pintu menutup otomatis setelah beberapa detik.
5. (Opsional) Tambahkan child `ProximityTrigger` dengan BoxCollider (`IsTrigger = true`) selebar 2 meter di depan pintu.

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Dekati pintu atau panggil tombol/trigger buka.
3. Amati animasi pintu membuka:
   - Jika `Swing`: Berputar 90° mulus.
   - Jika `Sliding`: Bergeser ke samping sejauh 1.0m mulus.
4. Tunggu atau tinggalkan pintu: pintu harus menutup kembali secara mulus ke posisi semula.

---

## ✅ Output yang Diharapkan

- Pintu terbuka dan tertutup tanpa patah-patah.
- 0 Error di Console.
