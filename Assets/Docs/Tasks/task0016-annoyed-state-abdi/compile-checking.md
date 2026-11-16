# Compile Checking & Manual Test — TASK-0016 (Annoyed State Debuff)

## Goal

Panduan pengujian efek berkurangnya kecepatan gerak saat mood < 25% (SYS-15).

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene pengujian dengan karakter pemain memiliki komponen `PlayerMovementController` dan `MoodChemistrySystem`.

---

## 🛠️ Setup di Scene

1. Pastikan Player memiliki komponen `MoodChemistrySystem`.
2. Buka Inspector komponen `MoodChemistrySystem`.

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Gerakkan karakter saat Mood = 60%:
   - Karakter bergerak dengan kecepatan normal (100%).
3. Ubah nilai `Current Mood` di Inspector menjadi `20` (State Annoyed / < 25%):
   - Gerakkan kembali karakter.
   - Kecepatan karakter terasa melambat sekitar 15% (kecepatan menjadi 85% dari normal).
4. Naikkan kembali nilai `Current Mood` menjadi `50`:
   - Kecepatan karakter seketika kembali lincah ke 100%.

---

## ✅ Output yang Diharapkan

- Kecepatan berkurang 15% hanya saat mood < 25%.
- Karakter tidak macet atau berhenti bergerak total.
- 0 Error di Console.
