using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerStaminaController : MonoBehaviour
{
    [System.Serializable]
    private class DashProfile
    {
        public float speed = 7f;
        [Range(0f, 1f)] public float cost = 0.2f;
        public float regenSeconds = 3f;
    }

    [Header("Dash")]
    [SerializeField] private float _dashDuration = 0.22f;
    [SerializeField] private float _bufferWindow = 0.15f;
    [SerializeField] private DashProfile _husband = new DashProfile();
    [SerializeField] private DashProfile _wife = new DashProfile
    {
        speed = 8f,
        cost = 0.35f,
        regenSeconds = 5f
    };

    [Header("Playful Clash")]
    [SerializeField] private float _knockbackDistance = 0.5f;
    [SerializeField] private float _knockbackDuration = 0.15f;
    [SerializeField] private ParticleSystem _clashParticles;

    private PlayerMovementController _movement;
    private float _dashEndTime;
    private float _bufferedUntil;

    public float Stamina { get; private set; } = 1f;
    public bool DashBlocked { get; set; }
    public bool IsDashing => Time.time < _dashEndTime;

    private DashProfile Profile => _movement.IsHusband ? _husband : _wife;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        if (DashPressed()) _bufferedUntil = Time.time + _bufferWindow;

        TryConsumeBuffer();
        Regenerate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerStaminaController other = collision.collider.GetComponentInParent<PlayerStaminaController>();
        if (other == null || other == this) return;
        if (!IsDashing && !other.IsDashing) return;

        Vector3 away = transform.position - other.transform.position;
        away.y = 0f;
        if (away.sqrMagnitude < 0.0001f) away = -transform.forward;

        _dashEndTime = 0f;
        _movement.ApplyBurst(away.normalized * (_knockbackDistance / _knockbackDuration), _knockbackDuration);
        PlayClashParticles(collision.GetContact(0).point);
    }

    private void TryConsumeBuffer()
    {
        if (Time.time > _bufferedUntil || IsDashing) return;

        if (DashBlocked)
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
        Stamina -= Profile.cost;
        _dashEndTime = Time.time + _dashDuration;
        _movement.ApplyBurst(transform.forward * Profile.speed, _dashDuration);
    }

    private void Regenerate()
    {
        if (IsDashing) return;

        Stamina = Mathf.MoveTowards(Stamina, 1f, Time.deltaTime / Profile.regenSeconds);
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
        if (kb != null)
        {
            var key = _movement.IsHusband ? kb.leftShiftKey : kb.rightShiftKey;
            if (key.wasPressedThisFrame) return true;
        }

        int index = _movement.PlayerIndex;
        return Gamepad.all.Count > index && Gamepad.all[index].buttonEast.wasPressedThisFrame;
    }
}