using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerStaminaController : MonoBehaviour
{
    [System.Serializable]
    private class DashProfile
    {
        public float speed = 7f;
        [Range(0f, 1f)] public float cost = 0.2f;
        public float regenSeconds = 3f;
        [Tooltip("Jarak recoil (m) saat menabrak perabot dengan kecepatan tinggi.")]
        public float furnitureRecoil = 0.2f;
    }

    [Header("Abilities")]
    [SerializeField] private bool _canDash = true;

    [Header("Keybinds")]
    [SerializeField] private Key _keyDash = Key.LeftShift;
    [SerializeField] private GamepadButton _padDash = GamepadButton.East;

    [Header("Dash")]
    [SerializeField] private float _dashDuration = 0.22f;
    [SerializeField] private float _bufferWindow = 0.15f;
    [SerializeField] private DashProfile _husband = new DashProfile();
    [SerializeField] private DashProfile _wife = new DashProfile
    {
        speed = 8f,
        cost = 0.35f,
        regenSeconds = 5f,
        furnitureRecoil = 0.8f
    };

    [Header("Air Dash (CCC-05)")]
    [SerializeField] private bool _oneAirDashPerJump = true;

    [Header("Playful Clash")]
    [SerializeField] private float _knockbackDistance = 0.5f;
    [SerializeField] private float _knockbackDuration = 0.15f;
    [SerializeField] private ParticleSystem _clashParticles;

    [Header("Furniture Recoil (CCC-03)")]
    [SerializeField] private LayerMask _furnitureMask;
    [SerializeField] private float _recoilMinImpact = 5f;

    [Header("Skid (CCC-07, hanya istri, hanya di tanah)")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _skidTrigger = "Skid";
    [SerializeField] private ParticleSystem _skidParticles;

    private PlayerMovementController _movement;
    private int _skidHash;
    private float _dashEndTime;
    private float _bufferedUntil;
    private bool _airDashUsed;

    public float Stamina { get; private set; } = 1f;
    public bool DashBlocked { get; set; }
    public bool IsDashing => Time.time < _dashEndTime;

    private DashProfile Profile => _movement.IsHusband ? _husband : _wife;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _skidHash = Animator.StringToHash(_skidTrigger);
    }

    private void Update()
    {
        if (_movement.IsGrounded) _airDashUsed = false;
        if (_canDash && DashPressed()) _bufferedUntil = Time.time + _bufferWindow;

        TryConsumeBuffer();
        Regenerate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerStaminaController other = collision.collider.GetComponentInParent<PlayerStaminaController>();

        if (other == this) return;

        if (other != null) HandlePlayerClash(other, collision);
        else HandleFurnitureImpact(collision);
    }

    private void HandlePlayerClash(PlayerStaminaController other, Collision collision)
    {
        if (!IsDashing && !other.IsDashing) return;

        Vector3 away = transform.position - other.transform.position;
        away.y = 0f;
        if (away.sqrMagnitude < 0.0001f) away = -transform.forward;

        _dashEndTime = 0f;
        _movement.ApplyBurst(away.normalized * (_knockbackDistance / _knockbackDuration), _knockbackDuration);
        PlayClashParticles(collision.GetContact(0).point);
    }

    // CCC-02/03: suami stabil (recoil kecil), istri recoil lebih besar saat menabrak perabot.
    private void HandleFurnitureImpact(Collision collision)
    {
        if ((_furnitureMask.value & (1 << collision.gameObject.layer)) == 0) return;
        if (collision.relativeVelocity.magnitude < _recoilMinImpact) return;

        Vector3 point = collision.GetContact(0).point;
        Vector3 away = transform.position - point;
        away.y = 0f;
        if (away.sqrMagnitude < 0.0001f) away = -transform.forward;

        _dashEndTime = 0f;
        _movement.ApplyBurst(away.normalized * (Profile.furnitureRecoil / _knockbackDuration), _knockbackDuration);
    }

    private void TryConsumeBuffer()
    {
        if (Time.time > _bufferedUntil || IsDashing) return;

        if (DashBlocked)
        {
            _bufferedUntil = 0f;
            return;
        }

        if (_oneAirDashPerJump && !_movement.IsGrounded && _airDashUsed)
        {
            _bufferedUntil = 0f;
            return;
        }

        if (Stamina < Profile.cost) return;

        StartDash();
        _bufferedUntil = 0f;
    }

    private void StartDash()
    {
        bool airborne = !_movement.IsGrounded;

        Stamina -= Profile.cost;
        _dashEndTime = Time.time + _dashDuration;

        // Air dash: burst datar (gravitasi ditahan selama burst).
        _movement.ApplyBurst(transform.forward * Profile.speed, _dashDuration, airborne);

        if (airborne) _airDashUsed = true;
        else if (!_movement.IsHusband) PlaySkid();
    }

    private void Regenerate()
    {
        if (IsDashing) return;

        Stamina = Mathf.MoveTowards(Stamina, 1f, Time.deltaTime / Profile.regenSeconds);
    }

    private void PlaySkid()
    {
        if (_animator != null) _animator.SetTrigger(_skidHash);
        if (_skidParticles != null) _skidParticles.Play();
    }

    private void PlayClashParticles(Vector3 point)
    {
        if (_clashParticles == null) return;

        _clashParticles.transform.position = point;
        _clashParticles.Play();
    }

    private bool DashPressed()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && _keyDash != Key.None && kb[_keyDash].wasPressedThisFrame) return true;

        int index = _movement.PlayerIndex;
        return Gamepad.all.Count > index && Gamepad.all[index][_padDash].wasPressedThisFrame;
    }
}