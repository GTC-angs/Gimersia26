# Compile Checking & Manual Test — TASK-0017 (Hug Interaction)

## Goal

Panduan pengujian interaksi Peluk (*Hug Interaction* SYS-16) di Unity Editor.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene dengan 2 pemain (P1 dan P2) yang memiliki komponen `MoodChemistrySystem`.

---

## 🛠️ Setup di Scene

1. Pastikan `MoodChemistrySystem` memiliki referensi prefab partikel hati pada slot `_heartHugVfxPrefab`.
2. Atur `Hug Range = 2.0` meter dan `Hug Duration = 1.5` detik.

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Turunkan mood salah satu atau kedua karakter hingga menjadi rendah (misal 20%).
3. Bawa kedua karakter berdiri saling berhadapan dalam jarak dekat (< 2 meter).
4. Pemicu aksi Hug (via tombol / method debug `TryHug()`):
   - Kedua karakter terkunci sejenak selama 1.5 detik.
   - Partikel hati merah coral (`VFX_Heart_Hug`) meletup di antara kedua karakter.
   - Nilai mood kedua karakter melonjak naik +50% (misal dari 20% menjadi 70%).
5. Coba pemicu Hug saat kedua pemain berjauhan (> 2 meter):
   - Pelukan tidak dapat dieksekusi (ditolak karena di luar jangkauan).

---

## ✅ Output yang Diharapkan

- Mood pulih +50%.
- Efek visual hati muncul pas di tengah pelukan.
- Kunci pergerakan terlepas mulus setelah 1.5 detik.
- 0 Error di Console.
