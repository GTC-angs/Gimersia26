
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemLandingVFX : MonoBehaviour
{
    [Header("VFX Settings")]
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private Transform dustSpawnPoint;
    [SerializeField] private Vector3 dustRotation = new Vector3(-90f, 0f, 0f);

    [Header("Ground Detection")]
    [SerializeField] private string groundLayerName = "Decals";

    private int groundLayer;
    private bool hasBeenDropped;
    private bool hasSpawnedDust;

    private void Awake()
    {
        groundLayer = LayerMask.NameToLayer(groundLayerName);

        if (groundLayer == -1)
        {
            Debug.LogError("Layer Decals tidak ditemukan!", this);
        }
    }

    private void OnEnable()
    {
        if (DropZoneManager.Instance != null)
            DropZoneManager.Instance.OnItemSnapped += HandleItemSnapped;
    }

    private void OnDisable()
    {
        if (DropZoneManager.Instance != null)
            DropZoneManager.Instance.OnItemSnapped -= HandleItemSnapped;
    }

    public void OnPickedUp()
    {
        hasBeenDropped = false;
        hasSpawnedDust = false;
    }

    public void OnDropped()
    {
        hasBeenDropped = true;
        hasSpawnedDust = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!CanSpawnDust()) return;

        if (collision.gameObject.layer != groundLayer)
            return;

        SpawnDust();
    }

    private void HandleItemSnapped(Rigidbody item, DropZone zone)
    {
        if (item.gameObject != gameObject)
            return;

        if (!CanSpawnDust())
            return;

        SpawnDust();
    }

    private bool CanSpawnDust()
    {
        return hasBeenDropped && !hasSpawnedDust;
    }

    private void SpawnDust()
    {
        if (dustPrefab == null)
            return;

        Vector3 spawnPosition = dustSpawnPoint != null
            ? dustSpawnPoint.position
            : transform.position;

        Instantiate(
            dustPrefab,
            spawnPosition,
            Quaternion.Euler(dustRotation)
        );

        hasSpawnedDust = true;
        hasBeenDropped = false;
    }
}
