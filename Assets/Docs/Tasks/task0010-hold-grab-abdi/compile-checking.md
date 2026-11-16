# Compile Checking & Manual Test — TASK-0010 (Hold to Grab)

## Goal

Panduan lengkap setup/wiring komponen di Unity Editor dan cara melakukan smoke test untuk sistem kontrol **Hold to Grab & Release to Drop** (New Input System).

## Prasyarat

- Unity Editor v6000.1.7f1 (Unity 6 - URP).
- Project terkompilasi bersih tanpa error:
  ```powershell
  dotnet build Assembly-CSharp.csproj --no-restore
  ```
  *(Hasil wajib: 0 Errors).*
- Scene pengujian yang memiliki karakter player (misal: scene gameplay/testing yang sudah ada) atau setup manual minimal di bawah.

---

## 🛠️ Setup & Wiring di Unity Editor

Sesuai aturan `Agentrule.md`, seluruh modifikasi komponen/prefab dilakukan via Unity Editor oleh developer:

### 1. Setup Layer & Mask
1. Pastikan Layer `CarriedItem` sudah ada di Unity (`Edit > Project Settings > Tags and Layers`).
2. Pastikan objek yang ingin bisa diambil (kardus, buku, perabot, dll.) berada di Layer tertentu (misal Layer `Default` atau Layer khusus `Interactable`).
3. Objek interaktif **wajib** memiliki:
   - `Rigidbody` (dengan `isKinematic = false`).
   - `Collider` (Box/Sphere/Capsule Collider).

### 2. Setup Komponen di Karakter Player
Pada GameObject Player (P1 Suami / P2 Istri):
1. **`PlayerMovementController`**:
   - Atur `Is Husband`: centang untuk P1, jangan dicentang untuk P2.
   - Atur `Player Index`: 0 untuk P1, 1 untuk P2 (relevan saat menggunakan gamepad co-op).
2. **`PlayerCarryController`**:
   - `Hand Socket`: Assign transform anak di posisi tangan kanan/depan dada.
   - `Carry Pivot`: Assign transform anak di posisi dada karakter.
   - `Carried Layer Name`: Pastikan bernilai `"CarriedItem"`.
3. **`PlayerInteractionController`**:
   - `Item Mask`: Centang Layer objek barang (misal `Default` atau `Interactable`).
   - `Reach`: `0.9` (jarak jangkauan deteksi ke depan).
   - `Radius`: `0.7` (radius bola deteksi).
   - `Height`: `0.6` (tinggi offset titik deteksi dari kaki).

---

## 🎮 Kontrol yang Digunakan (New Input System)

| Peran Karakter | Keyboard | Gamepad |
| :--- | :--- | :--- |
| **Player 1 (Suami)** | Tahan tombol **`E`** | Tahan tombol **`South Button`** (`A` / `✕`) |
| **Player 2 (Istri)** | Tahan tombol **`Numpad 1`** atau **`Keypad Enter`** | Tahan tombol **`South Button`** (`A` / `✕`) gamepad 2 |

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Tekan Play)** di Unity Editor.
2. **Uji Coba Ambil Barang (Hold to Grab):**
   - Dekati objek barang hingga berada di depan karakter (radius < 0.9m).
   - **Tekan dan tahan** tombol Grab (`E` untuk P1 / `Numpad 1` untuk P2).
   - Karakter harus langsung mengangkat objek ke socket dada/tangan.
3. **Uji Coba Membawa Barang (Keep Holding):**
   - Sambil **tetap menahan** tombol Grab, gerakkan karakter (WASD atau analog).
   - Barang harus tetap menempel di socket karakter mengikuti pergerakan.
4. **Uji Coba Meletakkan Barang (Release to Drop):**
   - **Lepaskan** tombol Grab.
   - Barang harus seketika lepas dari tangan/dada karakter dan jatuh ke lantai dengan fisika normal.
5. **Uji Coba Co-Op Lift (Furnitur Berat):**
   - Dekati furnitur dengan komponen `CoOpLiftController`.
   - Tahan tombol Grab untuk memegang furnitur.
   - Lepas tombol Grab ➔ Pegangan furnitur harus terlepas secara aman.

---

## ✅ Output yang Diharapkan

- Barang hanya terangkat dan dibawa selama tombol ditekan/ditahan.
- Begitu tombol dilepas, barang langsung terlepas (*drop*) tanpa perlu menekan tombol kedua kalinya.
- Tidak ada error (*NullReferenceException*) di jendela Console saat mengambil atau melepas barang.

---

## ⚠️ Troubleshooting Jika Gagal

- **Barang tidak mau terangkat:**
  - Cek `Item Mask` pada `PlayerInteractionController`: pastikan mencakup Layer dari objek barang tersebut.
  - Cek objek: pastikan memiliki `Rigidbody` aktif dan tidak berstatus `isKinematic = true` (atau belum terkunci oleh `DropZoneLock`).
- **Barang langsung jatuh kembali saat tombol ditahan:**
  - Pastikan menggunakan New Input System keyboard/gamepad aktif dan window Game View di Unity sedang berada dalam fokus (*active focus*).
- **Console error "Layer 'CarriedItem' belum dibuat":**
  - Buat layer bernama `CarriedItem` di `Tags and Layers`.
