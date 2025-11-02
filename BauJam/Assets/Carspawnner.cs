using UnityEngine;

public class RightSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 10f;

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

        // Araba sprite'ý saða bakýyorsa bu satýr doðru
        car.transform.up = Vector2.right;

        Rigidbody2D rb = car.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.right * 10f, ForceMode2D.Impulse); // Gücü istersen inspector’a alýrsýn

        SpawnNext(); // sonraki spawn çaðýr
    }
}
