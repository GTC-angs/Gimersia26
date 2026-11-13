// Dynamic three-mode camera, vertical split-screen, partner leash, and safe teleport controller.
using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicCameraController : MonoBehaviour
{
    public enum CameraMode { Overview, Shared, Split }

    [Header("Players")]
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;

    [Header("Cameras")]
    [SerializeField] private Camera _sharedCamera;
    [SerializeField] private Camera _player1Camera;
    [SerializeField] private Camera _player2Camera;
    [SerializeField] private bool _createMissingCameras = true;

    [Header("Camera Layout")]
    [SerializeField] private float _yaw = 45f;
    [SerializeField] private float _pitch = 52f;
    [SerializeField] private float _overviewDistance = 22f;
    [SerializeField] private float _overviewFov = 30f;
    [SerializeField] private float _sharedMinDistance = 10f;
    [SerializeField] private float _sharedMaxDistance = 14f;
    [SerializeField] private float _sharedFov = 30f;
    [SerializeField] private float _splitDistance = 8f;
    [SerializeField] private float _splitFov = 30f;
    [SerializeField] private float _overviewSeconds = 3f;
    [SerializeField] private float _transitionSeconds = 0.4f;
    [SerializeField] private float _followSmoothTime = 0.25f;

    [Header("Split Trigger")]
    [SerializeField] private float _splitDistanceThreshold = 9f;
    [SerializeField] private Transform _player1RoomRoot;
    [SerializeField] private Transform _player2RoomRoot;

    [Header("Anti-Straggle")]
    [SerializeField] private float _straggleSeconds = 15f;
    [SerializeField] private float _teleportHoldSeconds = 3f;
    [SerializeField] private float _teleportRadius = 1.5f;
    [SerializeField] private float _teleportProbeRadius = 0.35f;
    [SerializeField] private float _teleportGroundHeight = 2f;
    [SerializeField] private LayerMask _teleportBlockMask = ~0;

    private CameraMode _currentMode = CameraMode.Overview;
    private CameraMode _targetMode = CameraMode.Overview;
    private float _overviewEndTime;
    private float _transitionStartedAt;
    private float _lastTogetherTime;
    private float _player1TeleportHold;
    private float _player2TeleportHold;
    private bool _roomDifferentOverride;
    private bool _hasRoomDifferentOverride;
    private bool _isTransitioning;
    private Vector3 _sharedVelocity;
    private Vector3 _player1Velocity;
    private Vector3 _player2Velocity;

    public CameraMode CurrentMode => _currentMode;
    public CameraMode TargetMode => _targetMode;
    public bool IsRoomDifferent => IsRoomDifferentInternal();
    public bool IsStraggling => IsRoomDifferent || DistanceBetweenPlayers() > _splitDistanceThreshold;
    public bool PartnerCompassVisible => IsStraggling && Time.unscaledTime - _lastTogetherTime >= _straggleSeconds;
    public bool Player1NeedsPartner => PartnerCompassVisible;
    public bool Player2NeedsPartner => PartnerCompassVisible;
    public float Player1TeleportHold => _player1TeleportHold;
    public float Player2TeleportHold => _player2TeleportHold;

    public Vector3 GetPartnerViewportPosition(int playerIndex)
    {
        Camera camera = playerIndex == 0 ? _player1Camera : _player2Camera;
        Transform partner = playerIndex == 0 ? _player2 : _player1;
        if (camera == null || partner == null) return Vector3.zero;
        return camera.WorldToViewportPoint(partner.position);
    }

    public Vector3 GetPartnerWorldDirection(int playerIndex)
    {
        Transform player = playerIndex == 0 ? _player1 : _player2;
        Transform partner = playerIndex == 0 ? _player2 : _player1;
        if (player == null || partner == null) return Vector3.forward;
        Vector3 direction = partner.position - player.position;
        direction.y = 0f;
        return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
    }

    private void Awake()
    {
        EnsureCameras();
        ConfigureCameras();
    }

    private void Start()
    {
        _overviewEndTime = Time.unscaledTime + _overviewSeconds;
        _lastTogetherTime = Time.unscaledTime;
        _currentMode = CameraMode.Overview;
        _targetMode = CameraMode.Overview;
        ApplyCameraVisibility(CameraMode.Overview);
    }

    private void Update()
    {
        if (!HasPlayers()) return;
        UpdateTogetherTimer();
        UpdateTargetMode();
        UpdateTransition();
        UpdateTeleportInput();
    }

    private void LateUpdate()
    {
        if (!HasPlayers()) return;
        if (_currentMode == CameraMode.Overview) ApplyOverviewPose(_sharedCamera);
        else if (_currentMode == CameraMode.Shared) ApplySharedPose(_sharedCamera);
        else
        {
            ApplySplitPose(_player1Camera, _player1);
            ApplySplitPose(_player2Camera, _player2);
        }
    }

    private void EnsureCameras()
    {
        if (_sharedCamera == null) _sharedCamera = GetComponent<Camera>();
        if (!_createMissingCameras || _sharedCamera == null) return;
        if (_player1Camera == null) _player1Camera = CreateCamera("Player1 Split Camera");
        if (_player2Camera == null) _player2Camera = CreateCamera("Player2 Split Camera");
    }

    private Camera CreateCamera(string cameraName)
    {
        GameObject cameraObject = new GameObject(cameraName);
        cameraObject.transform.SetParent(transform, false);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.CopyFrom(_sharedCamera);
        return camera;
    }

    private void ConfigureCameras()
    {
        if (_sharedCamera != null) _sharedCamera.rect = new Rect(0f, 0f, 1f, 1f);
        if (_player1Camera != null) _player1Camera.rect = new Rect(0f, 0f, 0.5f, 1f);
        if (_player2Camera != null) _player2Camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
    }

    private void UpdateTargetMode()
    {
        if (Time.unscaledTime < _overviewEndTime)
        {
            SetTargetMode(CameraMode.Overview);
            return;
        }

        CameraMode desired = IsRoomDifferentInternal() || DistanceBetweenPlayers() > _splitDistanceThreshold
            ? CameraMode.Split : CameraMode.Shared;
        SetTargetMode(desired);
    }

    private void SetTargetMode(CameraMode mode)
    {
        if (_targetMode == mode) return;
        _targetMode = mode;
        _transitionStartedAt = Time.unscaledTime;
        _isTransitioning = true;
    }

    private void UpdateTransition()
    {
        if (!_isTransitioning) return;
        if (Time.unscaledTime - _transitionStartedAt < Mathf.Max(0.01f, _transitionSeconds)) return;
        _currentMode = _targetMode;
        _isTransitioning = false;
        ApplyCameraVisibility(_currentMode);
    }

    private void ApplyCameraVisibility(CameraMode mode)
    {
        if (_sharedCamera != null) _sharedCamera.enabled = mode != CameraMode.Split;
        if (_player1Camera != null) _player1Camera.enabled = mode == CameraMode.Split;
        if (_player2Camera != null) _player2Camera.enabled = mode == CameraMode.Split;
    }

    private void ApplyOverviewPose(Camera camera)
    {
        if (camera == null) return;
        SetPose(camera, (_player1.position + _player2.position) * 0.5f, _overviewDistance, _overviewFov);
    }

    private void ApplySharedPose(Camera camera)
    {
        if (camera == null) return;
        float distance = Mathf.Clamp(DistanceBetweenPlayers() + 6f, _sharedMinDistance, _sharedMaxDistance);
        SetPose(camera, (_player1.position + _player2.position) * 0.5f, distance, _sharedFov);
    }

    private void ApplySplitPose(Camera camera, Transform player)
    {
        if (camera == null || player == null) return;
        SetPose(camera, player.position, _splitDistance, _splitFov);
    }

    private void SetPose(Camera camera, Vector3 target, float distance, float fov)
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 position = target - rotation * Vector3.forward * distance;
        float smoothTime = Mathf.Max(0.01f, _followSmoothTime);
        Vector3 velocity = camera == _sharedCamera ? _sharedVelocity
            : camera == _player1Camera ? _player1Velocity : _player2Velocity;
        Vector3 smoothedPosition = Vector3.SmoothDamp(camera.transform.position, position, ref velocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

        if (camera == _sharedCamera) _sharedVelocity = velocity;
        else if (camera == _player1Camera) _player1Velocity = velocity;
        else _player2Velocity = velocity;

        float rotationT = 1f - Mathf.Exp(-Time.unscaledDeltaTime / smoothTime);
        camera.transform.position = smoothedPosition;
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, rotation, rotationT);
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, rotationT);
    }

    private void UpdateTogetherTimer()
    {
        if (!IsStraggling) _lastTogetherTime = Time.unscaledTime;
    }

    private void UpdateTeleportInput()
    {
        _player1TeleportHold = InteractHeld(0) && IsStraggling
            ? Mathf.Min(_teleportHoldSeconds, _player1TeleportHold + Time.unscaledDeltaTime) : 0f;
        _player2TeleportHold = InteractHeld(1) && IsStraggling
            ? Mathf.Min(_teleportHoldSeconds, _player2TeleportHold + Time.unscaledDeltaTime) : 0f;

        if (_player1TeleportHold >= _teleportHoldSeconds) TryTeleport(_player1, _player2);
        if (_player2TeleportHold >= _teleportHoldSeconds) TryTeleport(_player2, _player1);
    }

    private bool InteractHeld(int playerIndex)
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            bool held = playerIndex == 0
                ? keyboard.eKey.isPressed
                : keyboard.numpad1Key.isPressed || keyboard.numpadEnterKey.isPressed;
            if (held) return true;
        }
        return Gamepad.all.Count > playerIndex && Gamepad.all[playerIndex].buttonSouth.isPressed;
    }

    private void TryTeleport(Transform straggler, Transform partner)
    {
        if (straggler == null || partner == null) return;
        Vector3 direction = partner.position - straggler.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f) direction = partner.forward;
        direction.Normalize();
        Vector3 side = Vector3.Cross(Vector3.up, direction);
        Vector3[] candidates = { partner.position - direction * _teleportRadius, partner.position + side * _teleportRadius, partner.position - side * _teleportRadius };

        foreach (Vector3 candidate in candidates)
        {
            if (!TryFindGround(candidate, out Vector3 safePosition) || !IsTeleportPositionFree(safePosition)) continue;
            Rigidbody body = straggler.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.position = safePosition;
            }
            else straggler.position = safePosition;
            _lastTogetherTime = Time.unscaledTime;
            return;
        }
    }

    private bool TryFindGround(Vector3 candidate, out Vector3 position)
    {
        Vector3 origin = candidate + Vector3.up * _teleportGroundHeight;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _teleportGroundHeight * 2f, _teleportBlockMask, QueryTriggerInteraction.Ignore))
        {
            position = hit.point + Vector3.up * 0.05f;
            return true;
        }
        position = candidate;
        return false;
    }

    private bool IsTeleportPositionFree(Vector3 position)
    {
        return !Physics.CheckSphere(position + Vector3.up * _teleportProbeRadius, _teleportProbeRadius, _teleportBlockMask, QueryTriggerInteraction.Ignore);
    }

    private float DistanceBetweenPlayers()
    {
        if (!HasPlayers()) return 0f;
        Vector3 delta = _player1.position - _player2.position;
        delta.y = 0f;
        return delta.magnitude;
    }

    private bool IsRoomDifferentInternal()
    {
        if (_hasRoomDifferentOverride) return _roomDifferentOverride;
        if (_player1RoomRoot == null || _player2RoomRoot == null) return false;
        return _player1RoomRoot != _player2RoomRoot;
    }

    private bool HasPlayers() => _player1 != null && _player2 != null;

    public void SetRoomDifferent(bool roomDifferent)
    {
        _roomDifferentOverride = roomDifferent;
        _hasRoomDifferentOverride = true;
    }

    public void ClearRoomDifferentOverride() => _hasRoomDifferentOverride = false;
}
