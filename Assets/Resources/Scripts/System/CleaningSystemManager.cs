// CleaningSystemManager.cs — Registry GrimeSpot + rasio kebersihan level (SYS-01).
// Fitur: singleton scene, registrasi spot, query spot dalam radius, event OnCleanlinessRatioChanged, reset semua spot.
using System;
using System.Collections.Generic;
using UnityEngine;

public class CleaningSystemManager : MonoBehaviour
{
    public static CleaningSystemManager Instance { get; private set; }

    [Header("Registry")]
    [Tooltip("Cari semua GrimeSpot di scene saat Awake.")]
    [SerializeField] private bool _autoFindSpotsOnAwake = true;

    [Header("Event")]
    [Tooltip("Event rasio hanya dikirim bila berubah minimal sebesar ini (hemat update UI).")]
    [SerializeField, Range(0f, 0.1f)] private float _ratioEventThreshold = 0.001f;

    [SerializeField] private readonly List<GrimeSpot> _spots = new List<GrimeSpot>();
    private float _progressSum;
    private float _lastRaisedRatio;

    /// <summary>Rata-rata progres semua spot (0..1). Tidak ada spot = 1.</summary>
    public float CleanlinessRatio => _spots.Count == 0 ? 1f : Mathf.Clamp01(_progressSum / _spots.Count);
    public int TotalSpots => _spots.Count;

    public int CleanedSpots
    {
        get
        {
            int n = 0;
            for (int i = 0; i < _spots.Count; i++) if (_spots[i].IsClean) n++;
            return n;
        }
    }

    public event Action<float> OnCleanlinessRatioChanged;
    public event Action<GrimeSpot> OnSpotCleaned;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[CleaningSystemManager] Sudah ada instance lain, duplikat dihapus.");
            Destroy(this);
            return;
        }
        Instance = this;

        if (_autoFindSpotsOnAwake)
        {
            GrimeSpot[] found = FindObjectsByType<GrimeSpot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < found.Length; i++) RegisterSpot(found[i]);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void RegisterSpot(GrimeSpot spot)
    {
        if (spot == null || _spots.Contains(spot)) return;
        _spots.Add(spot);
        spot.OnProgressChanged += handleSpotProgress;
        spot.OnCleaned += handleSpotCleaned;
        recalculateProgress();
        raiseRatioIfChanged(true);
    }

    public void UnregisterSpot(GrimeSpot spot)
    {
        if (spot == null || !_spots.Remove(spot)) return;
        spot.OnProgressChanged -= handleSpotProgress;
        spot.OnCleaned -= handleSpotCleaned;
        recalculateProgress();
        raiseRatioIfChanged(true);
    }

    /// <summary>Isi 'results' dengan spot belum bersih dalam radius (bidang XZ, top-down). Tanpa alokasi.</summary>
    public void GetSpotsInRadius(Vector3 center, float radius, List<GrimeSpot> results)
    {
        results.Clear();
        float sqrRadius = radius * radius;
        for (int i = 0; i < _spots.Count; i++)
        {
            GrimeSpot spot = _spots[i];
            if (spot.IsClean) continue;
            Vector3 p = spot.transform.position;
            float dx = p.x - center.x;
            float dz = p.z - center.z;
            if (dx * dx + dz * dz <= sqrRadius) results.Add(spot);
        }
    }

    public void ResetAllSpots()
    {
        for (int i = 0; i < _spots.Count; i++) _spots[i].ResetGrime();
        recalculateProgress();
        raiseRatioIfChanged(true);
    }

    private void handleSpotProgress(GrimeSpot spot, float delta)
    {
        _progressSum = Mathf.Clamp(_progressSum + delta, 0f, _spots.Count);
        raiseRatioIfChanged(false);
    }

    private void handleSpotCleaned(GrimeSpot spot)
    {
        recalculateProgress(); // buang drift floating point
        raiseRatioIfChanged(false);
        OnSpotCleaned?.Invoke(spot);
    }

    private void recalculateProgress()
    {
        float sum = 0f;
        for (int i = 0; i < _spots.Count; i++) sum += _spots[i].Progress01;
        _progressSum = sum;
    }

    private void raiseRatioIfChanged(bool force)
    {
        float ratio = CleanlinessRatio;
        bool reachedFull = ratio >= 1f - 1e-4f && _lastRaisedRatio < 1f - 1e-4f;
        bool reachedEmpty = ratio <= 1e-4f && _lastRaisedRatio > 1e-4f;

        if (force || reachedFull || reachedEmpty || Mathf.Abs(ratio - _lastRaisedRatio) >= _ratioEventThreshold)
        {
            _lastRaisedRatio = ratio;
            OnCleanlinessRatioChanged?.Invoke(ratio);
        }
    }
}