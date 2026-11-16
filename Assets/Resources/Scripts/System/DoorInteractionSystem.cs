// DoorInteractionSystem: Mengelola pintu ayun (Swing) dan pintu geser (Sliding) untuk Level 2 (SYS-12),
// serta dilengkapi sensor anti-terjepit (Anti-Pinch Rebound) yang membal membuka 30 cm jika ada rintangan (SYS-13).
using System;
using UnityEngine;

namespace Gimersia.System
{
    public enum DoorType
    {
        Swing,   // Berputar pada pivot engsel
        Sliding  // Bergeser translasi ke samping
    }

    public class DoorInteractionSystem : MonoBehaviour
    {
        private const int ObstacleBufferSize = 8;

        [Header("Door Configuration (SYS-12)")]
        [Tooltip("Jenis pergerakan pintu (Swing atau Sliding).")]
        [SerializeField] private DoorType _doorType = DoorType.Swing;

        [Tooltip("Transform daun pintu/engsel yang akan digerakkan.")]
        [SerializeField] private Transform _doorTransform;

        [Tooltip("Kecepatan pergerakan pintu (kecepatan transisi).")]
        [SerializeField] private float _animationSpeed = 4f;

        [Header("Swing Door Settings")]
        [Tooltip("Sumbu rotasi lokal untuk pintu ayun (default: Y / ke atas).")]
        [SerializeField] private Vector3 _swingAxis = Vector3.up;

        [Tooltip("Sudut rotasi terbuka (dalam derajat).")]
        [SerializeField] private float _openAngle = 90f;

        [Header("Sliding Door Settings")]
        [Tooltip("Offset translasi lokal saat pintu geser terbuka penuh.")]
        [SerializeField] private Vector3 _slideOffset = new Vector3(1.2f, 0f, 0f);

        [Header("Auto Close")]
        [Tooltip("Apakah pintu otomatis menutup setelah beberapa saat?")]
        [SerializeField] private bool _autoClose = true;

        [Tooltip("Waktu jeda (detik) sebelum pintu mulai menutup otomatis.")]
        [SerializeField] private float _autoCloseDelay = 2.5f;

        [Header("Anti-Pinch Rebound (SYS-13)")]
        [Tooltip("Aktifkan fitur deteksi anti-terjepit saat pintu menutup.")]
        [SerializeField] private bool _antiPinchEnabled = true;

        [Tooltip("Transform titik pusat sensor celah kusen pintu.")]
        [SerializeField] private Transform _pinchSensorPoint;

        [Tooltip("Ukuran setengah kotak deteksi sensor (Half Extents).")]
        [SerializeField] private Vector3 _sensorHalfExtents = new Vector3(0.5f, 1f, 0.3f);

        [Tooltip("Layer rintangan yang dicek (Pemain, Barang Bawaan, Perabot).")]
        [SerializeField] private LayerMask _obstacleMask = ~0;

        [Tooltip("Jarak membal terbuka kembali saat terjepit (default: 0.3 meter / 30 cm).")]
        [SerializeField] private float _reboundDistance = 0.3f;

        [Tooltip("Sudut membal terbuka kembali untuk swing door (derajat).")]
        [SerializeField] private float _reboundAngle = 25f;

        private Vector3 _closedPosition;
        private Quaternion _closedRotation;
        private Vector3 _openedPosition;
        private Quaternion _openedRotation;

        private float _currentProgress; // 0 = Tertutup penuh, 1 = Terbuka penuh
        private float _targetProgress;
        private float _closeTimer;
        private bool _isRebounding;

        private readonly Collider[] _obstacleHits = new Collider[ObstacleBufferSize];

        public event Action<bool> OnDoorStateChanged;
        public event Action OnAntiPinchTriggered;

        public bool IsOpen => _currentProgress > 0.05f;
        public bool IsFullyOpen => Mathf.Approximately(_currentProgress, 1f);
        public bool IsFullyClosed => Mathf.Approximately(_currentProgress, 0f);
        public bool IsClosing => _targetProgress < _currentProgress;

        private void Awake()
        {
            if (_doorTransform == null)
            {
                _doorTransform = transform;
            }

            // Catat pose awal sebagai pose tertutup
            _closedPosition = _doorTransform.localPosition;
            _closedRotation = _doorTransform.localRotation;

            // Hitung pose target saat terbuka
            _openedPosition = _closedPosition + _slideOffset;
            _openedRotation = _closedRotation * Quaternion.Euler(_swingAxis * _openAngle);

            if (_pinchSensorPoint == null)
            {
                _pinchSensorPoint = transform;
            }
        }

        private void Update()
        {
            HandleMovement();
            HandleAutoClose();
            HandleAntiPinch();
        }

        private void HandleMovement()
        {
            if (Mathf.Approximately(_currentProgress, _targetProgress)) return;

            _currentProgress = Mathf.MoveTowards(_currentProgress, _targetProgress, _animationSpeed * Time.deltaTime);

            if (_doorType == DoorType.Swing)
            {
                _doorTransform.localRotation = Quaternion.Slerp(_closedRotation, _openedRotation, _currentProgress);
            }
            else
            {
                _doorTransform.localPosition = Vector3.Lerp(_closedPosition, _openedPosition, _currentProgress);
            }
        }

        private void HandleAutoClose()
        {
            if (!_autoClose || _targetProgress < 1f || _isRebounding) return;

            _closeTimer += Time.deltaTime;
            if (_closeTimer >= _autoCloseDelay)
            {
                Close();
            }
        }

        private void HandleAntiPinch()
        {
            // Hanya periksa anti-pinch saat pintu sedang dalam proses bergerak menutup
            if (!_antiPinchEnabled || !IsClosing) return;

            int count = Physics.OverlapBoxNonAlloc(
                _pinchSensorPoint.position,
                _sensorHalfExtents,
                _obstacleHits,
                _pinchSensorPoint.rotation,
                _obstacleMask,
                QueryTriggerInteraction.Ignore
            );

            bool obstacleFound = false;
            for (int i = 0; i < count; i++)
            {
                Collider col = _obstacleHits[i];
                if (col == null || col.transform.IsChildOf(transform)) continue;

                // Abaikan static level collider
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null || col.CompareTag("Player"))
                {
                    obstacleFound = true;
                    break;
                }
            }

            if (obstacleFound)
            {
                TriggerAntiPinchRebound();
            }
        }

        /// <summary>
        /// Protokol Anti-Pinch Rebound (SYS-13): Membal membuka minimal 30 cm agar tidak menjepit karakter/perabot.
        /// </summary>
        public void TriggerAntiPinchRebound()
        {
            _isRebounding = true;

            // Hitung persentase rebound setara 30 cm atau sudut proporsional
            float reboundRatio = 0.35f;
            if (_doorType == DoorType.Sliding && _slideOffset.magnitude > 0.01f)
            {
                reboundRatio = Mathf.Clamp01(_reboundDistance / _slideOffset.magnitude);
            }
            else if (_doorType == DoorType.Swing && Mathf.Abs(_openAngle) > 0.01f)
            {
                reboundRatio = Mathf.Clamp01(_reboundAngle / Mathf.Abs(_openAngle));
            }

            // Buka kembali sebesar persentase rebound di atas posisi saat ini
            _targetProgress = Mathf.Clamp01(_currentProgress + reboundRatio);
            _closeTimer = 0f; // Reset timer auto-close

            OnAntiPinchTriggered?.Invoke();

            // Selesai rebounding setelah kembali membuka
            Invoke(nameof(ResetReboundState), 0.5f);
        }

        private void ResetReboundState()
        {
            _isRebounding = false;
        }

        public void Open()
        {
            _targetProgress = 1f;
            _closeTimer = 0f;
            _isRebounding = false;
            OnDoorStateChanged?.Invoke(true);
        }

        public void Close()
        {
            _targetProgress = 0f;
            _closeTimer = 0f;
            OnDoorStateChanged?.Invoke(false);
        }

        public void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }

        private void OnDrawGizmosSelected()
        {
            // Visualisasi kotak sensor anti-terjepit di Scene view
            if (_pinchSensorPoint != null)
            {
                Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.35f);
                Gizmos.matrix = Matrix4x4.TRS(_pinchSensorPoint.position, _pinchSensorPoint.rotation, Vector3.one);
                Gizmos.DrawCube(Vector3.zero, _sensorHalfExtents * 2f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(Vector3.zero, _sensorHalfExtents * 2f);
            }
        }
    }
}
