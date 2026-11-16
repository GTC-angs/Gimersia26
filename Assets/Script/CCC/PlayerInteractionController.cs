// PlayerInteractionController: menangani interaksi grab item dengan mekanisme hold button,
// release untuk drop, koordinasi co-op lift, serta Smart Target Selection (SYS-11).
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

    [Header("Smart Target Selection (SYS-11)")]
    [Tooltip("Tag penanda alat pembersih (Sapu, Pel, Kemoceng) - Prioritas Tertinggi (Tier 3).")]
    [SerializeField] private string _toolTag = "Tool";

    [Tooltip("Tag penanda furnitur berat (Kulkas, Sofa, dll.) - Prioritas Terendah (Tier 1).")]
    [SerializeField] private string _heavyTag = "Heavy";

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
        bool isHeld = IsGrabHeld();

        // 1. Sedang membawa barang solo -> jika tombol dilepas, drop barang.
        if (_carry.IsCarrying)
        {
            if (!isHeld)
            {
                _carry.TryDrop();
            }
            return;
        }

        // 2. Sedang co-op lift -> jika tombol dilepas, lepaskan lift.
        if (CurrentLift != null)
        {
            if (!isHeld)
            {
                CurrentLift.TryInteract(_movement);
            }
            return;
        }

        // 3. Belum membawa apa pun -> jika tombol ditahan, cari dan ambil objek terdekat dengan seleksi cerdas.
        if (isHeld)
        {
            GrabNearest();
        }
    }

    private void GrabNearest()
    {
        Vector3 center = transform.position + Vector3.up * _height + transform.forward * _reach;
        int count = Physics.OverlapSphereNonAlloc(center, _radius, _hits, _itemMask,
            QueryTriggerInteraction.Ignore);

        Rigidbody bestItem = null;
        int bestPriority = -1;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Rigidbody item = _hits[i].attachedRigidbody;
            if (item == null || item.isKinematic) continue;

            // Evaluasi bobot prioritas (SYS-11):
            // Tier 3 = Alat pembersih (Cleaning Tools)
            // Tier 2 = Barang ringan / sedang (Light / Medium)
            // Tier 1 = Furnitur berat (Heavy)
            int priority = GetItemPriority(item);
            float distance = (item.position - transform.position).sqrMagnitude;

            // Prioritas lebih tinggi selalu menang; jika prioritas sama, yang lebih dekat yang dipilih.
            if (priority > bestPriority || (priority == bestPriority && distance < bestDistance))
            {
                bestPriority = priority;
                bestDistance = distance;
                bestItem = item;
            }
        }

        if (bestItem == null)
        {
            TryHelpPartnerCarry();
            return;
        }

        CoOpLiftController lift = bestItem.GetComponent<CoOpLiftController>();

        // Furnitur co-op: istri memulai menunggu, atau pemain kedua bergabung.
        if (lift != null && (lift.IsWaiting || !_movement.IsHusband))
        {
            if (lift.TryInteract(_movement)) return;
            if (lift.IsWaiting) return; // jangan solo-pickup saat partner sedang menunggu
        }

        _carry.TryPickUp(bestItem);
    }

    private int GetItemPriority(Rigidbody item)
    {
        // 1. Cek Alat Pembersih (Tier 3)
        if (item.CompareTag(_toolTag) || item.name.IndexOf("mop", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            item.name.IndexOf("broom", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            item.name.IndexOf("duster", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            item.name.IndexOf("clean", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return 3;
        }

        // 2. Cek Furnitur Berat (Tier 1)
        if (item.CompareTag(_heavyTag) || item.GetComponent<CoOpLiftController>() != null)
        {
            return 1;
        }

        // 3. Default: Barang Ringan & Sedang (Tier 2)
        return 2;
    }

    // Partner solo carry furnitur co-op -> tekan/tahan Interact di dekatnya untuk membantu.
    private void TryHelpPartnerCarry()
    {
        if (_partner == null) return;

        Rigidbody carried = _partner._carry.Current;
        if (carried == null) return;

        CoOpLiftController lift = carried.GetComponent<CoOpLiftController>();
        if (lift != null) lift.TryInteract(_movement);
    }

    private bool IsGrabHeld()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            bool held = _movement.IsHusband
                ? kb.eKey.isPressed
                : kb.numpad1Key.isPressed || kb.numpadEnterKey.isPressed;
            if (held) return true;
        }

        int index = _movement.PlayerIndex;
        return Gamepad.all.Count > index && Gamepad.all[index].buttonSouth.isPressed;
    }
}

