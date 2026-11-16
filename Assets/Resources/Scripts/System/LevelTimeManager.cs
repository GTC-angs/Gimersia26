// LevelTimeManager: Mengatur waktu level, countdown, pause, penambahan bonus waktu,
// state kritis (<= 30 detik), dan kondisi timeout saat tamu tiba (SYS-18).
using System;
using UnityEngine;

namespace Gimersia.System
{
    public class LevelTimeManager : MonoBehaviour
    {
        [Header("Timer Settings (SYS-18)")]
        [Tooltip("Durasi total level dalam detik (misal: 180 detik = 3 menit).")]
        [SerializeField] private float _levelDuration = 180f;

        [Tooltip("Ambang batas waktu kritis dalam detik (default: 30 detik).")]
        [SerializeField] private float _criticalTimeThreshold = 30f;

        [Tooltip("Otomatis memulai countdown saat level dibuka.")]
        [SerializeField] private bool _autoStart = true;

        private float _remainingTime;
        private bool _isRunning;
        private bool _isPaused;
        private bool _isCriticalAlertTriggered;

        public static LevelTimeManager Instance { get; private set; }

        public event Action<float, float> OnTimeUpdated; // (remainingTime, totalDuration)
        public event Action OnCriticalTimeEntered;
        public event Action OnTimeOut;
        public event Action<bool> OnTimerStateChanged; // isPaused

        public float RemainingTime => _remainingTime;
        public float TotalDuration => _levelDuration;
        public bool IsRunning => _isRunning;
        public bool IsPaused => _isPaused;
        public bool IsCriticalTime => _remainingTime <= _criticalTimeThreshold && _remainingTime > 0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning("[LevelTimeManager] Duplikat instance terdeteksi, menggunakan instance pertama.", this);
            }

            _remainingTime = _levelDuration;
        }

        private void Start()
        {
            if (_autoStart)
            {
                StartTimer();
            }
            NotifyTimeUpdated();
        }

        private void Update()
        {
            if (!_isRunning || _isPaused) return;

            if (_remainingTime > 0f)
            {
                _remainingTime = Mathf.Max(0f, _remainingTime - Time.deltaTime);
                NotifyTimeUpdated();

                // Cek state kritis (< 30s)
                if (!_isCriticalAlertTriggered && _remainingTime <= _criticalTimeThreshold)
                {
                    _isCriticalAlertTriggered = true;
                    OnCriticalTimeEntered?.Invoke();
                }

                // Cek kondisi timeout (00:00)
                if (_remainingTime <= 0.001f)
                {
                    _isRunning = false;
                    _remainingTime = 0f;
                    NotifyTimeUpdated();
                    OnTimeOut?.Invoke();
                }
            }
        }

        public void StartTimer()
        {
            _isRunning = true;
            _isPaused = false;
            _isCriticalAlertTriggered = _remainingTime <= _criticalTimeThreshold;
            OnTimerStateChanged?.Invoke(_isPaused);
        }

        public void PauseTimer()
        {
            if (!_isRunning) return;
            _isPaused = true;
            OnTimerStateChanged?.Invoke(_isPaused);
        }

        public void ResumeTimer()
        {
            if (!_isRunning) return;
            _isPaused = false;
            OnTimerStateChanged?.Invoke(_isPaused);
        }

        public void AddBonusTime(float seconds)
        {
            if (seconds <= 0f) return;
            _remainingTime += seconds;

            // Jika waktu kembali di atas threshold kritis, reset flag alert
            if (_remainingTime > _criticalTimeThreshold)
            {
                _isCriticalAlertTriggered = false;
            }

            NotifyTimeUpdated();
        }

        public void StopTimer()
        {
            _isRunning = false;
            _isPaused = false;
            OnTimerStateChanged?.Invoke(false);
        }

        public void SetDuration(float durationInSeconds)
        {
            _levelDuration = Mathf.Max(1f, durationInSeconds);
            _remainingTime = _levelDuration;
            _isCriticalAlertTriggered = false;
            NotifyTimeUpdated();
        }

        private void NotifyTimeUpdated()
        {
            OnTimeUpdated?.Invoke(_remainingTime, _levelDuration);
        }
    }
}
