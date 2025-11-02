using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Değişkenlerin hepsi aynı...
    private Rigidbody2D rb;

    [Header("Oyuncu Hareketi Ayarları")]
    [SerializeField] private float moveSpeed = 7f;
    private float horizontalInput;
    private bool isFacingRight = true;

    [Header("Trambolin Ayarları")]
    [SerializeField] private float ilkFirlatmaGucu = 25f;
    [SerializeField] private float kaymaMesafesi = 5f;
    [SerializeField] private float minFirlatmaGucu = 2f;
    private float mevcutFirlatmaGucu;
    


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        mevcutFirlatmaGucu = ilkFirlatmaGucu;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        Flip();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.contacts[0].normal.y < -0.5f)
        {
            Rigidbody2D kupRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (kupRb != null)
            {
                // --- SORUNU BULMAK İÇİN EKLENEN SATIR ---
                // Her çarpışmada mevcut gücü ve minimum gücü konsola yazdır.
                Debug.Log("Mevcut Güç: " + mevcutFirlatmaGucu + " | Işınlanma için Gerekli Minimum Güç: " + minFirlatmaGucu);
                

                // 2. Küpü zıplat veya ışınla.
                // Eğer mevcut güç, minimum güçten BÜYÜK veya EŞİTSE zıplat.
                if (mevcutFirlatmaGucu >= minFirlatmaGucu)
                {
                    kupRb.linearVelocity = new Vector2(kupRb.linearVelocity.x, mevcutFirlatmaGucu);
                    mevcutFirlatmaGucu /= 2f;
                }
                else // Değilse (yani mevcut güç minimumun ALTINA DÜŞTÜYSE) ışınla.
                {
                    ResetTrampoline();
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ResetTrampoline();
        }
    }

    // Geri kalan fonksiyonlar aynı...
    private void Flip()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }


    private void ResetTrampoline()
    {
        mevcutFirlatmaGucu = ilkFirlatmaGucu;
    }
}