using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerStaminaController))]
public class PlayerCarryController : MonoBehaviour
{
    private enum Weight
    {
        Light,
        Medium,
        Heavy
    }

    private const int RingSteps = 8;
    private const float DefaultRadius = 0.3f;

    [Header("Sockets")]
    [SerializeField] private Transform _handSocket;
    [SerializeField] private Transform _carryPivot;

    [Header("Weight Tags")]
    [SerializeField] private string _mediumTag = "Medium";
    [SerializeField] private string _heavyTag = "Heavy";

    [Header("Speed")]
    [SerializeField, Range(0f, 1f)] private float _wifeMediumMultiplier = 0.7f;
    [SerializeField, Range(0f, 1f)] private float _husbandHeavyMultiplier = 0.45f;

    [Header("Safe Drop")]
    [SerializeField] private string _carriedLayerName = "CarriedItem";
    [SerializeField] private LayerMask _blockMask = ~0;
    [SerializeField] private float _castHeight = 1f;
    [SerializeField] private float _dropDistance = 1.2f;
    [SerializeField] private float _fallbackRadius = 0.6f;

    private PlayerMovementController _movement;
    private PlayerStaminaController _stamina;
    private Rigidbody _current;
    private Transform[] _parts;
    private int[] _originalLayers;
    private float _currentRadius;
    private int _carriedLayer;

    public event Action<Rigidbody> HelpNeeded;

    public Rigidbody Current => _current;
    public bool IsCarrying => _current != null;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _stamina = GetComponent<PlayerStaminaController>();
        _carriedLayer = LayerMask.NameToLayer(_carriedLayerName);

        if (_carriedLayer < 0)
        {
            Debug.LogError($"Layer '{_carriedLayerName}' belum dibuat.", this);
            enabled = false;
            return;
        }

        Physics.IgnoreLayerCollision(_carriedLayer, gameObject.layer);
        _blockMask = _blockMask.value & ~((1 << _carriedLayer) | (1 << gameObject.layer));
    }

    public bool TryPickUp(Rigidbody item)
    {
        if (!enabled || IsCarrying || item == null || item.isKinematic) return false;

        Weight weight = GetWeight(item);

        if (weight == Weight.Heavy && !_movement.IsHusband)
        {
            HelpNeeded?.Invoke(item);
            return false;
        }

        Transform socket = weight == Weight.Light ? _handSocket : _carryPivot;
        if (socket == null)
        {
            Debug.LogWarning("Socket belum diisi di Inspector.", this);
            return false;
        }

        Attach(item, socket);
        ApplyWeightRules(weight);

        return true;
    }

    public bool TryDrop()
    {
        if (!IsCarrying) return false;

        Vector3 point = FindSafePoint(_currentRadius);
        Quaternion upright = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        Rigidbody item = Detach();

        item.transform.SetPositionAndRotation(point, upright);

        return true;
    }

    public Rigidbody Detach()
    {
        Rigidbody item = _current;
        if (item == null) return null;

        _current = null;
        item.transform.SetParent(null, true);
        item.isKinematic = false;

        for (int i = 0; i < _parts.Length; i++)
        {
            _parts[i].gameObject.layer = _originalLayers[i];
        }

        _movement.SpeedMultiplier = 1f;
        _stamina.DashBlocked = false;

        return item;
    }

    private void Attach(Rigidbody item, Transform socket)
    {
        _current = item;
        _parts = item.GetComponentsInChildren<Transform>(true);
        _originalLayers = new int[_parts.Length];
        _currentRadius = CalculateRadius(item);

        item.linearVelocity = Vector3.zero;
        item.angularVelocity = Vector3.zero;
        item.isKinematic = true;

        for (int i = 0; i < _parts.Length; i++)
        {
            _originalLayers[i] = _parts[i].gameObject.layer;
            _parts[i].gameObject.layer = _carriedLayer;
        }

        item.transform.SetParent(socket, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    private Weight GetWeight(Rigidbody item)
    {
        if (item.CompareTag(_heavyTag)) return Weight.Heavy;
        if (item.CompareTag(_mediumTag)) return Weight.Medium;

        return Weight.Light;
    }

    private void ApplyWeightRules(Weight weight)
    {
        float multiplier = 1f;

        if (weight == Weight.Medium && !_movement.IsHusband)
        {
            multiplier = _wifeMediumMultiplier;
        }
        else if (weight == Weight.Heavy)
        {
            multiplier = _husbandHeavyMultiplier;
        }

        _movement.SpeedMultiplier = multiplier;
        _stamina.DashBlocked = weight == Weight.Heavy;
    }

    private float CalculateRadius(Rigidbody item)
    {
        Collider[] colliders = item.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0) return DefaultRadius;

        Bounds bounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
        {
            bounds.Encapsulate(colliders[i].bounds);
        }

        return Mathf.Max(bounds.extents.x, bounds.extents.z);
    }

    private Vector3 FindSafePoint(float radius)
    {
        Vector3 origin = transform.position + Vector3.up * _castHeight;
        Vector3 direction = (transform.forward + Vector3.down * 0.5f).normalized;
        Vector3 candidate = origin + direction * _dropDistance;

        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit,
                _dropDistance, _blockMask, QueryTriggerInteraction.Ignore))
        {
            candidate = origin + direction * hit.distance;
        }

        if (IsFree(candidate, radius)) return candidate;

        for (int i = 0; i < RingSteps; i++)
        {
            Vector3 ring = Quaternion.Euler(0f, i * 360f / RingSteps, 0f) * transform.forward;
            Vector3 spot = origin + ring * _fallbackRadius;

            if (IsFree(spot, radius)) return spot;
        }

        return origin;
    }

    private bool IsFree(Vector3 point, float radius)
    {
        return !Physics.CheckSphere(point, radius * 0.95f, _blockMask, QueryTriggerInteraction.Ignore);
    }
}