# Compile Checking & Manual Test — TASK-0012 (Smart Target Selection)

## Goal

Panduan pengujian seleksi target cerdas (Smart Target Selection SYS-11) pada `PlayerInteractionController.cs`.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene pengujian dengan karakter pemain dan beberapa objek bertumpuk/berdekatan:
  - 1 Alat Pembersih (misal pel / sapu).
  - 1 Barang Ringan (misal kardus / buku).
  - 1 Furnitur Berat (misal sofa / kulkas bertag `Heavy` atau ber-`CoOpLiftController`).

---

## 🛠️ Setup di Scene

1. Letakkan sebuah furnitur berat (misal Sofa ber-tag `"Heavy"`).
2. Tepat di samping atau di atas sofa tersebut, taruh kardus kecil (barang ringan).
3. Di samping kardus tersebut, taruh alat pembersih (misal Pel).

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Arahkan karakter ke arah ketiga objek tersebut sehingga ketiganya berada di dalam radius jangkauan (`Reach = 0.9m`).
3. Tekan/Tahan tombol Grab:
   - **Hasil Pertama:** Karakter harus mengambil **Alat Pembersih** terlebih dahulu.
4. Letakkan / buang alat pembersih, lalu dekati kembali sofa dan kardus yang berdekatan.
5. Tekan/Tahan tombol Grab:
   - **Hasil Kedua:** Karakter harus mengambil **Kardus (Barang Ringan)** terlebih dahulu, bukan sofa.
6. Letakkan kardus, tersisa hanya furnitur berat.
7. Tekan/Tahan tombol Grab:
   - **Hasil Ketiga:** Karakter baru mengambil/menggeser **Furnitur Berat**.

---

## ✅ Output yang Diharapkan

- Alat Pembersih > Barang Ringan > Furnitur Berat.
- Pemain tidak salah ambil objek besar saat ingin mengambil alat kecil di dekatnya.
- 0 Error di Console.
