using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Oyuncunun yeniden do�aca�� nokta (Empty Object).")]
    [SerializeField] private Transform spawnPoint;

    // Oyuncunun Rigidbody component'ini tutmak i�in
    private Rigidbody2D rb;

    private void Awake()
    {
        // Script ba�larken Rigidbody component'ini bul ve ata
        rb = GetComponent<Rigidbody2D>();
    }

    // Ba�ka bir nesneye temas edildi�inde bu fonksiyon �al���r
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Temas etti�imiz nesnenin etiketi "Zemin" mi?
        if (collision.gameObject.CompareTag("Zemin"))
        {
            // E�er evetse, oyuncuyu yeniden do�ur
            Respawn();
        }
    }

    // Oyuncuyu yeniden do�uran fonksiyon
    private void Respawn()
    {
        // Spawn noktas� atanm�� m� diye kontrol et
        if (spawnPoint != null)
        {
            // I��nlanma sonras� olu�abilecek istenmeyen hareketleri engellemek i�in
            // oyuncunun h�z�n� (velocity) s�f�rla.
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Oyuncunun pozisyonunu spawn noktas�n�n pozisyonuna e�itle
            transform.position = spawnPoint.position;
        }
        else
        {
            // E�er spawn noktas� atanmad�ysa konsola bir hata mesaj� yazd�r.
            Debug.LogError("Spawn Point atanmam��! L�tfen PlayerRespawn script'ine SpawnNoktasi objesini atay�n.");
        }
    }
}