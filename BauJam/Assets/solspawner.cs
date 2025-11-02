using UnityEngine;

public class LeftSpawner : MonoBehaviour
{
    // Oluþturulacak nesnenin prefab'ý
    public GameObject carPrefab;

    // Rastgele spawn süresi için minimum ve maksimum deðerler
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 10f;

    // Nesneye uygulanacak sol yönlü kuvvet
    public float spawnForce = 10f;

    void Start()
    {
        // Ýlk spawn iþlemini baþlat
        SpawnNext();
    }

    void SpawnNext()
    {
        // Belirtilen min ve max deðerler arasýnda rastgele bir süre belirle
        float randomTime = Random.Range(minSpawnTime, maxSpawnTime);

        // "SpawnCar" fonksiyonunu, belirlenen rastgele süre sonunda çaðýr
        Invoke("SpawnCar", randomTime);
    }

    void SpawnCar()
    {
        // Prefab'ý spawner'ýn pozisyonunda oluþtur
        GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);

        // Oluþturulan nesnenin yönünü sola çevir
        car.transform.up = Vector2.left;

        // Rigidbody2D bileþenini al
        Rigidbody2D rb = car.GetComponent<Rigidbody2D>();

        // Eðer Rigidbody2D varsa, ona SOL yönde bir kuvvet uygula
        if (rb != null)
        {
            rb.AddForce(Vector2.left * spawnForce, ForceMode2D.Impulse);
        }

        // Bu araba oluþturulduktan sonra, bir sonraki rastgele zamanlý spawn'ý planla
        SpawnNext();
    }
}