# Compile Checking & Manual Test — TASK-0011 (SYS-09 & SYS-10)

## Goal

Panduan lengkap setup komponen di Unity Editor dan cara melakukan pengujian sistem deteksi jatuh ke luar peta (*Out-of-Bounds Detection*) dan pemulihan objek (*Item Recovery & VFX*).

## Prasyarat

- Unity Editor v6000.1.7f1 (Unity 6 - URP).
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene pengujian dengan minimal 1 objek perabot/barang ber-Rigidbody dan lantai dengan tepi jurang/tepi level.

---

## 🛠️ Setup & Wiring di Unity Editor

Sesuai aturan arsitektur:

### 1. Setup Manager / Bounding Volume
1. Buat GameObject baru di scene, beri nama **`OOB_Recoverer`**.
2. Tambahkan komponen **`ItemOutOfBoundsRecoverer`** (`Assets/Resources/Scripts/System/ItemOutOfBoundsRecoverer.cs`).
3. Opsi Deteksi Trigger (Opsional tapi direkomendasikan):
   - Tambahkan komponen `BoxCollider` pada `OOB_Recoverer`.
   - Centang **`Is Trigger = true`**.
   - Perbesar ukuran BoxCollider menjadi kotak besar di bawah lantai level (misal Y = -2, Size = 100, 2, 100).
4. Opsi Fallback Height (Otomatis):
   - Atur parameter `Death Y Threshold = -10.0` di Inspector. Objek apa pun yang menembus koordinat Y < -10 akan otomatis tertangkap meskipun tidak menyentuh trigger.
5. Slot VFX Asap Kartun:
   - Drag prefab partikel asap (misal `VFX_OOB_RespawnPuff` atau partikel asap yang tersedia) ke slot `Smoke Vfx Prefab`.

### 2. Setup Objek Barang yang Bisa Jatuh
- Objek barang (kardus, piring, sofa, dll.) hanya membutuhkan komponen standar:
  - `Rigidbody` (non-kinematic).
  - `Collider`.
- Script `ItemOutOfBoundsRecoverer` secara otomatis merekam posisi awal (*initial spawn position & rotation*) saat game dimulai, sehingga developer tidak perlu memasang komponen satu per satu ke setiap barang.

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)** di Unity Editor.
2. **Uji Coba Jatuhkan Barang:**
   - Dorong atau lempar objek keluar dari tepi lantai/pagar ruangan hingga jatuh ke bawah melewati Y < -10 atau menyentuh trigger box di bawah.
3. **Amati Proses Recovery:**
   - Objek harus seketika menghilang dari jurang/bawah dan muncul kembali di posisi spawn awal / titik tengah ruangan.
   - Partikel asap kartun meletup di titik munculnya objek.
   - Objek langsung diam (kecepatan linear & angular velocity 0) dan tidak membal liar.
4. **Uji Coba Ulang:**
   - Ulangi lemparan ke arah jurang yang berbeda untuk memastikan objek selalu kembali dengan aman tanpa softlock.

---

## ✅ Output yang Diharapkan

- Objek yang jatuh ke luar batas peta di-respawn ke posisi semula dengan kecepatan 0.
- Muncul kepulan asap kartun di posisi respawn.
- Tidak ada error (*NullReferenceException*) di Console.
- Game tidak mengalami softlock akibat barang objektif hilang.

---

## ⚠️ Troubleshooting Jika Gagal

- **Objek tidak kembali saat jatuh:**
  - Cek nilai koordinat Y objek di Transform saat jatuh; jika belum menyentuh -10 dan tidak ada trigger collider, sesuaikan `Death Y Threshold` (misal ubah jadi `-5.0` untuk pengetesan cepat).
  - Pastikan objek memiliki `Rigidbody`.
- **Objek membal kencang setelah respawn:**
  - Pastikan Rigidbody velocity benar-benar di-reset ke nol.
- **Asap tidak muncul:**
  - Pastikan slot `Smoke Vfx Prefab` sudah diisi prefab partikel aktif.
