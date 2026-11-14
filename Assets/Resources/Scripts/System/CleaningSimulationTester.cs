// CleaningSimulationTester.cs — Simulasi test otomatis (tanpa input/waktu nyata) untuk SYS-01..04.
// Fitur: matriks tool x grime, durasi P1/P2, 2 tahap Genangan, radius, rasio & event, reset. Hasil di Console + layar.
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using data.gameplay;

public class CleaningSimulationTester : MonoBehaviour
{
    [Header("Jalankan")]
    [SerializeField] private bool _runOnStart = true;

    [Header("Simulasi")]
    [Tooltip("Langkah waktu virtual per iterasi (1/60 = 60 FPS).")]
    [SerializeField] private float _stepSeconds = 1f / 60f;
    [Tooltip("Toleransi pengukuran durasi (detik).")]
    [SerializeField] private float _timeTolerance = 0.05f;

    [Header("Spesifikasi yang diuji (Pretelan §2.1)")]
    [SerializeField] private float _p1Radius = 0.9f;
    [SerializeField] private float _p1Seconds = 3.0f;
    [SerializeField] private float _p2Radius = 1.5f;
    [SerializeField] private float _p2Seconds = 0.8f;

    private readonly List<GameObject> _created = new List<GameObject>();
    private readonly List<GrimeSpot> _createdSpots = new List<GrimeSpot>();
    private readonly StringBuilder _report = new StringBuilder();
    private CleaningSystemManager _manager;
    private GameObject _managerObject;
    private int _passCount;
    private int _failCount;
    private bool _finished;

    private void Start()
    {
        if (_runOnStart) RunAllTests();
    }

    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[CleaningSim] Jalankan saat Play Mode.");
            return;
        }
        if (CleaningSystemManager.Instance != null)
        {
            Debug.LogError("[CleaningSim] Scene sudah punya CleaningSystemManager. Jalankan di scene test KOSONG.");
            return;
        }

        _report.Clear();
        _passCount = 0;
        _failCount = 0;
        _finished = false;

        _managerObject = new GameObject("SimCleaningSystemManager");
        _manager = _managerObject.AddComponent<CleaningSystemManager>();

        testToolMatrix();
        testTiming();
        testPuddleTwoStages();
        testRadius();
        testRatioAndEvents();
        testNoToolAndReset();

        Destroy(_managerObject);
        _finished = true;

        string summary = $"[CleaningSim] SELESAI — PASS: {_passCount}, FAIL: {_failCount}";
        if (_failCount == 0) Debug.Log("<color=#4CAF50>" + summary + "</color>");
        else Debug.LogError(summary);
        _report.AppendLine(summary);
    }

    // ---------------------------------------------------------------- tests

    // SYS-02: setiap grime hanya bersih dengan tool yang benar. Matriks expected ditulis independen dari kode produksi.
    private void testToolMatrix()
    {
        GrimeLevel[] levels = { GrimeLevel.Dust, GrimeLevel.Spill, GrimeLevel.Puddle };
        CleaningToolType[] tools = { CleaningToolType.Broom, CleaningToolType.Duster, CleaningToolType.Mop, CleaningToolType.None };

        for (int l = 0; l < levels.Length; l++)
        {
            for (int t = 0; t < tools.Length; t++)
            {
                PlayerCleaner cleaner = makeCleaner(_p2Radius, _p2Seconds, tools[t]);
                GrimeSpot spot = makeSpot(levels[l], cleaner.CleanPoint);

                runFor(cleaner, 3f); // 3 s > waktu terlama P2 (Genangan = 1.6 s)

                bool expected = expectedCanClean(levels[l], tools[t]);
                check($"SYS-02 {levels[l]} + {tools[t]} => bersih={expected}", spot.IsClean == expected,
                      $"IsClean={spot.IsClean}, progress={spot.Progress01:F2}");
                cleanup();
            }
        }
    }

    // SYS-03 / SYS-04: durasi per petak.
    private void testTiming()
    {
        timingCase("SYS-03 P1 Debu+Broom", _p1Radius, _p1Seconds, GrimeLevel.Dust, CleaningToolType.Broom, _p1Seconds);
        timingCase("SYS-03 P1 Tumpahan+Mop", _p1Radius, _p1Seconds, GrimeLevel.Spill, CleaningToolType.Mop, _p1Seconds);
        timingCase("SYS-04 P2 Debu+Duster", _p2Radius, _p2Seconds, GrimeLevel.Dust, CleaningToolType.Duster, _p2Seconds);
        timingCase("SYS-04 P2 Tumpahan+Mop", _p2Radius, _p2Seconds, GrimeLevel.Spill, CleaningToolType.Mop, _p2Seconds);
        // Genangan = 2 tahap => 2x durasi
        timingCase("SYS-03 P1 Genangan+Mop (2 tahap)", _p1Radius, _p1Seconds, GrimeLevel.Puddle, CleaningToolType.Mop, _p1Seconds * 2f);
        timingCase("SYS-04 P2 Genangan+Mop (2 tahap)", _p2Radius, _p2Seconds, GrimeLevel.Puddle, CleaningToolType.Mop, _p2Seconds * 2f);
    }

    private void timingCase(string name, float radius, float seconds, GrimeLevel level, CleaningToolType tool, float expectedSeconds)
    {
        PlayerCleaner cleaner = makeCleaner(radius, seconds, tool);
        GrimeSpot spot = makeSpot(level, cleaner.CleanPoint);
        float elapsed = runUntilClean(cleaner, spot, expectedSeconds * 3f);
        bool ok = elapsed >= 0f && Mathf.Abs(elapsed - expectedSeconds) <= _timeTolerance + _stepSeconds;
        check($"{name}: ~{expectedSeconds:F2}s", ok, $"terukur={elapsed:F3}s");
        cleanup();
    }

    // Genangan: setelah tahap 1 -> progres 50%, belum bersih, CurrentStage = 1.
    private void testPuddleTwoStages()
    {
        PlayerCleaner cleaner = makeCleaner(_p2Radius, _p2Seconds, CleaningToolType.Mop);
        GrimeSpot spot = makeSpot(GrimeLevel.Puddle, cleaner.CleanPoint);

        int steps = Mathf.RoundToInt(_p2Seconds / _stepSeconds);
        for (int i = 0; i < steps; i++) cleaner.CleanStep(_stepSeconds);

        check("Genangan akhir tahap 1: progres ~0.5", Mathf.Abs(spot.Progress01 - 0.5f) <= 0.03f, $"progress={spot.Progress01:F3}");
        check("Genangan akhir tahap 1: belum bersih", !spot.IsClean, $"IsClean={spot.IsClean}");
        check("Genangan akhir tahap 1: CurrentStage = 1", spot.CurrentStage == 1, $"stage={spot.CurrentStage}");
        cleanup();
    }

    // Radius: spot di dalam radius ikut bersih, di luar tidak. Y berbeda tidak berpengaruh (bidang XZ).
    private void testRadius()
    {
        radiusCase("P1", _p1Radius, _p1Seconds);
        radiusCase("P2", _p2Radius, _p2Seconds);
    }

    private void radiusCase(string who, float radius, float seconds)
    {
        PlayerCleaner cleaner = makeCleaner(radius, seconds, CleaningToolType.Broom);
        Vector3 c = cleaner.CleanPoint;
        GrimeSpot inside = makeSpot(GrimeLevel.Dust, c + new Vector3(radius - 0.1f, 0.2f, 0f));
        GrimeSpot outside = makeSpot(GrimeLevel.Dust, c + new Vector3(radius + 0.1f, 0f, 0f));

        runFor(cleaner, seconds * 1.2f);

        check($"Radius {who} ({radius} m): spot di dalam bersih", inside.IsClean, $"progress={inside.Progress01:F2}");
        check($"Radius {who} ({radius} m): spot di luar tetap kotor", outside.Progress01 == 0f, $"progress={outside.Progress01:F2}");
        cleanup();
    }

    // Rasio kebersihan + event.
    private void testRatioAndEvents()
    {
        PlayerCleaner cleaner = makeCleaner(_p2Radius, _p2Seconds, CleaningToolType.Broom);
        Vector3[] positions = { new Vector3(0, 0, 0), new Vector3(10, 0, 0), new Vector3(20, 0, 0), new Vector3(30, 0, 0) };
        GrimeSpot[] spots = new GrimeSpot[positions.Length];
        for (int i = 0; i < positions.Length; i++) spots[i] = makeSpot(GrimeLevel.Dust, positions[i]);

        int cleanedEvents = 0;
        int ratioEvents = 0;
        float lastRatio = -1f;
        System.Action<GrimeSpot> onCleaned = s => cleanedEvents++;
        System.Action<float> onRatio = r => { ratioEvents++; lastRatio = r; };
        _manager.OnSpotCleaned += onCleaned;
        _manager.OnCleanlinessRatioChanged += onRatio;

        check("Rasio awal = 0", Mathf.Approximately(_manager.CleanlinessRatio, 0f), $"ratio={_manager.CleanlinessRatio:F3}");

        for (int i = 0; i < spots.Length; i++)
        {
            moveCleanPointTo(cleaner, positions[i]);
            runUntilClean(cleaner, spots[i], _p2Seconds * 3f);
            float expected = (i + 1) / (float)spots.Length;
            check($"Rasio setelah {i + 1}/{spots.Length} spot = {expected:F2}",
                  Mathf.Abs(_manager.CleanlinessRatio - expected) <= 0.001f, $"ratio={_manager.CleanlinessRatio:F3}");
        }

        check("Event OnSpotCleaned terpanggil 4x", cleanedEvents == 4, $"count={cleanedEvents}");
        check("Event OnCleanlinessRatioChanged terpanggil & terakhir = 1", ratioEvents > 0 && Mathf.Approximately(lastRatio, 1f),
              $"count={ratioEvents}, last={lastRatio:F3}");

        _manager.OnSpotCleaned -= onCleaned;
        _manager.OnCleanlinessRatioChanged -= onRatio;
        cleanup();
    }

    private void testNoToolAndReset()
    {
        PlayerCleaner cleaner = makeCleaner(_p2Radius, _p2Seconds, CleaningToolType.None);
        GrimeSpot spot = makeSpot(GrimeLevel.Dust, cleaner.CleanPoint);
        runFor(cleaner, 2f);
        check("Tool None: tidak ada progres", spot.Progress01 == 0f, $"progress={spot.Progress01:F2}");

        cleaner.SetTool(CleaningToolType.Broom);
        runFor(cleaner, 2f);
        check("Setelah ganti ke Broom: bersih", spot.IsClean, $"progress={spot.Progress01:F2}");

        _manager.ResetAllSpots();
        check("ResetAllSpots: spot kotor lagi", !spot.IsClean && spot.Progress01 == 0f, $"progress={spot.Progress01:F2}");
        check("ResetAllSpots: rasio = 0", Mathf.Approximately(_manager.CleanlinessRatio, 0f), $"ratio={_manager.CleanlinessRatio:F3}");

        runFor(cleaner, 2f);
        check("Setelah reset bisa dibersihkan lagi", spot.IsClean, $"progress={spot.Progress01:F2}");
        cleanup();
    }

    // ---------------------------------------------------------------- helpers

    private static bool expectedCanClean(GrimeLevel level, CleaningToolType tool)
    {
        if (level == GrimeLevel.Dust) return tool == CleaningToolType.Broom || tool == CleaningToolType.Duster;
        return tool == CleaningToolType.Mop; // Spill & Puddle
    }

    private PlayerCleaner makeCleaner(float radius, float seconds, CleaningToolType tool)
    {
        GameObject go = new GameObject("SimCleaner");
        go.transform.position = Vector3.zero;
        PlayerCleaner cleaner = go.AddComponent<PlayerCleaner>();
        cleaner.Configure(radius, seconds, tool);
        _created.Add(go);
        return cleaner;
    }

    private GrimeSpot makeSpot(GrimeLevel level, Vector3 position)
    {
        GameObject go = new GameObject("SimSpot_" + level);
        go.transform.position = position;
        GrimeSpot spot = go.AddComponent<GrimeSpot>(); // OnEnable -> auto-register ke manager
        spot.Initialize(level);
        _created.Add(go);
        _createdSpots.Add(spot);
        return spot;
    }

    private void moveCleanPointTo(PlayerCleaner cleaner, Vector3 target)
    {
        cleaner.transform.position += target - cleaner.CleanPoint;
    }

    private void runFor(PlayerCleaner cleaner, float seconds)
    {
        int steps = Mathf.CeilToInt(seconds / _stepSeconds);
        for (int i = 0; i < steps; i++) cleaner.CleanStep(_stepSeconds);
    }

    /// <summary>Mengembalikan waktu virtual sampai bersih, atau -1 jika tidak bersih dalam maxSeconds.</summary>
    private float runUntilClean(PlayerCleaner cleaner, GrimeSpot spot, float maxSeconds)
    {
        float t = 0f;
        while (t < maxSeconds)
        {
            cleaner.CleanStep(_stepSeconds);
            t += _stepSeconds;
            if (spot.IsClean) return t;
        }
        return -1f;
    }

    // Unregister manual dulu: Destroy() baru efektif di akhir frame, sedangkan test berikutnya jalan di frame yang sama.
    private void cleanup()
    {
        for (int i = 0; i < _createdSpots.Count; i++) _manager.UnregisterSpot(_createdSpots[i]);
        for (int i = 0; i < _created.Count; i++) Destroy(_created[i]);
        _createdSpots.Clear();
        _created.Clear();
    }

    private void check(string name, bool condition, string detail)
    {
        if (condition) _passCount++; else _failCount++;
        string line = (condition ? "PASS  " : "FAIL  ") + name + "  [" + detail + "]";
        _report.AppendLine(line);
        if (!condition) Debug.LogError("[CleaningSim] " + line);
        else Debug.Log("[CleaningSim] " + line);
    }

    private void OnGUI()
    {
        if (!_finished) return;
        GUI.color = _failCount == 0 ? Color.green : Color.red;
        GUILayout.BeginArea(new Rect(10, 10, 900, 1000));
        GUILayout.Label(_report.ToString());
        GUILayout.EndArea();
    }
}