// GrimeSpot.cs — Satu "petak" kotoran berbasis URP DecalProjector (SYS-01, SYS-02).
// Fitur: level grime, aturan tool, progres bersih (Puddle 2 tahap), fade decal, event progres & selesai.
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using data.gameplay;

public class GrimeSpot : MonoBehaviour
{
    [Header("Grime")]
    [SerializeField] private GrimeLevel _level = GrimeLevel.Dust;

    [Header("Visual (URP Decal)")]
    [Tooltip("Kosongkan untuk auto-cari DecalProjector di object ini / child-nya.")]
    [SerializeField] private DecalProjector _decal;

    // Satuan "work": 1.0 = satu tahap selesai. Debu/Tumpahan butuh 1.0, Genangan butuh 2.0.
    private float _work;

    public GrimeLevel Level => _level;
    public int StageCount => _level == GrimeLevel.Puddle ? 2 : 1;
    public float Progress01 => Mathf.Clamp01(_work / StageCount);
    /// <summary>Tahap aktif, 0-based (Genangan: 0 = serap, 1 = lap).</summary>
    public int CurrentStage => Mathf.Min(Mathf.FloorToInt(_work), StageCount - 1);
    public bool IsClean => _work >= StageCount;

    /// <summary>(spot, deltaProgress01) — delta bisa negatif saat reset.</summary>
    public event Action<GrimeSpot, float> OnProgressChanged;
    public event Action<GrimeSpot> OnCleaned;

    private void Awake()
    {
        if (_decal == null) _decal = GetComponentInChildren<DecalProjector>(true);
        refreshVisual();
    }

    private void OnEnable()
    {
        if (CleaningSystemManager.Instance != null) CleaningSystemManager.Instance.RegisterSpot(this);
    }

    private void OnDestroy()
    {
        if (CleaningSystemManager.Instance != null) CleaningSystemManager.Instance.UnregisterSpot(this);
    }

    /// <summary>Aturan tool (SYS-02): Debu = Broom/Duster, Tumpahan & Genangan = Mop.</summary>
    public static bool CanToolClean(GrimeLevel level, CleaningToolType tool)
    {
        switch (level)
        {
            case GrimeLevel.Dust:
                return tool == CleaningToolType.Broom || tool == CleaningToolType.Duster;
            case GrimeLevel.Spill:
            case GrimeLevel.Puddle:
                return tool == CleaningToolType.Mop;
            default:
                return false;
        }
    }

    /// <summary>Dipanggil PlayerCleaner. workAmount = deltaTime / secondsPerCell. True jika progres bertambah.</summary>
    public bool ApplyCleaning(CleaningToolType tool, float workAmount)
    {
        if (IsClean || workAmount <= 0f || !CanToolClean(_level, tool)) return false;

        float before = Progress01;
        _work = Mathf.Min(_work + workAmount, StageCount);
        float after = Progress01;

        refreshVisual();
        OnProgressChanged?.Invoke(this, after - before);

        if (IsClean)
        {
            if (_decal != null) _decal.enabled = false;
            OnCleaned?.Invoke(this);
        }
        return true;
    }

    /// <summary>Kembalikan ke kotor penuh (restart level / testing).</summary>
    public void ResetGrime()
    {
        float before = Progress01;
        _work = 0f;
        if (_decal != null) _decal.enabled = true;
        refreshVisual();
        if (before > 0f) OnProgressChanged?.Invoke(this, -before);
    }

    /// <summary>Untuk spot yang dibuat lewat kode (dipakai simulasi test).</summary>
    public void Initialize(GrimeLevel level)
    {
        _level = level;
        ResetGrime();
    }

    private void refreshVisual()
    {
        if (_decal != null) _decal.fadeFactor = 1f - Progress01;
    }
}