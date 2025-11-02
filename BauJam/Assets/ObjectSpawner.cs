using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Ayarlarý")]
    public GameObject objectPrefab;

    // Her bir obje arasýnda kaç saniye bekleneceðini belirler.
    // Her 4 saniyede bir obje çýkmasý için bu deðeri 4 yapmalýsýn.
    public float spawnInterval = 4f;

    [Header("Hýz Ayarlarý")]
    public float[] possibleSpeeds = new float[3] { 5f, 8f, 10f };

    void Start()
    {
        // Spawner'ý baþlat
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Bu döngü oyun boyunca sonsuza kadar çalýþýr
        while (true)
        {
            // --- Obje oluþturma mantýðý ---

            // Rastgele bir hýz seç
            int randomIndex = Random.Range(0, possibleSpeeds.Length);
            float selectedSpeed = possibleSpeeds[randomIndex];

            // Prefab'ý bu spawner'ýn pozisyonunda oluþtur
            GameObject spawnedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);

            // Oluþturulan objenin içindeki ObjectMover script'ine ulaþ
            ObjectMover mover = spawnedObject.GetComponent<ObjectMover>();

            // Script'i bulduysak, seçtiðimiz rastgele hýzý ata
            if (mover != null)
            {
                mover.speed = selectedSpeed;
            }
            else
            {
                Debug.LogError("Spawn edilen '" + objectPrefab.name + "' prefab'ýnda ObjectMover script'i bulunamadý!");
            }

            // Bir sonraki objeyi oluþturmadan önce 'spawnInterval' süresi kadar bekle.
            // Bu satýr sayesinde her 4 saniyede bir bu döngü tekrar çalýþýr.
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}