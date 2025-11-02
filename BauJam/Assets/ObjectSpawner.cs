using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Ayarlar�")]
    public GameObject objectPrefab;

    // Her bir obje aras�nda ka� saniye beklenece�ini belirler.
    // Her 4 saniyede bir obje ��kmas� i�in bu de�eri 4 yapmal�s�n.
    public float spawnInterval = 4f;

    [Header("H�z Ayarlar�")]
    public float[] possibleSpeeds = new float[3] { 5f, 8f, 10f };

    void Start()
    {
        // Spawner'� ba�lat
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Bu d�ng� oyun boyunca sonsuza kadar �al���r
        while (true)
        {
            // --- Obje olu�turma mant��� ---

            // Rastgele bir h�z se�
            int randomIndex = Random.Range(0, possibleSpeeds.Length);
            float selectedSpeed = possibleSpeeds[randomIndex];

            // Prefab'� bu spawner'�n pozisyonunda olu�tur
            GameObject spawnedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);

            // Olu�turulan objenin i�indeki ObjectMover script'ine ula�
            ObjectMover mover = spawnedObject.GetComponent<ObjectMover>();

            // Script'i bulduysak, se�ti�imiz rastgele h�z� ata
            if (mover != null)
            {
                mover.speed = selectedSpeed;
            }
            else
            {
                Debug.LogError("Spawn edilen '" + objectPrefab.name + "' prefab'�nda ObjectMover script'i bulunamad�!");
            }

            // Bir sonraki objeyi olu�turmadan �nce 'spawnInterval' s�resi kadar bekle.
            // Bu sat�r sayesinde her 4 saniyede bir bu d�ng� tekrar �al���r.
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}