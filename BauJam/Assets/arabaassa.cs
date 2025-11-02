using UnityEngine;

public class DownSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 10f;
    public float spawnForce = 10f;

    void Start()
    {
        SpawnNext();
    }

    void SpawnNext()
    {
        float randomTime = Random.Range(minSpawnTime, maxSpawnTime);
        Invoke("SpawnCar", randomTime);
    }

    void SpawnCar()
    {
        GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);

        // Aracý aþaðý yönlendir
        car.transform.up = Vector2.down;

        // Aþaðý doðru kuvvet uygula
        Rigidbody2D rb = car.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.down * spawnForce, ForceMode2D.Impulse);

        // Sonraki spawn
        SpawnNext();
    }
}
