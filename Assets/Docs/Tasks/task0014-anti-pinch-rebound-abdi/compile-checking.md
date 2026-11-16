# Compile Checking & Manual Test — TASK-0014 (Anti-Pinch Rebound)

## Goal

Panduan pengujian mekanisme sensor anti-terjepit (*Anti-Pinch Rebound*) pada pintu di Unity Editor.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Pintu dengan komponen `DoorInteractionSystem` yang memiliki fitur anti-pinch aktif.

---

## 🛠️ Setup di Scene

1. Siapkan pintu `DoorInteractionSystem` dengan auto-close aktif (misal auto close setelah 2 detik).
2. Pastikan area kusen pintu memiliki sensor overlap atau child transform `_pinchSensorPoint`.
3. Pastikan `Obstacle Layer Mask` pada `DoorInteractionSystem` mencakup layer pemain (`Player`), barang yang dibawa (`CarriedItem`), dan furnitur (`Default` / `Furniture`).

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Buka pintu, lalu bawa karakter berjalan dan **berhenti tepat di tengah-tengah kusen pintu**.
3. Tunggu hingga timer auto-close habis dan pintu mulai menutup ke arah karakter.
4. **Amati Reaksi Pintu:**
   - Begitu daun pintu mendekati/sensor menyentuh collider karakter, pintu **TIDAK BOLEH terus mendorong atau menjepit karakter**.
   - Pintu harus seketika **membal terbuka kembali selebar 30 cm** (*rebound*) dan menjeda proses penutupan.
5. Ulangi tes dengan meletakkan perabot/kardus di tengah celah pintu. Pintu juga harus membal terbuka saat menyentuh perabot tersebut.

---

## ✅ Output yang Diharapkan

- Pintu tidak menjepit pemain atau perabot ke kusen dinding (*No Crushing*).
- Pintu membal mundur dan memberi ruang aman 30 cm.
- 0 Error di Console.
