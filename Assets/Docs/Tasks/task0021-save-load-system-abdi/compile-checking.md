# Compile Checking & Manual Test — TASK-0021 (Save & Load System)

## Goal

Panduan pengujian sistem penyimpanan data (`SaveLoadSystem.cs`) dan integrasi inspeksi dengan `PlayerPrefs Editor` di Unity Editor.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene pengujian dengan GameObject `SaveLoadSystem`.

---

## 🛠️ Setup di Unity Editor

1. Buat GameObject `SaveLoadSystem` di scene pertama / GameManager.
2. Tambahkan komponen **`SaveLoadSystem`** (`Assets/Resources/Scripts/System/SaveLoadSystem.cs`).
3. Buka jendela PlayerPrefs Editor:
   `Tools > CCG-Tools > PlayerPrefs Editor`.

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Simulasikan kemenangan level via debug method:
   `SaveLoadSystem.Instance.SaveStageRecord(stageIndex: 1, stars: 3, bestTime: 125.4f, coinsEarned: 100);`
3. Buka jendela **PlayerPrefs Editor**:
   - Pastikan muncul data save game / JSON string yang berisi data Stage 1, 3 Bintang, dan 100 Koin.
4. **Hentikan Scene (Stop)**, lalu **Jalankan kembali (Play)**:
   - Panggil `SaveLoadSystem.Instance.GetStageRecord(1)`.
   - Data harus tetap bernilai 3 Bintang dan 100 Koin (persistensi sukses).
5. Panggil `SaveLoadSystem.Instance.ResetSaveData()`:
   - Seluruh data kembali ke kondisi default (hanya Stage 0 yang terbuka).

---

## ✅ Output yang Diharapkan

- Data tersimpan dan terbaca dengan benar tanpa korupsi data.
- 0 Error di Console.
