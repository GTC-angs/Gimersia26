// ItemOutOfBoundsRecoverer: Mendeteksi objek yang jatuh ke luar batas peta (khususnya Y < -10 atau trigger OOB),
// mereset kecepatannya, me-respawn objek ke titik awal/ruangan asal, dan memunculkan efek asap kartun (SYS-09 & SYS-10).
using System.Collections.Generic;
using UnityEngine;

namespace Gimersia.System
{
    public class ItemOutOfBoundsRecoverer : MonoBehaviour
    {
        [global::System.Serializable]
        public struct SpawnPose
        {
            public Vector3 Position;
            public Quaternion Rotation;

            public SpawnPose(Vector3 pos, Quaternion rot)
            {
                Position = pos;
                Rotation = rot;
            }
        }

        [Header("Detection Settings (SYS-09)")]
        [Tooltip("Batas ketinggian Y minimum sebelum objek dianggap jatuh keluar peta.")]
        [SerializeField] private float _deathYThreshold = -10f;

        [Tooltip("Layer mask untuk memfilter objek Rigidbody yang diperiksa.")]
        [SerializeField] private LayerMask _recoverableLayerMask = ~0;

        [Header("Recovery & VFX Settings (SYS-10)")]
        [Tooltip("Prefab efek partikel asap kartun yang dimunculkan saat objek respawn.")]
        [SerializeField] private GameObject _smokeVfxPrefab;

        [Tooltip("Titik respawn fallback jika posisi awal tidak tercatat (opsional).")]
        [SerializeField] private Transform _defaultRespawnPoint;

        [Header("Auto Tracking")]
        [Tooltip("Otomatis mencari dan merekam seluruh Rigidbody di scene saat Start.")]
        [SerializeField] private bool _autoTrackAllRigidbodies = true;

        [Tooltip("Interval pengecekan ketinggian Y (dalam detik) untuk optimasi performa.")]
        [SerializeField] private float _heightCheckInterval = 0.25f;

        // Cache posisi & rotasi awal untuk setiap Rigidbody
        private readonly Dictionary<Rigidbody, SpawnPose> _trackedItems = new Dictionary<Rigidbody, SpawnPose>();
        private readonly List<Rigidbody> _pendingRemoval = new List<Rigidbody>();
        private float _lastHeightCheckTime;

        // Singleton instance opsional untuk kemudahan akses via kode
        public static ItemOutOfBoundsRecoverer Instance { get; private set; }

        public event global::System.Action<Rigidbody, Vector3> OnItemRecovered;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning("[ItemOutOfBoundsRecoverer] Duplikat instance terdeteksi, menggunakan instance pertama.", this);
            }
        }

        private void Start()
        {
            if (_autoTrackAllRigidbodies)
            {
                TrackAllSceneRigidbodies();
            }
        }

        private void Update()
        {
            if (Time.time - _lastHeightCheckTime < _heightCheckInterval) return;
            _lastHeightCheckTime = Time.time;

            CheckHeightBoundaries();
        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb == null) return;

            // Jika collider ini adalah trigger boundary OOB di bawah level
            if (IsRecoverable(rb))
            {
                Recover(rb);
            }
        }

        /// <summary>
        /// Mendaftarkan Rigidbody baru beserta posisi respawn spesifik.
        /// </summary>
        public void RegisterItem(Rigidbody item, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            if (item == null) return;
            _trackedItems[item] = new SpawnPose(spawnPosition, spawnRotation);
        }

        /// <summary>
        /// Mendaftarkan Rigidbody baru menggunakan transform saat ini sebagai posisi respawn.
        /// </summary>
        public void RegisterItem(Rigidbody item)
        {
            if (item == null) return;
            RegisterItem(item, item.position, item.rotation);
        }

        /// <summary>
        /// Memulihkan objek yang jatuh ke luar peta: nolkan kecepatan, pindahkan posisi, dan spawn VFX asap.
        /// </summary>
        public bool Recover(Rigidbody item)
        {
            if (item == null) return false;

            // Tentukan posisi dan rotasi target respawn
            Vector3 targetPos;
            Quaternion targetRot;

            if (_trackedItems.TryGetValue(item, out SpawnPose pose))
            {
                targetPos = pose.Position;
                targetRot = pose.Rotation;
            }
            else if (_defaultRespawnPoint != null)
            {
                targetPos = _defaultRespawnPoint.position;
                targetRot = _defaultRespawnPoint.rotation;
            }
            else
            {
                // Fallback jika tidak ada data: letakkan di ketinggian aman di atas posisi terakhirnya
                targetPos = new Vector3(item.position.x, 1f, item.position.z);
                targetRot = Quaternion.identity;
            }

            // Nolkan kecepatan linier dan angular untuk mencegah objek membal liar (SYS-10)
            item.linearVelocity = Vector3.zero;
            item.angularVelocity = Vector3.zero;

            // Pindahkan transform objek ke target respawn
            item.position = targetPos;
            item.rotation = targetRot;

            // Spawn efek asap kartun (VFX_OOB_RespawnPuff)
            SpawnSmokeVFX(targetPos);

            OnItemRecovered?.Invoke(item, targetPos);
            return true;
        }

        private void CheckHeightBoundaries()
        {
            if (_trackedItems.Count == 0) return;

            _pendingRemoval.Clear();

            foreach (var kvp in _trackedItems)
            {
                Rigidbody rb = kvp.Key;
                if (rb == null)
                {
                    _pendingRemoval.Add(rb);
                    continue;
                }

                // Jangan cek jika item sedang kinematic (misal sedang dipegang player atau terkunci di drop-zone)
                if (rb.isKinematic) continue;

                // SYS-09: Deteksi jika Y < death threshold
                if (rb.position.y < _deathYThreshold)
                {
                    Recover(rb);
                }
            }

            for (int i = 0; i < _pendingRemoval.Count; i++)
            {
                _trackedItems.Remove(_pendingRemoval[i]);
            }
        }

        private void TrackAllSceneRigidbodies()
        {
            Rigidbody[] allBodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
            for (int i = 0; i < allBodies.Length; i++)
            {
                Rigidbody rb = allBodies[i];
                if (rb == null || !IsRecoverable(rb)) continue;

                // Jangan auto-track player
                if (rb.GetComponent<PlayerMovementController>() != null) continue;

                if (!_trackedItems.ContainsKey(rb))
                {
                    _trackedItems[rb] = new SpawnPose(rb.position, rb.rotation);
                }
            }
        }

        private bool IsRecoverable(Rigidbody rb)
        {
            return (_recoverableLayerMask.value & (1 << rb.gameObject.layer)) != 0;
        }

        private void SpawnSmokeVFX(Vector3 position)
        {
            if (_smokeVfxPrefab == null) return;

            GameObject vfxInstance = Instantiate(_smokeVfxPrefab, position, Quaternion.identity);
            // Hancurkan instance VFX otomatis jika bukan particle system looping
            ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(vfxInstance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(vfxInstance, 3f);
            }
        }
    }
}
