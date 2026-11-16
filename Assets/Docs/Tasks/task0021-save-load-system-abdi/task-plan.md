# TASK-0021 — Save & Load System (SYS-20)

## Goal

Mengimplementasikan sistem penyimpanan dan pemuatan data permainan (`SaveLoadSystem.cs`):
- Menyimpan dan membaca progres level (stage yang terbuka, bintang emas tertinggi 1–3 bintang, rekor waktu terbaik, koin emas).
- Mendukung format JSON native (`JsonUtility`) dan integrasi cepat dengan `PlayerPrefs` yang kompatibel dengan plugin `PlayerPrefs Editor`.
- Menyediakan fungsi reset save data untuk kemudahan playtest.

## User behavior

- Setelah menyelesaikan level, perolehan bintang dan rekor waktu terbaik otomatis disimpan.
- Saat game dimainkan kembali atau pemain membuka layar Stage Select, data bintang dan rekor waktu sebelumnya otomatis terbaca.
- Di menu settings/testing, pemain atau developer dapat mereset seluruh save data untuk memulai dari kondisi bersih.

## Evidence

- GDD §14 (*Progression & Currency - bintang, score, time left, gold coin*).
- PackagesDocumentation §5 (*PlayerPrefs Editor*).
- Pretelan Sistem Mekanik §2 (SYS-20 Save & Load System [P0 - CORE MVP]) dan §4.4.2 (`StageSelectUI`).
- Agent rules: `Assets/Docs/Agentrule.md` (*Save System: JSON Native*).

## Scope

### In scope

- Membuat script `Assets/Resources/Scripts/System/SaveLoadSystem.cs`.
- Struktur data serializable: `GameSaveData`, `StageSaveData`.
- Metode: `SaveGame()`, `LoadGame()`, `SaveStageRecord(int stageIndex, int stars, float bestTime, int coinsEarned)`, `GetStageRecord(int stageIndex)`, `ResetSaveData()`.
- Dual backend: JSON file persistence di `Application.persistentDataPath` dengan fallback `PlayerPrefs`.

### Out of scope

- Cloud save / backend server eksternal.

## Technical approach

Menggunakan class serializable `GameSaveData` yang di-serialize via `JsonUtility.ToJson()` dan disimpan ke `savegame.json` atau `PlayerPrefs.SetString()`.

## Files expected to change

- `Assets/Resources/Scripts/System/SaveLoadSystem.cs`
- `Assembly-CSharp.csproj`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/task-status.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/task-review.md`
- `Assets/Docs/Tasks/task0021-save-load-system-abdi/compile-checking.md`

## Interfaces and state

- Class: `SaveLoadSystem`.
- Properties: `Data`, `TotalCoins`, `UnlockedStageCount`.
- Methods: `SaveStageRecord()`, `GetStageData()`, `ClearAllData()`.

## Invariants

- Data aman dari kerusakan korupsi (try-catch block pada IO file).
- Zero compile errors.

## Approval gate

- Required: Yes
- Reason: Sistem data persistensi permainan (SYS-20).
- Approval: Approved by User, 2026-11-16 (chat: "gas implement bang").

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Data bintang, rekor waktu, dan koin tersimpan permanen.
- [ ] Data dapat dibaca kembali saat game di-restart.
- [ ] Fitur reset data bekerja mengembalikan ke kondisi awal.
