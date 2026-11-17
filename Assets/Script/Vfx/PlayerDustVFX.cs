
using UnityEngine;

public class PlayerDustVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovementController movement;
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private Transform dustSpawnPoint;

    [Header("Dust Settings")]
    [SerializeField] private float spawnInterval = 0.2f;
    [SerializeField] private float minimumSpeed = 0.5f;

    private float spawnTimer;

    private void Update()
    {
        if (movement == null || dustPrefab == null || dustSpawnPoint == null)
            return;

        Debug.Log("Grounded: " + movement.IsGrounded + " | Velocity: " + movement.Velocity);

        // Dust hanya muncul ketika player berada di tanah.
        if (!movement.IsGrounded)
        {
            spawnTimer = 0f;
            return;
        }

        // Ambil kecepatan horizontal player.
        Vector3 velocity = movement.Velocity;
        velocity.y = 0f;

        // Player terlalu lambat atau sedang diam.
        if (velocity.magnitude < minimumSpeed)
        {
            spawnTimer = 0f;
            return;
        }

        // Hitung interval spawn.
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnDust();
            spawnTimer = 0f;
        }
    }

    private void SpawnDust()
    {
        Debug.Log("DUST SPAWNED!");

        Instantiate(
            dustPrefab,
            dustSpawnPoint.position,
            dustSpawnPoint.rotation
        );
    }
}
