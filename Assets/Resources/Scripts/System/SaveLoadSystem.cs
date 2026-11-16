// SaveLoadSystem: Menyimpan dan memuat progres pemain (bintang emas 1-3, rekor waktu terbaik,
// total koin, dan unlockable stages) menggunakan JSON Native dan PlayerPrefs sync (SYS-20).
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Gimersia.System
{
    [global::System.Serializable]
    public class StageRecord
    {
        public int StageIndex;
        public int StarsEarned;      // 0 - 3 Bintang
        public float BestTimeLeft;   // Rekor sisa waktu tertinggi
        public bool IsCompleted;

        public StageRecord(int stageIndex)
        {
            StageIndex = stageIndex;
            StarsEarned = 0;
            BestTimeLeft = 0f;
            IsCompleted = false;
        }
    }

    [global::System.Serializable]
    public class GameSaveData
    {
        public int TotalCoins = 0;
        public int HighestUnlockedStage = 0;
        public List<StageRecord> StageRecords = new List<StageRecord>();

        public StageRecord GetOrCreateRecord(int stageIndex)
        {
            for (int i = 0; i < StageRecords.Count; i++)
            {
                if (StageRecords[i].StageIndex == stageIndex)
                {
                    return StageRecords[i];
                }
            }

            StageRecord newRecord = new StageRecord(stageIndex);
            StageRecords.Add(newRecord);
            return newRecord;
        }
    }

    public class SaveLoadSystem : MonoBehaviour
    {
        private const string SaveFileName = "savegame.json";
        private const string PlayerPrefsKey = "GIMERSIA_SAVE_DATA";

        [Header("Persistence (SYS-20)")]
        [Tooltip("Otomatis simpan data saat OnApplicationQuit atau OnApplicationPause.")]
        [SerializeField] private bool _autoSaveOnQuit = true;

        private GameSaveData _currentData;
        private string _saveFilePath;

        public static SaveLoadSystem Instance { get; private set; }

        public event Action<GameSaveData> OnDataLoaded;
        public event Action<GameSaveData> OnDataSaved;

        public GameSaveData Data => _currentData;
        public int TotalCoins => _currentData != null ? _currentData.TotalCoins : 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
            LoadGame();
        }

        private void OnApplicationQuit()
        {
            if (_autoSaveOnQuit)
            {
                SaveGame();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _autoSaveOnQuit)
            {
                SaveGame();
            }
        }

        public void SaveGame()
        {
            if (_currentData == null) _currentData = new GameSaveData();

            try
            {
                string json = JsonUtility.ToJson(_currentData, true);

                // 1. Simpan ke file JSON fisik
                File.WriteAllText(_saveFilePath, json);

                // 2. Simpan backup ke PlayerPrefs agar terbaca di PlayerPrefs Editor
                PlayerPrefs.SetString(PlayerPrefsKey, json);
                PlayerPrefs.Save();

                OnDataSaved?.Invoke(_currentData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveLoadSystem] Gagal menyimpan data: {ex.Message}", this);
            }
        }

        public void LoadGame()
        {
            try
            {
                string json = string.Empty;

                if (File.Exists(_saveFilePath))
                {
                    json = File.ReadAllText(_saveFilePath);
                }
                else if (PlayerPrefs.HasKey(PlayerPrefsKey))
                {
                    json = PlayerPrefs.GetString(PlayerPrefsKey);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    _currentData = JsonUtility.FromJson<GameSaveData>(json);
                }
                else
                {
                    _currentData = new GameSaveData();
                }

                OnDataLoaded?.Invoke(_currentData);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SaveLoadSystem] Gagal memuat save game, membuat data baru: {ex.Message}", this);
                _currentData = new GameSaveData();
            }
        }

        /// <summary>
        /// Menyimpan hasil kemenangan suatu stage: bintang, sisa waktu terbaik, dan koin.
        /// </summary>
        public void SaveStageRecord(int stageIndex, int starsEarned, float timeLeft, int coinsEarned)
        {
            if (_currentData == null) _currentData = new GameSaveData();

            StageRecord record = _currentData.GetOrCreateRecord(stageIndex);

            // Perbarui bintang jika perolehan baru lebih tinggi
            if (starsEarned > record.StarsEarned)
            {
                record.StarsEarned = Mathf.Clamp(starsEarned, 0, 3);
            }

            // Perbarui rekor waktu jika lebih lama
            if (timeLeft > record.BestTimeLeft)
            {
                record.BestTimeLeft = timeLeft;
            }

            record.IsCompleted = true;
            _currentData.TotalCoins += Mathf.Max(0, coinsEarned);

            // Buka kunci stage berikutnya
            int nextStage = stageIndex + 1;
            if (nextStage > _currentData.HighestUnlockedStage)
            {
                _currentData.HighestUnlockedStage = nextStage;
            }

            SaveGame();
        }

        public StageRecord GetStageRecord(int stageIndex)
        {
            if (_currentData == null) return null;
            return _currentData.GetOrCreateRecord(stageIndex);
        }

        public void ResetSaveData()
        {
            _currentData = new GameSaveData();

            try
            {
                if (File.Exists(_saveFilePath))
                {
                    File.Delete(_saveFilePath);
                }

                PlayerPrefs.DeleteKey(PlayerPrefsKey);
                PlayerPrefs.Save();

                SaveGame();
                Debug.Log("[SaveLoadSystem] Save data berhasil direset ke kondisi awal.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveLoadSystem] Gagal mereset data: {ex.Message}", this);
            }
        }
    }
}
