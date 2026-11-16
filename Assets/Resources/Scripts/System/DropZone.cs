// DropZone.cs — Area penataan transparan untuk furnitur (SYS-06).
// Fitur: marker mint #9FE3C2, radius snap, toleransi sudut, target pose, gizmo & status terisi.
using UnityEngine;

public class DropZone : MonoBehaviour
{
    [Header("SYS-06: Zona")]
    [Tooltip("Titik & rotasi ideal barang. Kosongkan untuk pakai transform object ini.")]
    [SerializeField] private Transform _snapPoint;

    [Tooltip("Radius deteksi snap (meter). Spec: 0.5.")]
    [SerializeField, Min(0.05f)] private float _radius = 0.5f;

    [Tooltip("Toleransi sudut Y (derajat) item vs target. Spec: ±35°.")]
    [SerializeField, Range(1f, 180f)] private float _angleTolerance = 35f;

    [Tooltip("Warna marker mint #9FE3C2.")]
    [SerializeField] private Color _markerColor = new Color(0.6235f, 0.8902f, 0.7608f, 0.4f); // #9FE3C2

    [Tooltip("Renderer quad/plane marker. Kosongkan untuk auto-cari di child.")]
    [SerializeField] private Renderer _markerRenderer;

    public float Radius => _radius;
    public float AngleTolerance => _angleTolerance;
    public Vector3 SnapPosition => _snapPoint != null ? _snapPoint.position : transform.position;
    public Quaternion SnapRotation => _snapPoint != null ? _snapPoint.rotation : transform.rotation;
    public Color MarkerColor => _markerColor;

    /// <summary>Barang yang sedang mengisi slot ini (null = kosong). Diatur DropZoneManager.</summary>
    public Rigidbody Occupant { get; private set; }

    private void Awake()
    {
        if (_snapPoint == null) _snapPoint = transform;
        if (_markerRenderer == null) _markerRenderer = GetComponentInChildren<Renderer>(true);
    }

    private void OnEnable()
    {
        if (DropZoneManager.Instance != null) DropZoneManager.Instance.RegisterZone(this);
    }

    private void OnDisable()
    {
        if (DropZoneManager.Instance != null) DropZoneManager.Instance.UnregisterZone(this);
    }

    /// <summary>Cek apakah item berada dalam radius & sudut toleransi zona ini.</summary>
    public bool IsInRange(Rigidbody item)
    {
        if (item == null || Occupant != null) return false;

        Vector3 itemPos = item.worldCenterOfMass;
        Vector3 zonePos = SnapPosition;
        float dx = itemPos.x - zonePos.x;
        float dz = itemPos.z - zonePos.z;
        if (dx * dx + dz * dz > _radius * _radius) return false;

        float angle = Mathf.Abs(Mathf.DeltaAngle(item.transform.eulerAngles.y, SnapRotation.eulerAngles.y));
        return angle <= _angleTolerance;
    }

    internal void Occupy(Rigidbody item)
    {
        Occupant = item;
    }

    internal void Release()
    {
        Occupant = null;
    }

    private void OnDrawGizmos()
    {
        Vector3 p = _snapPoint != null ? _snapPoint.position : transform.position;
        Gizmos.color = _markerColor;
        Gizmos.DrawWireSphere(p, _radius);
    }
}
