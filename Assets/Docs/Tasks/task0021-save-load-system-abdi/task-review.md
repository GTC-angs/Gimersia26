# Review — TASK-0021

## Result

PASS WITH RISKS

## What changed

- Membuat script `Assets/Resources/Scripts/System/SaveLoadSystem.cs`:
  - Kelas serializable `GameSaveData` dan `StageRecord`.
  - Metode `SaveGame()`, `LoadGame()`, `SaveStageRecord()`, `GetStageRecord()`, dan `ResetSaveData()`.
  - Dual persistence: file JSON fisik di `persistentDataPath` dan sinkronisasi ke `PlayerPrefs`.
  - Event `OnDataLoaded` dan `OnDataSaved`.

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Data bintang, rekor waktu, dan koin tersimpan permanen (menunggu smoke test Editor).
- [ ] Data dapat dibaca kembali saat game di-restart.
- [ ] Fitur reset data bekerja mengembalikan ke kondisi awal.

## Regression check

- Related feature: Save data.
- Result: 0 build errors.

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Known issues

- Perlu smoke test inspeksi via `Tools > CCG-Tools > PlayerPrefs Editor`.

## Follow-up tasks

None.
