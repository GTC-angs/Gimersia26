using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    private enum PlayerRole
    {
        Husband = 0,
        Wife = 1
    }

    [SerializeField] private PlayerRole _role = PlayerRole.Husband;
    [SerializeField] private float _turnSpeed = 720f;

    [Header("Husband (P1)")]
    [SerializeField] private float _husbandSpeed = 3.5f;
    [SerializeField] private float _husbandAcceleration = 18f;

    [Header("Wife (P2)")]
    [SerializeField] private float _wifeSpeed = 4.2f;
    [SerializeField] private float _wifeAcceleration = 30f;

    private Rigidbody _body;
    private Vector2 _input;
    private Vector3 _burstVelocity;
    private float _burstEndTime;

    public bool IsHusband => _role == PlayerRole.Husband;
    public int PlayerIndex => (int)_role;
    public float SpeedMultiplier { get; set; } = 1f;
    public Vector3 Velocity => _body.linearVelocity;

    private bool IsBursting => Time.time < _burstEndTime;
    private float BaseSpeed => IsHusband ? _husbandSpeed : _wifeSpeed;
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
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    public void ApplyBurst(Vector3 velocity, float duration)
    {
        _burstVelocity = velocity;
        _burstEndTime = Time.time + duration;
    }

    private void Move()
    {
        Vector3 current = _body.linearVelocity;

        if (IsBursting)
        {
            _body.linearVelocity = new Vector3(_burstVelocity.x, current.y, _burstVelocity.z);
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

    private Vector2 ReadKeyboard()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return Vector2.zero;

        float x = Axis(IsHusband ? kb.dKey : kb.rightArrowKey, IsHusband ? kb.aKey : kb.leftArrowKey);
        float y = Axis(IsHusband ? kb.wKey : kb.upArrowKey, IsHusband ? kb.sKey : kb.downArrowKey);

        return new Vector2(x, y);
    }

    private Vector2 ReadGamepad()
    {
        if (Gamepad.all.Count <= PlayerIndex) return Vector2.zero;

        return Gamepad.all[PlayerIndex].leftStick.ReadValue();
    }

    private static float Axis(UnityEngine.InputSystem.Controls.KeyControl positive,
                              UnityEngine.InputSystem.Controls.KeyControl negative)
    {
        return (positive.isPressed ? 1f : 0f) - (negative.isPressed ? 1f : 0f);
    }
}