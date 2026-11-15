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
    private PlayerInteractionController _partner;

    // Diisi oleh CoOpLiftController saat pemain ini sedang co-op lift.
    public CoOpLiftController CurrentLift { get; set; }

    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _carry = GetComponent<PlayerCarryController>();
    }

    private void Start()
    {
        PlayerInteractionController[] all =
            FindObjectsByType<PlayerInteractionController>(FindObjectsSortMode.None);

        foreach (PlayerInteractionController other in all)
        {
            if (other != this) _partner = other;
        }
    }

    private void Update()
    {
        if (!GrabPressed()) return;

        // Sedang co-op lift: tombol Interact = letakkan.
        if (CurrentLift != null)
        {
            CurrentLift.TryInteract(_movement);
            return;
        }

        if (_carry.IsCarrying)
        {
            _carry.TryDrop();
            return;
        }

        GrabNearest();
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

        if (nearest == null)
        {
            TryHelpPartnerCarry();
            return;
        }

        CoOpLiftController lift = nearest.GetComponent<CoOpLiftController>();

        // Furnitur co-op: istri memulai menunggu, atau pemain kedua bergabung.
        if (lift != null && (lift.IsWaiting || !_movement.IsHusband))
        {
            if (lift.TryInteract(_movement)) return;
            if (lift.IsWaiting) return; // jangan solo-pickup saat partner sedang menunggu
        }

        _carry.TryPickUp(nearest);
    }

    // Partner solo carry furnitur co-op -> tekan Interact di dekatnya untuk membantu.
    private void TryHelpPartnerCarry()
    {
        if (_partner == null) return;

        Rigidbody carried = _partner._carry.Current;
        if (carried == null) return;

        CoOpLiftController lift = carried.GetComponent<CoOpLiftController>();
        if (lift != null) lift.TryInteract(_movement);
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