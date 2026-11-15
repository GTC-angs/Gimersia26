using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    private enum PlayerRole
    {
        Husband = 0,
        Wife = 1
    }

    [Header("Keybinds (Keyboard)")]
    [SerializeField] private Key _keyUp = Key.W;
    [SerializeField] private Key _keyDown = Key.S;
    [SerializeField] private Key _keyLeft = Key.A;
    [SerializeField] private Key _keyRight = Key.D;
    [SerializeField] private Key _keyJump = Key.Q;

    [Header("Keybinds (Gamepad)")]
    [SerializeField] private GamepadButton _padJump = GamepadButton.North;

    [SerializeField] private PlayerRole _role = PlayerRole.Husband;
    [SerializeField] private float _turnSpeed = 720f;

    [Header("Husband (P1)")]
    [SerializeField] private float _husbandSpeed = 3.5f;
    [SerializeField] private float _husbandAcceleration = 18f;

    [Header("Wife (P2)")]
    [SerializeField] private float _wifeSpeed = 4.2f;
    [SerializeField] private float _wifeAcceleration = 30f;

    [Header("Abilities")]
    [SerializeField] private bool _canJump = true;

    [Header("Jump (CCC-05)")]
    [SerializeField] private float _jumpHeight = 1.2f;
    [SerializeField] private float _jumpBuffer = 0.1f;
    [SerializeField] private LayerMask _groundMask = ~0;
    [SerializeField] private float _groundCheckDistance = 0.15f;

    private const float GroundRayOffset = 0.1f;
    private const float GroundIgnoreAfterJump = 0.1f;

    private Rigidbody _body;
    private Vector2 _input;
    private Vector3 _burstVelocity;
    private float _burstEndTime;
    private bool _burstFreezeVertical;
    private float _jumpBufferedUntil;
    private float _groundIgnoreUntil;

    public bool IsHusband => _role == PlayerRole.Husband;
    public int PlayerIndex => (int)_role;
    public float SpeedMultiplier { get; set; } = 1f;
    public bool IsGrounded { get; private set; }
    public Vector3 Velocity => _body.linearVelocity;

    // Public supaya CoOpLiftController bisa menghitung multiplier ke 3.8 m/s.
    public float BaseSpeed => IsHusband ? _husbandSpeed : _wifeSpeed;

    private bool IsBursting => Time.time < _burstEndTime;
    private float Acceleration => IsHusband ? _husbandAcceleration : _wifeAcceleration;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _body.isKinematic = false;
        _body.useGravity = true;
        _body.freezeRotation = true;
        _body.interpolation = RigidbodyInterpolation.Interpolate;
        _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Update()
    {
        _input = ReadInput();

        if (_canJump && JumpPressed()) _jumpBufferedUntil = Time.time + _jumpBuffer;
    }

    private void FixedUpdate()
    {
        UpdateGrounded();
        TryJump();
        Move();
        Rotate();
    }

    // freezeVertical = true dipakai air dash: burst datar, gravitasi ditahan selama burst.
    public void ApplyBurst(Vector3 velocity, float duration, bool freezeVertical = false)
    {
        _burstVelocity = velocity;
        _burstEndTime = Time.time + duration;
        _burstFreezeVertical = freezeVertical;
    }

    private void UpdateGrounded()
    {
        if (Time.time < _groundIgnoreUntil)
        {
            IsGrounded = false;
            return;
        }

        // Pivot pemain diasumsikan di kaki. Ray mulai di dalam collider sendiri, jadi tidak mengenainya.
        Vector3 origin = transform.position + Vector3.up * GroundRayOffset;

        IsGrounded = Physics.Raycast(origin, Vector3.down, GroundRayOffset + _groundCheckDistance,
            _groundMask, QueryTriggerInteraction.Ignore);
    }

    private void TryJump()
    {
        if (Time.time > _jumpBufferedUntil || !IsGrounded) return;

        float jumpSpeed = Mathf.Sqrt(2f * -Physics.gravity.y * _jumpHeight);
        Vector3 current = _body.linearVelocity;

        _body.linearVelocity = new Vector3(current.x, jumpSpeed, current.z);
        _jumpBufferedUntil = 0f;
        _groundIgnoreUntil = Time.time + GroundIgnoreAfterJump;
        IsGrounded = false;
    }

    private void Move()
    {
        Vector3 current = _body.linearVelocity;

        if (IsBursting)
        {
            float y = _burstFreezeVertical ? 0f : current.y;
            _body.linearVelocity = new Vector3(_burstVelocity.x, y, _burstVelocity.z);
            return;
        }

        Vector3 target = new Vector3(_input.x, 0f, _input.y) * (BaseSpeed * SpeedMultiplier);
        Vector3 flat = new Vector3(current.x, 0f, current.z);

        flat = Vector3.MoveTowards(flat, target, Acceleration * Time.fixedDeltaTime);
        _body.linearVelocity = new Vector3(flat.x, current.y, flat.z);
    }

    private void Rotate()
    {
        if (_input.sqrMagnitude < 0.01f) return;

        Quaternion goal = Quaternion.LookRotation(new Vector3(_input.x, 0f, _input.y));
        _body.MoveRotation(Quaternion.RotateTowards(_body.rotation, goal, _turnSpeed * Time.fixedDeltaTime));
    }

    private Vector2 ReadInput()
    {
        Vector2 keyboard = ReadKeyboard();
        Vector2 gamepad = ReadGamepad();
        Vector2 result = gamepad.sqrMagnitude > keyboard.sqrMagnitude ? gamepad : keyboard;

        return Vector2.ClampMagnitude(result, 1f);
    }

    private Vector2 ReadGamepad()
    {
        if (Gamepad.all.Count <= PlayerIndex) return Vector2.zero;

        return Gamepad.all[PlayerIndex].leftStick.ReadValue();
    }

    private Vector2 ReadKeyboard()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return Vector2.zero;

        float x = Axis(kb, _keyRight, _keyLeft);
        float y = Axis(kb, _keyUp, _keyDown);

        return new Vector2(x, y);
    }

    private bool JumpPressed()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && _keyJump != Key.None && kb[_keyJump].wasPressedThisFrame) return true;

        return Gamepad.all.Count > PlayerIndex
            && Gamepad.all[PlayerIndex][_padJump].wasPressedThisFrame;
    }

    private static float Axis(Keyboard kb, Key positive, Key negative)
    {
        float p = positive != Key.None && kb[positive].isPressed ? 1f : 0f;
        float n = negative != Key.None && kb[negative].isPressed ? 1f : 0f;
        return p - n;
    }

    [ContextMenu("Apply Default Keybinds")]
    private void ApplyDefaultKeybinds()
    {
        if (IsHusband)
        {
            _keyUp = Key.W; _keyDown = Key.S; _keyLeft = Key.A; _keyRight = Key.D;
            _keyJump = Key.Q;
        }
        else
        {
            _keyUp = Key.UpArrow; _keyDown = Key.DownArrow;
            _keyLeft = Key.LeftArrow; _keyRight = Key.RightArrow;
            _keyJump = Key.Numpad0;
        }
    }
}