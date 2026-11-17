
using UnityEngine;

public class DropZoneSnapVFX : MonoBehaviour
{
    [Header("VFX Settings")]
    [SerializeField] private GameObject snapVFXPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Fallback Settings")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(-90f, 0f, 0f);

    private void Start()
    {
        if (DropZoneManager.Instance != null)
        {
            DropZoneManager.Instance.OnItemSnapped += HandleItemSnapped;
            Debug.Log("Snap VFX berhasil subscribe!");
        }
        else
        {
            Debug.LogError("DropZoneManager tidak ditemukan!");
        }
    }

    private void OnDisable()
    {
        if (DropZoneManager.Instance != null)
        {
            DropZoneManager.Instance.OnItemSnapped -= HandleItemSnapped;
        }
    }

    private void HandleItemSnapped(Rigidbody item, DropZone zone)
    {
        if (snapVFXPrefab == null)
            return;

        Vector3 spawnPosition = spawnPoint != null
            ? spawnPoint.position
            : zone.SnapPosition;

        Quaternion spawnRotation = spawnPoint != null
            ? spawnPoint.rotation
            : zone.SnapRotation;

        spawnRotation *= Quaternion.Euler(rotationOffset);

        Instantiate(snapVFXPrefab, spawnPosition, spawnRotation);
    }
}
