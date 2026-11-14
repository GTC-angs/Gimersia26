// PlayerCleaner.cs — Aksi membersihkan milik pemain: radius ayunan + durasi per petak (SYS-02, SYS-03, SYS-04).
// Fitur: input Clean (New Input System), cek tool, cari spot dalam radius, terapkan progres, gizmo debug.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using data.gameplay;

public class PlayerCleaner : MonoBehaviour
{
    [Header("Performa (P1: 0.9 m / 3.0 s — P2: 1.5 m / 0.8 s)")]
    [SerializeField] private float _swingRadius = 0.9f;
    [SerializeField] private float _secondsPerCell = 3f;

    [Header("Titik Ayunan")]
    [Tooltip("Offset lokal dari pemain (default: 0.8 m di depan).")]
    [SerializeField] private Vector3 _cleanPointOffset = new Vector3(0f, 0f, 0.8f);

    [Header("Tool")]
    [SerializeField] private CleaningToolType _currentTool = CleaningToolType.Mop;

    [Header("Input (opsional)")]
    [Tooltip("Untuk test solo. Dengan PlayerInput per-pemain, pakai OnClean(InputValue) atau OnCleanCallback.")]
    // [SerializeField] private InputActionReference _cleanActionReference;

    [Header("Debug")]
    [SerializeField] private bool _drawGizmos = true;

    private readonly List<GrimeSpot> _buffer = new List<GrimeSpot>(32);
    [SerializeField] private bool _inputHeld;

    public CleaningToolType CurrentTool => _currentTool;
    public float SwingRadius => _swingRadius;
    public float SecondsPerCell => _secondsPerCell;
    public Vector3 CleanPoint => transform.TransformPoint(_cleanPointOffset);
    public bool IsCleaning { get; private set; }

    // private void OnEnable()
    // {
    //     // if (_cleanActionReference != null && _cleanActionReference.action != null)
    //     //     _cleanActionReference.action.Enable();
    // }

    private void OnDisable()
    {
        _inputHeld = false;
        IsCleaning = false;
    }

    private void Update()
    {
        bool held = _inputHeld;
                    // (_cleanActionReference != null && _cleanActionReference.action != null &&
                    //  _cleanActionReference.action.IsPressed());

        IsCleaning = held && _currentTool != CleaningToolType.None;
        if (IsCleaning) CleanStep(Time.deltaTime);
    }

    /// <summary>
    /// PlayerInput Behavior = "Send Messages"/"Broadcast Messages": dipanggil otomatis untuk action bernama "Clean".
    /// Signature harus OnClean(InputValue), jangan di-overload agar tidak ambigu.
    /// </summary>
    public void OnClean(InputValue value)
    {
        Debug.Log(value.isPressed);
        _inputHeld = value.isPressed;
    }

    /// <summary>PlayerInput Behavior = "Invoke Unity Events": drag PlayerCleaner ke event Clean, pilih OnCleanCallback.</summary>
    // public void OnCleanCallback(InputAction.CallbackContext context)
    // {
    //     if (context.started || context.performed) _inputHeld = true;
    //     else if (context.canceled) _inputHeld = false;
    // }

    /// <summary>Untuk skrip lain (mis. controller pemain) yang mengatur status tahan sendiri.</summary>
    public void SetCleaningHeld(bool held)
    {
        _inputHeld = held;
    }

    public void SetTool(CleaningToolType tool)
    {
        _currentTool = tool;
    }

    public void Configure(float swingRadius, float secondsPerCell, CleaningToolType tool)
    {
        _swingRadius = Mathf.Max(0f, swingRadius);
        _secondsPerCell = Mathf.Max(0.01f, secondsPerCell);
        _currentTool = tool;
    }

    /// <summary>Satu langkah bersih-bersih. Mengembalikan jumlah spot yang progresnya bertambah.</summary>
    public int CleanStep(float deltaTime)
    {
        if (_currentTool == CleaningToolType.None || deltaTime <= 0f) return 0;

        CleaningSystemManager manager = CleaningSystemManager.Instance;
        if (manager == null) return 0;

        float work = deltaTime / Mathf.Max(_secondsPerCell, 0.01f);
        manager.GetSpotsInRadius(CleanPoint, _swingRadius, _buffer);

        int affected = 0;
        for (int i = 0; i < _buffer.Count; i++)
        {
            if (_buffer[i].ApplyCleaning(_currentTool, work)) affected++;
        }
        return affected;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos) return;
        Gizmos.color = IsCleaning ? Color.green : Color.cyan;
        Gizmos.DrawWireSphere(CleanPoint, _swingRadius);
    }
}