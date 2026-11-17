// DropZoneManager.cs — Registry drop-zone + magnetic snap + static kinematic lock (SYS-06/07/08).
// Fitur: singleton scene, registrasi zona, cari zona terdekat valid, snap via DOTween, kunci kinematis pasca-snap.
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropZoneManager : MonoBehaviour
{
    public static DropZoneManager Instance { get; private set; }

    [Header("SYS-07: Magnetic Snap")]
    [Tooltip("Durasi tween snap ke posisi ideal.")]
    [SerializeField, Min(0.01f)] private float _snapDuration = 0.25f;

    [Tooltip("Scale punch setelah snap (SYS-07 juice).")]
    [SerializeField] private float _snapPunchScale = 1.15f;

    [Header("Registry")]
    [SerializeField] private bool _autoFindZonesOnAwake = true;

    [SerializeField] private readonly List<DropZone> _zones = new List<DropZone>();

    /// <summary>Dipanggil saat snap sukses: (item, zona).</summary>
    public event Action<Rigidbody, DropZone> OnItemSnapped;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[DropZoneManager] Sudah ada instance lain, duplikat dihapus.", this);
            Destroy(this);
            return;
        }
        Instance = this;

        if (_autoFindZonesOnAwake)
        {
            DropZone[] found = FindObjectsByType<DropZone>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < found.Length; i++) RegisterZone(found[i]);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void RegisterZone(DropZone zone)
    {
        if (zone == null || _zones.Contains(zone)) return;
        _zones.Add(zone);
    }

    public void UnregisterZone(DropZone zone)
    {
        if (zone == null) return;
        _zones.Remove(zone);
    }

    /// <summary>Zona valid terdekat untuk item (dalam radius & sudut), atau null.</summary>
    public DropZone FindZoneFor(Rigidbody item)
    {
        DropZone best = null;
        float bestSqr = float.MaxValue;
        for (int i = 0; i < _zones.Count; i++)
        {
            DropZone zone = _zones[i];
            if (!zone.IsInRange(item)) continue;
            float sqr = (zone.SnapPosition - item.worldCenterOfMass).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = zone;
            }
        }
        return best;
    }

    /// <summary>SYS-07: snap item ke zona valid terdekat, lalu SYS-08 kunci kinematis. True jika snap dimulai.</summary>
    public bool TrySnap(Rigidbody item)
    {
        DropZone zone = FindZoneFor(item);
        if (zone == null) return false;

        zone.Occupy(item);

        Transform t = item.transform;
        t.DOKill();
        item.linearVelocity = Vector3.zero;
        item.angularVelocity = Vector3.zero;
        item.isKinematic = true;

        Sequence seq = DOTween.Sequence();
        seq.Append(t.DOMove(zone.SnapPosition, _snapDuration).SetEase(Ease.OutQuad));
        seq.Join(t.DORotateQuaternion(zone.SnapRotation, _snapDuration));
        seq.Append(t.DOPunchScale(Vector3.one * (_snapPunchScale - 1f), 0.2f, vibrato: 6, elasticity: 0.6f));
        seq.OnComplete(() =>
        {
            LockItem(item);
            OnItemSnapped?.Invoke(item, zone);
        });

        return true;
    }

    /// <summary>SYS-08: kunci item agar tidak bisa diambil/tertendang lagi.</summary>
    public void LockItem(Rigidbody item)
    {
        if (item == null) return;
        item.isKinematic = true;
        

        DropZoneLock lockMarker = item.GetComponent<DropZoneLock>();
        if (lockMarker == null) lockMarker = item.gameObject.AddComponent<DropZoneLock>();
        lockMarker.MarkLocked();
    }

    /// <summary>SYS-08: buka kunci (mis. reset level).</summary>
    public void UnlockItem(Rigidbody item)
    {
        if (item == null) return;
        DropZoneLock markerComp = item.GetComponent<DropZoneLock>();
        if (markerComp != null) Destroy(markerComp);
        item.isKinematic = false;
        for (int i = 0; i < _zones.Count; i++)
        {
            if (_zones[i].Occupant == item) _zones[i].Release();
        }
    }
}

/// <summary>Penanda furnitur yang sudah tertata rapi (SYS-08). PlayerCarryController menolak mengambil item dengan komponen ini.</summary>
public class DropZoneLock : MonoBehaviour
{
    public bool IsLocked { get; private set; }

    public void MarkLocked()
    {
        IsLocked = true;
    }
}
