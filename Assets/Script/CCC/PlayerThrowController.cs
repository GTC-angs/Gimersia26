using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCarryController))]
public class PlayerThrowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _throwOrigin;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _catchClip;

    [Header("Throw")]
    [SerializeField] private float _husbandMaxSpeed = 14f;
    [SerializeField] private float _wifeMaxSpeed = 6f;
    [SerializeField, Range(0f, 1f)] private float _minSpeedRatio = 0.25f;
    [SerializeField] private float _chargeSeconds = 0.8f;
    [SerializeField, Range(5f, 70f)] private float _launchAngle = 30f;
    [SerializeField] private string _heavyTag = "Heavy";

    [Header("Pass Assist")]
    [SerializeField, Range(0f, 90f)] private float _autoAimAngle = 20f;
    [SerializeField] private float _chestHeight = 1f;
    [SerializeField] private float _catchRadius = 0.8f;
    [SerializeField, Range(-1f, 1f)] private float _catchFacingDot = 0.5f;

    [Header("Preview")]
    [SerializeField] private int _previewSteps = 40;
    [SerializeField] private float _previewStep = 0.05f;
    [SerializeField] private string _carriedLayerName = "CarriedItem";
    [SerializeField] private LayerMask _blockMask = ~0;

    [Header("Flight")]
    [SerializeField] private float _ignoreSeconds = 0.3f;
    [SerializeField] private float _maxFlightSeconds = 5f;

    private PlayerMovementController _movement;
    private PlayerCarryController _carry;
    private PlayerThrowController _partner;
    private Collider[] _ownColliders;
    private Collider[] _flyingColliders;
    private Rigidbody _flying;
    private Vector3[] _points;
    private float _charge;
    private float _flightStart;
    private bool _charging;
    private bool _wasHeld;
    private bool _ignoring;

    private float MaxSpeed => _movement.IsHusband ? _husbandMaxSpeed : _wifeMaxSpeed;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _carry = GetComponent<PlayerCarryController>();
        _ownColliders = GetComponentsInChildren<Collider>();
        _points = new Vector3[_previewSteps + 1];

        int carriedLayer = LayerMask.NameToLayer(_carriedLayerName);
        int mask = _blockMask.value & ~(1 << gameObject.layer);
        if (carriedLayer >= 0) mask &= ~(1 << carriedLayer);
        _blockMask = mask;

        if (_line != null) _line.enabled = false;
    }

    private void Start()
    {
        PlayerThrowController[] all = FindObjectsByType<PlayerThrowController>(FindObjectsSortMode.None);

        foreach (PlayerThrowController other in all)
        {
            if (other != this) _partner = other;
        }
    }

    private void Update()
    {
        UpdateFlight();
        UpdateCharge();
    }

    private void UpdateCharge()
    {
        bool held = IsHeld();
        bool canThrow = _carry.IsCarrying && !_carry.Current.CompareTag(_heavyTag);

        if (!canThrow)
        {
            CancelCharge();
            _wasHeld = held;
            return;
        }

        if (held && !_wasHeld)
        {
            _charging = true;
            _charge = 0f;
        }

        if (_charging && held)
        {
            _charge = Mathf.Min(1f, _charge + Time.deltaTime / _chargeSeconds);
            DrawPreview();
        }
        else if (_charging)
        {
            Throw();
        }

        _wasHeld = held;
    }

    private void CancelCharge()
    {
        _charging = false;
        _charge = 0f;
        if (_line != null) _line.enabled = false;
    }

    private void Throw()
    {
        Vector3 velocity = ComputeVelocity();
        CancelCharge();

        Rigidbody item = _carry.Detach();
        if (item == null) return;

        item.transform.position = _throwOrigin.position;
        item.linearVelocity = velocity;

        BeginFlight(item);
    }

    private Vector3 ComputeVelocity()
    {
        Vector3 aim = transform.forward;
        float speed = Mathf.Lerp(MaxSpeed * _minSpeedRatio, MaxSpeed, _charge);

        if (TryAssistedVelocity(out Vector3 assisted)) return assisted;

        return LaunchDirection(aim) * speed;
    }

    private Vector3 LaunchDirection(Vector3 flatForward)
    {
        flatForward.y = 0f;
        Vector3 right = Vector3.Cross(Vector3.up, flatForward.normalized);

        return Quaternion.AngleAxis(-_launchAngle, right) * flatForward.normalized;
    }

    private bool TryAssistedVelocity(out Vector3 velocity)
    {
        velocity = Vector3.zero;
        if (_partner == null) return false;

        Vector3 origin = _throwOrigin.position;
        Vector3 chest = _partner.transform.position + Vector3.up * _chestHeight;
        Vector3 delta = chest - origin;
        Vector3 flat = new Vector3(delta.x, 0f, delta.z);

        float distance = flat.magnitude;
        if (distance < 0.1f) return false;
        if (Vector3.Angle(transform.forward, flat) > _autoAimAngle) return false;

        float angle = _launchAngle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float denominator = 2f * cos * cos * (distance * Mathf.Tan(angle) - delta.y);
        if (denominator <= 0.001f) return false;

        float speed = Mathf.Sqrt(-Physics.gravity.y * distance * distance / denominator);
        if (speed > MaxSpeed) return false;

        Vector3 direction = flat / distance;
        velocity = direction * (speed * cos) + Vector3.up * (speed * Mathf.Sin(angle));

        return true;
    }

    private void DrawPreview()
    {
        if (_line == null) return;

        Vector3 velocity = ComputeVelocity();
        Vector3 position = _throwOrigin.position;
        int count = 0;

        _points[count++] = position;

        for (int i = 1; i <= _previewSteps; i++)
        {
            velocity += Physics.gravity * _previewStep;
            Vector3 next = position + velocity * _previewStep;

            if (Physics.Linecast(position, next, out RaycastHit hit, _blockMask,
                    QueryTriggerInteraction.Ignore))
            {
                _points[count++] = hit.point;
                break;
            }

            _points[count++] = next;
            position = next;
        }

        _line.enabled = true;
        _line.positionCount = count;
        _line.SetPositions(_points);
    }

    private void BeginFlight(Rigidbody item)
    {
        EndFlight();

        _flying = item;
        _flyingColliders = item.GetComponentsInChildren<Collider>();
        _flightStart = Time.time;

        SetIgnore(true);
    }

    private void UpdateFlight()
    {
        if (_flying == null) return;

        float elapsed = Time.time - _flightStart;

        if (_flying.isKinematic || elapsed > _maxFlightSeconds)
        {
            EndFlight();
            return;
        }

        if (_ignoring && elapsed > _ignoreSeconds) SetIgnore(false);

        TryCatch();
    }

    private void TryCatch()
    {
        if (_partner == null || _partner._carry.IsCarrying) return;

        Vector3 chest = _partner.transform.position + Vector3.up * _chestHeight;
        if ((chest - _flying.position).sqrMagnitude > _catchRadius * _catchRadius) return;

        Vector3 incoming = _flying.position - _partner.transform.position;
        incoming.y = 0f;
        if (incoming.sqrMagnitude < 0.0001f) return;
        if (Vector3.Dot(_partner.transform.forward, incoming.normalized) < _catchFacingDot) return;

        if (!_partner._carry.TryPickUp(_flying)) return;

        _partner.PlayCatch();
        EndFlight();
    }

    private void EndFlight()
    {
        if (_flying != null && _ignoring) SetIgnore(false);

        _ignoring = false;
        _flying = null;
    }

    private void SetIgnore(bool ignore)
    {
        _ignoring = ignore;

        foreach (Collider flying in _flyingColliders)
        {
            if (flying == null) continue;

            foreach (Collider own in _ownColliders)
            {
                Physics.IgnoreCollision(flying, own, ignore);
            }
        }
    }

    private void PlayCatch()
    {
        if (_audio != null && _catchClip != null) _audio.PlayOneShot(_catchClip);
    }

    private bool IsHeld()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            bool pressed = _movement.IsHusband ? kb.spaceKey.isPressed : kb.numpad2Key.isPressed;
            if (pressed) return true;
        }

        int index = _movement.PlayerIndex;
        return Gamepad.all.Count > index && Gamepad.all[index].buttonWest.isPressed;
    }
}