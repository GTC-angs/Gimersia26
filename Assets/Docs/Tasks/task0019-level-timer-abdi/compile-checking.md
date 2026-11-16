# Compile Checking & Manual Test — TASK-0019 (Level Timer)

## Goal

Panduan pengujian Level Timer System (`LevelTimeManager.cs`) di Unity Editor.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene dengan GameObject `LevelTimeManager`.

---

## 🛠️ Setup di Scene

1. Buat Empty GameObject di scene, beri nama **`LevelTimeManager`**.
2. Tambahkan komponen **`LevelTimeManager`** (`Assets/Resources/Scripts/System/LevelTimeManager.cs`).
3. Atur parameter di Inspector:
   - `Level Duration`: `180` detik (atau `35` detik untuk tes cepat state kritis).
   - `Critical Time Threshold`: `30` detik.
   - `Auto Start`: Centang.

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Amati nilai `Remaining Time` di Inspector:
   - Nilai waktu harus berkurang mundur setiap detik.
3. Saat sisa waktu menyentuh 30 detik:
   - Console menampilkan log / event `OnCriticalTimeEntered` tertembak.
4. Uji coba tombol / panggil method `PauseTimer()`:
   - Countdown berhenti seketika.
5. Panggil `ResumeTimer()`:
   - Countdown kembali berjalan.
6. Panggil `AddBonusTime(15)`:
   - Sisa waktu bertambah 15 detik.
7. Biarkan waktu mencapai 0:
   - Event `OnTimeOut` tertembak tepat saat 00:00.

---

## ✅ Output yang Diharapkan

- Hitung mundur stabil dan akurat.
- Event pause, resume, bonus time, dan timeout berjalan mulus tanpa error.
