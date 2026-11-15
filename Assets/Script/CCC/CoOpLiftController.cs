using System;
using UnityEngine;

// Pasang di furnitur Heavy (Rigidbody + tag "Heavy").
// Input TIDAK dibaca di sini. PlayerInteractionController memanggil TryInteract().
//
// Alur:
//  - Pemain A (biasanya istri) interact di dekat socket   -> Waiting
//  - Pemain B interact di dekat socket lainnya            -> Lifting
//  - Atau: suami solo carry, istri interact di dekatnya   -> Lifting
//  - Siapa pun interact saat Lifting, atau jarak > max    -> diletakkan
[RequireComponent(typeof(Rigidbody))]
public class CoOpLiftController : MonoBehaviour
{
    private enum State { Idle, Waiting, Lifting }

    private class Lifter
    {
        public PlayerMovementController Movement;
        public PlayerStaminaController Stamina;
        public PlayerCarryController Carry;
        public PlayerInteractionController Interaction;
        public Collider[] Colliders;
        public Transform Transform;
    }

    private const int DoorBufferSize = 8;

    [Header("Sockets (CCC-23)")]
    [SerializeField] private Transform _leftSocket;
    [SerializeField] private Transform _rightSocket;
    [SerializeField] private float _socketRadius = 1.2f;
    [SerializeField] private float _waitTimeout = 5f;

    [Header("Lift")]
    [SerializeField] private float _carryHeight = 0.8f;
    [SerializeField] private float _followSharpness = 18f;
    [SerializeField] private float _rotateSpeed = 360f;
    [SerializeField] private float _maxSeparation = 4f;
    [SerializeField] private float _restoreCollisionDelay = 0.5f;

    [Header("Co-op Movement (CCC-24)")]
    [SerializeField] private float _coopSpeed = 3.8f;

    [Header("Doorway Assist (CCC-25)")]
    [SerializeField] private LayerMask _doorMask;
    [SerializeField] private float _doorRadius = 1.5f;

    private readonly Lifter[] _lifters = new Lifter[2];
    private readonly Collider[] _doorBuffer = new Collider[DoorBufferSize];
    private Collider[] _itemColliders;
    private Rigidbody _body;
    private State _state = State.Idle;
    private int _waitingIndex;
    private int _waitingSocket;
    private int _leftIndex;
    private int _rightIndex;
    private float _waitStart;
    private float _yaw;
    private float _yawOffset;
    private float _restoreAt;
    private bool _restorePending;
    private bool _ready;

    public event Action<bool> LiftStateChanged;

    public bool IsWaiting => _state == State.Waiting;
    public bool IsCoOpCarried => _state == State.Lifting;
    public bool IsBusy => _state != State.Idle;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _itemColliders = GetComponentsInChildren<Collider>();

        if (_leftSocket == null || _rightSocket == null)
        {
            Debug.LogWarning("Left/Right socket belum diisi di Inspector.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        PlayerMovementController[] players =
            FindObjectsByType<PlayerMovementController>(FindObjectsSortMode.None);

        int count = Mathf.Min(players.Length, _lifters.Length);

        for (int i = 0; i < count; i++)
        {
            _lifters[i] = new Lifter
            {
                Movement = players[i],
                Stamina = players[i].GetComponent<PlayerStaminaController>(),
                Carry = players[i].GetComponent<PlayerCarryController>(),
                Interaction = players[i].GetComponent<PlayerInteractionController>(),
                Colliders = players[i].GetComponentsInChildren<Collider>(),
                Transform = players[i].transform
            };
        }

        _ready = count == _lifters.Length;
        if (!_ready) Debug.LogWarning("Co-op lift butuh 2 pemain di scene.", this);
    }

    private void Update()
    {
        if (!_ready) return;

        RestoreCollisionIfDue();

        if (_state == State.Waiting) UpdateWaiting();
        else if (_state == State.Lifting) UpdateLifting();
    }

    private void FixedUpdate()
    {
        if (_state != State.Lifting) return;

        Transform left = _lifters[_leftIndex].Transform;
        Transform right = _lifters[_rightIndex].Transform;

        Vector3 mid = (left.position + right.position) * 0.5f;
        float baseYaw = PlayersYaw(left, right) + _yawOffset;
        float targetYaw = ApplyDoorAssist(baseYaw);

        _yaw = Mathf.MoveTowardsAngle(_yaw, targetYaw, _rotateSpeed * Time.fixedDeltaTime);

        Vector3 target = new Vector3(mid.x, mid.y + _carryHeight, mid.z);
        float t = 1f - Mathf.Exp(-_followSharpness * Time.fixedDeltaTime);

        _body.MovePosition(Vector3.Lerp(_body.position, target, t));
        _body.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));
    }

    // ---------- Public API (dipanggil PlayerInteractionController) ----------

    // Return true jika tekanan Interact sudah ditangani oleh lift ini.
    public bool TryInteract(PlayerMovementController player)
    {
        if (!_ready || !enabled) return false;

        int index = IndexOf(player);
        if (index < 0) return false;

        switch (_state)
        {
            case State.Lifting:
                EndLift();
                return true;

            case State.Waiting:
                return InteractWhileWaiting(index);

            default:
                return _body.isKinematic ? TryJoinSoloCarry(index) : TryStartWaiting(index);
        }
    }

    // ---------- State ----------

    private bool TryStartWaiting(int index)
    {
        if (!TryFindSocket(index, -1, out int socket)) return false;

        _waitingIndex = index;
        _waitingSocket = socket;
        _waitStart = Time.time;
        _state = State.Waiting;
        return true;
    }

    private bool InteractWhileWaiting(int index)
    {
        if (index == _waitingIndex)
        {
            _state = State.Idle; // tekan lagi = batal
            return true;
        }

        if (!TryFindSocket(index, _waitingSocket, out _)) return false;

        BeginLift(_waitingSocket == 0 ? _waitingIndex : index);
        return true;
    }

    // Suami solo carry, istri menekan Interact di dekatnya -> jadi co-op lift.
    private bool TryJoinSoloCarry(int helperIndex)
    {
        int carrierIndex = 1 - helperIndex;
        Lifter carrier = _lifters[carrierIndex];
        Lifter helper = _lifters[helperIndex];

        if (carrier.Carry.Current != _body || helper.Carry.IsCarrying) return false;
        if (FlatDistance(carrier.Transform.position, helper.Transform.position) > _socketRadius * 2f) return false;

        carrier.Carry.Detach();
        BeginLift(carrierIndex);
        return true;
    }

    private void UpdateWaiting()
    {
        bool cancel = _body.isKinematic
            || Time.time - _waitStart > _waitTimeout
            || _lifters[_waitingIndex].Carry.IsCarrying
            || !InsideSocket(_waitingIndex, _waitingSocket);

        if (cancel) _state = State.Idle;
    }

    private void UpdateLifting()
    {
        Vector3 gap = _lifters[0].Transform.position - _lifters[1].Transform.position;
        gap.y = 0f;

        if (gap.sqrMagnitude > _maxSeparation * _maxSeparation) EndLift();
    }

    private void BeginLift(int leftIndex)
    {
        _leftIndex = leftIndex;
        _rightIndex = 1 - leftIndex;

        Transform left = _lifters[_leftIndex].Transform;
        Transform right = _lifters[_rightIndex].Transform;

        // Simpan orientasi furnitur relatif terhadap pemain supaya tidak "loncat" saat diangkat.
        _yaw = _body.rotation.eulerAngles.y;
        _yawOffset = Mathf.DeltaAngle(PlayersYaw(left, right), _yaw);

        _body.linearVelocity = Vector3.zero;
        _body.angularVelocity = Vector3.zero;
        _body.isKinematic = true;
        _restorePending = false;

        SetIgnorePlayers(true);

        foreach (Lifter lifter in _lifters)
        {
            // CCC-24: kecepatan co-op sama untuk kedua pemain. CCC-08: dash diblok.
            lifter.Movement.SpeedMultiplier = _coopSpeed / lifter.Movement.BaseSpeed;
            lifter.Stamina.DashBlocked = true;
            if (lifter.Interaction != null) lifter.Interaction.CurrentLift = this;
        }

        _state = State.Lifting;
        LiftStateChanged?.Invoke(true);
    }

    private void EndLift()
    {
        _state = State.Idle;

        _body.isKinematic = false;
        _body.linearVelocity = Vector3.zero;
        _body.angularVelocity = Vector3.zero;

        foreach (Lifter lifter in _lifters)
        {
            lifter.Movement.SpeedMultiplier = 1f;
            lifter.Stamina.DashBlocked = false;
            if (lifter.Interaction != null) lifter.Interaction.CurrentLift = null;
        }

        _restoreAt = Time.time + _restoreCollisionDelay;
        _restorePending = true;

        LiftStateChanged?.Invoke(false);
    }

    private void RestoreCollisionIfDue()
    {
        if (!_restorePending || Time.time < _restoreAt) return;

        _restorePending = false;
        SetIgnorePlayers(false);
    }

    // ---------- Helpers ----------

    private int IndexOf(PlayerMovementController player)
    {
        for (int i = 0; i < _lifters.Length; i++)
        {
            if (_lifters[i] != null && _lifters[i].Movement == player) return i;
        }

        return -1;
    }

    private static float PlayersYaw(Transform left, Transform right)
    {
        Vector3 axis = right.position - left.position;
        axis.y = 0f;
        if (axis.sqrMagnitude < 0.01f) return left.eulerAngles.y;

        // forward = Cross(right, up)
        Vector3 forward = Vector3.Cross(axis.normalized, Vector3.up);
        return Quaternion.LookRotation(forward).eulerAngles.y;
    }

    // CCC-25: soft-align sumbu furnitur ke sumbu pintu saat mendekat (radius default 1.5 m).
    private float ApplyDoorAssist(float baseYaw)
    {
        int count = Physics.OverlapSphereNonAlloc(_body.position, _doorRadius, _doorBuffer,
            _doorMask, QueryTriggerInteraction.Collide);

        Transform door = null;
        float best = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Vector3 closest = _doorBuffer[i].ClosestPoint(_body.position);
            Vector3 delta = closest - _body.position;
            delta.y = 0f;

            if (delta.magnitude >= best) continue;

            best = delta.magnitude;
            door = _doorBuffer[i].transform;
        }

        if (door == null) return baseYaw;

        float weight = Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(best / _doorRadius));
        float doorYaw = door.eulerAngles.y;
        float a = Mathf.DeltaAngle(baseYaw, doorYaw);
        float b = Mathf.DeltaAngle(baseYaw, doorYaw + 180f);
        float delta180 = Mathf.Abs(a) < Mathf.Abs(b) ? a : b;

        return baseYaw + delta180 * weight;
    }

    private bool TryFindSocket(int index, int excludeSocket, out int socket)
    {
        socket = -1;
        if (_lifters[index].Carry.IsCarrying) return false;

        float bestDistance = float.MaxValue;

        for (int s = 0; s < 2; s++)
        {
            if (s == excludeSocket || !InsideSocket(index, s)) continue;

            float d = FlatDistance(_lifters[index].Transform.position, SocketOf(s).position);
            if (d >= bestDistance) continue;

            bestDistance = d;
            socket = s;
        }

        return socket >= 0;
    }

    private bool InsideSocket(int index, int socket)
    {
        return FlatDistance(_lifters[index].Transform.position, SocketOf(socket).position) <= _socketRadius;
    }

    private Transform SocketOf(int socket) => socket == 0 ? _leftSocket : _rightSocket;

    private static float FlatDistance(Vector3 a, Vector3 b)
    {
        Vector3 d = a - b;
        d.y = 0f;
        return d.magnitude;
    }

    private void SetIgnorePlayers(bool ignore)
    {
        foreach (Lifter lifter in _lifters)
        {
            foreach (Collider player in lifter.Colliders)
            {
                foreach (Collider item in _itemColliders)
                {
                    if (player != null && item != null) Physics.IgnoreCollision(item, player, ignore);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (_leftSocket != null) Gizmos.DrawWireSphere(_leftSocket.position, _socketRadius);
        if (_rightSocket != null) Gizmos.DrawWireSphere(_rightSocket.position, _socketRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _doorRadius);
    }
}