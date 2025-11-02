using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.LoadScene("GameOver");
        }

        if (collision.gameObject.CompareTag("SafeZone"))
        {
            SceneManager.LoadScene("PainManager");
        }
    }
}