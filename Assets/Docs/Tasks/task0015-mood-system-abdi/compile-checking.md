# Compile Checking & Manual Test — TASK-0015 (Mood System Core)

## Goal

Panduan pengujian core sistem mood pemain di Unity Editor.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene pengujian dengan karakter pemain memiliki komponen `MoodChemistrySystem`.

---

## 🛠️ Setup di Scene

1. Buka scene pengujian.
2. Tambahkan komponen **`MoodChemistrySystem`** pada GameObject Player (P1 & P2) atau GameObject Manager terpusat.
3. Atur parameter:
   - `Initial Mood`: `60` (Nyaman).
   - `Decay Interval`: `8.0` detik (-1 mood per interval).

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Amati di Inspector Unity nilai `Current Mood`:
   - Nilai awal harus `60`.
3. Biarkan waktu berjalan atau trigger tabrakan:
   - Nilai `Current Mood` berkurang perlahan.
4. Panggil test method via Inspector button / script `ModifyMood(+20)`:
   - Nilai mood bertambah dan tidak melebihi 100.
5. Cek Console untuk memastikan event `OnMoodChanged` dipanggil tanpa error.

---

## ✅ Output yang Diharapkan

- Mood bergerak mulus antara rentang 0% hingga 100%.
- Tidak ada error di Console.
