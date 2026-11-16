// LevelProgressionManager: Mengatur alur tahapan level (Level 0 sampai Level 3),
// navigasi scene, restart stage, serta status unlock stage (SYS-19).
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gimersia.System
{
    [global::System.Serializable]
    public class StageData
    {
        public int StageIndex;
        public string StageName;
        public string SceneName;
        public bool IsUnlocked;

        public StageData(int index, string name, string sceneName, bool isUnlocked)
        {
            StageIndex = index;
            StageName = name;
            SceneName = sceneName;
            IsUnlocked = isUnlocked;
        }
    }

    public class LevelProgressionManager : MonoBehaviour
    {
        [Header("Progression Setup (SYS-19)")]
        [SerializeField] private int _currentStageIndex = 0;

        [SerializeField] private List<StageData> _stages = new List<StageData>
        {
            new StageData(0, "Level 0: Dojo Martial Arts", "testing_cleaning", true),
            new StageData(1, "Level 1: Mudik Lebaran", "testing_placement_lvl1", false),
            new StageData(2, "Level 2: Relokasi Antar Ruang", "testing_placement_lvl1", false),
            new StageData(3, "Level 3: Grand Chaos", "testing_placement_lvl1", false)
        };

        public static LevelProgressionManager Instance { get; private set; }

        public event Action<int> OnLevelStarted;
        public event Action<int, int> OnLevelCompleted; // stageIndex, stars

        public int CurrentStageIndex => _currentStageIndex;
        public int TotalStages => _stages.Count;

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
        }

        private void Start()
        {
            SyncWithCurrentScene();
        }

        public void LoadStage(int stageIndex)
        {
            if (stageIndex < 0 || stageIndex >= _stages.Count)
            {
                Debug.LogWarning($"[LevelProgressionManager] Stage index {stageIndex} di luar batas.", this);
                return;
            }

            _currentStageIndex = stageIndex;
            string targetScene = _stages[stageIndex].SceneName;

            OnLevelStarted?.Invoke(_currentStageIndex);
            SceneManager.LoadScene(targetScene);
        }

        public void LoadNextLevel()
        {
            int nextIndex = _currentStageIndex + 1;
            if (nextIndex < _stages.Count)
            {
                // Buka kunci level berikutnya
                _stages[nextIndex].IsUnlocked = true;
                LoadStage(nextIndex);
            }
            else
            {
                Debug.Log("[LevelProgressionManager] Seluruh stage telah selesai! Kembali ke Menu Utama.");
                LoadMainMenu();
            }
        }

        public void RestartCurrentLevel()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            OnLevelStarted?.Invoke(_currentStageIndex);
            SceneManager.LoadScene(activeScene.name);
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void CompleteLevel(int starsEarned)
        {
            OnLevelCompleted?.Invoke(_currentStageIndex, starsEarned);

            // Buka kunci stage berikutnya
            int nextIndex = _currentStageIndex + 1;
            if (nextIndex < _stages.Count)
            {
                _stages[nextIndex].IsUnlocked = true;
            }
        }

        public bool IsStageUnlocked(int stageIndex)
        {
            if (stageIndex >= 0 && stageIndex < _stages.Count)
            {
                return _stages[stageIndex].IsUnlocked;
            }
            return false;
        }

        public StageData GetStageInfo(int stageIndex)
        {
            if (stageIndex >= 0 && stageIndex < _stages.Count)
            {
                return _stages[stageIndex];
            }
            return null;
        }

        private void SyncWithCurrentScene()
        {
            string currentName = SceneManager.GetActiveScene().name;
            for (int i = 0; i < _stages.Count; i++)
            {
                if (_stages[i].SceneName.Equals(currentName, StringComparison.OrdinalIgnoreCase))
                {
                    _currentStageIndex = i;
                    break;
                }
            }
        }
    }
}
