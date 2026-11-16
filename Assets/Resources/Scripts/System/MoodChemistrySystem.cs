// MoodChemistrySystem: Mengelola emosi pemain (0-100%), decay waktu, pengurangan karena tabrakan/stamina (SYS-14),
// debuff kecepatan gerak saat Annoyed State < 25% (SYS-15), interaksi Hug (+50% mood & heart VFX) (SYS-16),
// serta interaksi High-Five (+25% mood & sparkle VFX) (SYS-17).
using System;
using System.Collections;
using UnityEngine;

namespace Gimersia.System
{
    public enum MoodState
    {
        Exhausted, // 0 - 19% (Bete / Mogok)
        Annoyed,   // 20 - 39% (Kesal)
        Normal,    // 40 - 74% (Nyaman / Baseline)
        Happy      // 75 - 100% (Semangat)
    }

    [RequireComponent(typeof(PlayerMovementController))]
    public class MoodChemistrySystem : MonoBehaviour
    {
        [Header("Mood Parameters (SYS-14)")]
        [Tooltip("Nilai awal mood karakter (baseline: 60%).")]
        [SerializeField, Range(0f, 100f)] private float _currentMood = 60f;

        [Tooltip("Interval decay alami mood per waktu (detik).")]
        [SerializeField] private float _decayInterval = 8f;

        [Tooltip("Jumlah pengurangan mood alami per interval.")]
        [SerializeField] private float _decayAmount = 1f;

        [Tooltip("Pengurangan mood saat menabrak pasangan / dinding keras.")]
        [SerializeField] private float _collisionMoodPenalty = 2f;

        [Tooltip("Pengurangan mood saat stamina lari terkuras habis.")]
        [SerializeField] private float _exhaustedMoodPenalty = 5f;

        [Header("Annoyed State (SYS-15)")]
        [Tooltip("Ambang batas persentase mood untuk Annoyed State (default: 25%).")]
        [SerializeField] private float _annoyedThreshold = 25f;

        [Tooltip("Pengali kecepatan jalan saat mood di bawah threshold (default: 0.85 = -15%).")]
        [SerializeField] private float _annoyedSpeedMultiplier = 0.85f;

        [Header("Co-op Emotes: Hug & High-Five (SYS-16 & SYS-17)")]
        [Tooltip("Jarak maksimum antar karakter untuk memicu emote berpasangan.")]
        [SerializeField] private float _interactionRange = 2.0f;

        [Tooltip("Durasi kunci aksi pelukan (detik).")]
        [SerializeField] private float _hugDuration = 1.5f;

        [Tooltip("Durasi kunci aksi tos tangan (detik).")]
        [SerializeField] private float _highFiveDuration = 0.7f;

        [Tooltip("Prefab efek partikel hati merah coral untuk aksi Peluk (VFX_Heart_Hug).")]
        [SerializeField] private GameObject _heartHugVfxPrefab;

        [Tooltip("Prefab efek partikel sparkle bintang untuk aksi Tos (VFX_HighFive_Spark).")]
        [SerializeField] private GameObject _highFiveVfxPrefab;

        [Header("Cooldowns")]
        [SerializeField] private float _hugCooldown = 20f;
        [SerializeField] private float _highFiveCooldown = 10f;

        private PlayerMovementController _movement;
        private PlayerStaminaController _stamina;
        private MoodChemistrySystem _partnerMood;

        private float _lastDecayTime;
        private float _lastHugTime = -999f;
        private float _lastHighFiveTime = -999f;
        private bool _isActionLocked;

        public event Action<float, MoodState> OnMoodChanged;
        public event Action OnHugExecuted;
        public event Action OnHighFiveExecuted;

        public float CurrentMood => _currentMood;
        public MoodState State => GetMoodState(_currentMood);
        public bool IsActionLocked => _isActionLocked;

        // SYS-15: Pengali kecepatan jalan dinamis dibaca oleh PlayerMovementController
        public float MoveSpeedMultiplier
        {
            get
            {
                if (_isActionLocked) return 0f;
                return _currentMood < _annoyedThreshold ? _annoyedSpeedMultiplier : 1.0f;
            }
        }

        public bool CanHug => Time.time >= _lastHugTime + _hugCooldown && !_isActionLocked;
        public bool CanHighFive => Time.time >= _lastHighFiveTime + _highFiveCooldown && !_isActionLocked;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovementController>();
            _stamina = GetComponent<PlayerStaminaController>();
        }

        private void Start()
        {
            _lastDecayTime = Time.time;
            FindPartner();

            NotifyMoodChanged();
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
            HandleTimeDecay();
            CheckStaminaExhaustion();
        }

        private bool _wasExhausted;
        private void CheckStaminaExhaustion()
        {
            if (_stamina == null) return;

            bool isExhausted = _stamina.Stamina <= 0.01f;
            if (isExhausted && !_wasExhausted)
            {
                ModifyMood(-_exhaustedMoodPenalty, "Stamina Exhausted");
            }
            _wasExhausted = isExhausted;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Pengurangan mood karena tabrakan keras dengan pasangan atau objek berat
            if (collision.relativeVelocity.magnitude > 3.0f)
            {
                if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Heavy"))
                {
                    ModifyMood(-_collisionMoodPenalty, "Collision Bump");
                }
            }
        }

        public void ModifyMood(float delta, string reason = "")
        {
            float oldMood = _currentMood;
            _currentMood = Mathf.Clamp(_currentMood + delta, 0f, 100f);

            if (!Mathf.Approximately(oldMood, _currentMood))
            {
                NotifyMoodChanged();
            }
        }

        /// <summary>
        /// Memicu aksi Peluk (SYS-16): Kedua pemain berdekatan, durasi 1.5s, mood pulih +50%, muncul hati VFX.
        /// </summary>
        public bool TryExecuteHug()
        {
            if (!CanHug || _partnerMood == null) return false;

            float distance = Vector3.Distance(transform.position, _partnerMood.transform.position);
            if (distance > _interactionRange) return false;

            StartCoroutine(HugRoutine());
            _partnerMood.StartCoroutine(_partnerMood.HugRoutine());
            return true;
        }

        /// <summary>
        /// Memicu aksi Tos Tangan (SYS-17): Kedua pemain berhadapan, durasi 0.7s, mood pulih +25%, muncul sparkle VFX.
        /// </summary>
        public bool TryExecuteHighFive()
        {
            if (!CanHighFive || _partnerMood == null) return false;

            float distance = Vector3.Distance(transform.position, _partnerMood.transform.position);
            if (distance > _interactionRange) return false;

            StartCoroutine(HighFiveRoutine());
            _partnerMood.StartCoroutine(_partnerMood.HighFiveRoutine());
            return true;
        }

        private IEnumerator HugRoutine()
        {
            _isActionLocked = true;
            _lastHugTime = Time.time;

            // Spawn VFX Hati di antara kedua karakter (hanya dipanggil satu kali oleh inisiator)
            if (_movement.IsHusband && _heartHugVfxPrefab != null && _partnerMood != null)
            {
                Vector3 center = (transform.position + _partnerMood.transform.position) * 0.5f + Vector3.up * 1.2f;
                SpawnVfx(_heartHugVfxPrefab, center);
            }

            yield return new WaitForSeconds(_hugDuration);

            // Pulihkan mood +50% (SYS-16)
            ModifyMood(50f, "Hug Interaction");
            _isActionLocked = false;
            OnHugExecuted?.Invoke();
        }

        private IEnumerator HighFiveRoutine()
        {
            _isActionLocked = true;
            _lastHighFiveTime = Time.time;

            // Spawn VFX Sparkle di antara kedua tangan (hanya dipanggil satu kali)
            if (_movement.IsHusband && _highFiveVfxPrefab != null && _partnerMood != null)
            {
                Vector3 center = (transform.position + _partnerMood.transform.position) * 0.5f + Vector3.up * 1.0f;
                SpawnVfx(_highFiveVfxPrefab, center);
            }

            yield return new WaitForSeconds(_highFiveDuration);

            // Pulihkan mood +25% (SYS-17)
            ModifyMood(25f, "High-Five Interaction");
            _isActionLocked = false;
            OnHighFiveExecuted?.Invoke();
        }

        private void HandleTimeDecay()
        {
            if (Time.time - _lastDecayTime >= _decayInterval)
            {
                _lastDecayTime = Time.time;
                ModifyMood(-_decayAmount, "Time Decay");
            }
        }

        private void FindPartner()
        {
            MoodChemistrySystem[] all = FindObjectsByType<MoodChemistrySystem>(FindObjectsSortMode.None);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != this)
                {
                    _partnerMood = all[i];
                    break;
                }
            }
        }

        private MoodState GetMoodState(float mood)
        {
            if (mood >= 75f) return MoodState.Happy;
            if (mood >= 40f) return MoodState.Normal;
            if (mood >= _annoyedThreshold) return MoodState.Annoyed;
            return MoodState.Exhausted;
        }

        private void NotifyMoodChanged()
        {
            OnMoodChanged?.Invoke(_currentMood, State);
        }

        private void SpawnVfx(GameObject prefab, Vector3 position)
        {
            if (prefab == null) return;
            GameObject vfx = Instantiate(prefab, position, Quaternion.identity);
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(vfx, 2.5f);
            }
        }
    }
}
