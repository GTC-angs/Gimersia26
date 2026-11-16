# TASK-0020 — Level Progression System (SYS-19)

## Goal

Mengimplementasikan sistem progresi tahapan level (`LevelProgressionManager.cs`):
- Mengatur data daftar stage/level:
  - Level 0: Tutorial Dojo Martial Arts
  - Level 1: Mudik Lebaran
  - Level 2: Relokasi Antar Ruang
  - Level 3: Grand Chaos
- Menyediakan fungsi navigasi level: `LoadNextLevel()`, `RestartCurrentLevel()`, `LoadStage(int stageIndex)`, `LoadMainMenu()`.
- Menyediakan status unlock level dan pencapaian bintang per stage.

## User behavior

- Setelah menyelesaikan suatu level dan melihat layar skor, pemain dapat menekan tombol "Lanjut ke Level Berikutnya".
- Level berikutnya otomatis dimuat secara mulus melalui `SceneManager.LoadScene()`.
- Di layar pemilihan stage, pemain dapat melihat level mana saja yang sudah terbuka (*unlocked*).

## Evidence

- GDD §6 (*Desain Level & Progresi - Level 0 sampai Level 3*) dan §14 (*Progression & Currency - unlock map baru, ruangan baru*).
- Pretelan Sistem Mekanik §2 (SYS-19 Level Progression [P0 - CORE MVP]).
- Agent rules: `Assets/Docs/Agentrule.md`.

## Scope

### In scope

- Membuat script `Assets/Resources/Scripts/System/LevelProgressionManager.cs`.
- Struktur data `StageInfo` (nama level, scene name, target threshold, status unlocked).
- Metode navigasi scene Unity (`SceneManager.LoadScene`).
- C# Event: `OnLevelStarted(int stageIndex)`, `OnLevelCompleted(int stageIndex, int starsEarned)`.
- Singleton instance persistent (`DontDestroyOnLoad` opsional).

### Out of scope

- Penyimpanan permanen ke disk (ditangani oleh `SaveLoadSystem.cs` / SYS-20).
- Desain UI pemilihan stage (ditangani oleh tim UI).

## Technical approach

Menggunakan ScriptableObject atau runtime registry daftar stage:
- Menyimpan index level aktif `_currentStageIndex`.
- Memanggil `SceneManager.LoadScene(nextSceneName)`.

## Files expected to change

- `Assets/Resources/Scripts/System/LevelProgressionManager.cs`
- `Assembly-CSharp.csproj`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/task-status.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/task-review.md`
- `Assets/Docs/Tasks/task0020-level-progression-abdi/compile-checking.md`

## Interfaces and state

- Methods: `LoadNextLevel()`, `RestartLevel()`, `LoadStage(int index)`.
- Properties: `CurrentStageIndex`, `CurrentStageInfo`, `TotalStages`.

## Invariants

- Validasi index stage agar tidak terjadi out of bounds.
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Sistem alur progresi level inti game (SYS-19).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Daftar stage Level 0–3 terkonfigurasi dengan benar.
- [ ] Fungsi `LoadNextLevel()` dan `RestartCurrentLevel()` bekerja memuat scene target.
- [ ] Menyediakan API untuk membaca status unlock stage.
