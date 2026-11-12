using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCarryController))]
public class PlayerInteractionController : MonoBehaviour
{
    private const int BufferSize = 16;

    [SerializeField] private LayerMask _itemMask;
    [SerializeField] private float _reach = 0.9f;
    [SerializeField] private float _radius = 0.7f;
    [SerializeField] private float _height = 0.6f;

    private readonly Collider[] _hits = new Collider[BufferSize];
    private PlayerMovementController _movement;
    private PlayerCarryController _carry;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _carry = GetComponent<PlayerCarryController>();
    }

    private void Update()
    {
        if (!GrabPressed()) return;

        if (_carry.IsCarrying) _carry.TryDrop();
        else GrabNearest();
    }

    private void GrabNearest()
    {
        Vector3 center = transform.position + Vector3.up * _height + transform.forward * _reach;
        int count = Physics.OverlapSphereNonAlloc(center, _radius, _hits, _itemMask,
            QueryTriggerInteraction.Ignore);

        Rigidbody nearest = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Rigidbody item = _hits[i].attachedRigidbody;
            if (item == null || item.isKinematic) continue;

            float distance = (item.position - transform.position).sqrMagnitude;
            if (distance >= bestDistance) continue;

            bestDistance = distance;
            nearest = item;
        }

        if (nearest != null) _carry.TryPickUp(nearest);
    }

    private bool GrabPressed()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            bool pressed = _movement.IsHusband
                ? kb.eKey.wasPressedThisFrame
                : kb.numpad1Key.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame;

            if (pressed) return true;
        }

        int index = _movement.PlayerIndex;
        return Gamepad.all.Count > index && Gamepad.all[index].buttonSouth.wasPressedThisFrame;
    }
}