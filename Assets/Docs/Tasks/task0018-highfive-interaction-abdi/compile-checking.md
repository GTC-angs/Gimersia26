# Compile Checking & Manual Test — TASK-0018 (High-Five Interaction)

## Goal

Panduan pengujian interaksi Tos Tangan (*High-Five Interaction* SYS-17) di Unity Editor.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene dengan 2 pemain (P1 dan P2) yang memiliki komponen `MoodChemistrySystem`.

---

## 🛠️ Setup di Scene

1. Pastikan `MoodChemistrySystem` memiliki referensi prefab partikel sparkle pada slot `_highFiveVfxPrefab`.
2. Atur `High Five Range = 2.0` meter dan `High Five Duration = 0.7` detik.

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Bawa kedua karakter berdiri saling berhadapan dalam jarak dekat (< 2 meter).
3. Pemicu aksi Tos Tangan (via tombol / method debug `TryHighFive()`):
   - Kedua karakter melakukan tos cepat (durasi 0.7 detik).
   - Efek partikel sparkle bintang emas (`VFX_HighFive_Spark`) meletup di antara kedua telapak tangan.
   - Nilai mood kedua karakter bertambah +25%.
4. Coba pemicu saat jarak > 2 meter:
   - Aksi tos tangan ditolak karena di luar jangkauan.

---

## ✅ Output yang Diharapkan

- Mood pulih +25%.
- Efek visual sparkle bintang meletup renyah.
- Durasi kunci sangat singkat (0.7 detik) tanpa mengganggu ritme gameplay.
- 0 Error di Console.
