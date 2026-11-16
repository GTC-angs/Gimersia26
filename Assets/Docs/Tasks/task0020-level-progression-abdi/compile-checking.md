# Compile Checking & Manual Test — TASK-0020 (Level Progression)

## Goal

Panduan pengujian Level Progression System (`LevelProgressionManager.cs`) di Unity Editor.

## Prasyarat

- Unity Editor v6000.1.7f1.
- Project terkompilasi bersih (`dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors).
- Scene pengujian dengan GameObject `LevelProgressionManager`.

---

## 🛠️ Setup di Scene

1. Buat GameObject `LevelProgressionManager`.
2. Tambahkan komponen **`LevelProgressionManager`** (`Assets/Resources/Scripts/System/LevelProgressionManager.cs`).
3. Konfigurasi daftar stage:
   - Stage 0: `Level 0 - Dojo`
   - Stage 1: `Level 1 - Mudik Lebaran`
   - Stage 2: `Level 2 - Relokasi Antar Ruang`
   - Stage 3: `Level 3 - Grand Chaos`
4. Pastikan nama-nama scene pengujian sudah terdaftar di `File > Build Settings > Scenes In Build`.

---

## 🧪 Langkah Smoke Test

1. **Jalankan Scene (Play)**.
2. Panggil method `LoadStage(1)`:
   - Sistem memuat scene Level 1.
3. Panggil method `LoadNextLevel()`:
   - Sistem memuat scene berikutnya (Level 2).
4. Panggil `RestartCurrentLevel()`:
   - Scene yang sedang aktif di-reload dari awal.

---

## ✅ Output yang Diharapkan

- Perpindahan level dan pergantian scene berlangsung mulus.
- 0 Error di Console.
