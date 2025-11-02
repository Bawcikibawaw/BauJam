using UnityEngine;

public class ZiplatanPlatform : MonoBehaviour
{
    [Header("Zıplatma Ayarları")]
    [Tooltip("Küpün ilk fırlatılacağı güç.")]
    [SerializeField] private float ilkFirlatmaGucu = 25f;

    [Tooltip("Bu gücün altına düştüğünde küp ışınlanır.")]
    [SerializeField] private float minFirlatmaGucu = 2f;

    [Header("Platform Hareket Ayarları")] // <-- YENİ BÖLÜM
    [Tooltip("Küp çarpınca platformun sağa/sola ne kadar güçle itileceği.")]
    [SerializeField] private float randomItmeGucu = 10f;

    [Header("Işınlanma")]
    [Tooltip("Küpün ışınlanacağı hedef nokta.")]
    [SerializeField] private Transform isinlanmaHedefi;

    // Değişkenler
    private float mevcutFirlatmaGucu;
    private Rigidbody2D platformRb; // <-- YENİ: Platformun kendi Rigidbody'si

    // Script başladığında bir kere çalışır
    private void Start()
    {
        // Başlangıçta mevcut gücü ilk güce eşitle
        mevcutFirlatmaGucu = ilkFirlatmaGucu;
        // Platformun kendi Rigidbody2D component'ini değişkene ata
        platformRb = GetComponent<Rigidbody2D>(); // <-- YENİ
    }

    // Bir nesne bu platforma çarptığında çalışır
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Çarpan nesnenin etiketi "Player" mı? (Yani bizim zıplayacak küpümüz mü?)
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D kupRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (kupRb != null && collision.contacts[0].normal.y < -0.5f)
            {
                // --- YENİ KOD BAŞLANGICI ---
                // Platformu rastgele sağa veya sola hareket ettir
                HareketEttir();
                // --- YENİ KOD SONU ---

                if (mevcutFirlatmaGucu >= minFirlatmaGucu)
                {
                    kupRb.linearVelocity = new Vector2(kupRb.linearVelocity.x, mevcutFirlatmaGucu);
                    mevcutFirlatmaGucu /= 2f;
                }
                else
                {
                    if (isinlanmaHedefi != null)
                    {
                        collision.transform.position = isinlanmaHedefi.position;
                    }
                    else
                    {
                        Debug.LogError("Işınlanma Hedefi atanmamış!");
                    }
                    ResetPlatform();
                }
            }
        }
    }

    // Küp platformdan ayrıldığında
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ResetPlatform();
        }
    }

    // Platformun gücünü başlangıç değerine döndüren fonksiyon
    private void ResetPlatform()
    {
        mevcutFirlatmaGucu = ilkFirlatmaGucu;
    }

    // --- YENİ FONKSİYON ---
    // Platforma rastgele yönde bir itme kuvveti uygular
    private void HareketEttir()
    {
        // Rastgele bir yön seç: -1 (sol) veya 1 (sağ)
        float yon = Random.value < 0.5f ? -1f : 1f;

        // Platformun kendi Rigidbody'sine anlık bir kuvvet (impulse) uygula
        if (platformRb != null)
        {
            // Önce mevcut yatay hızı sıfırlayarak daha kontrollü bir itme sağlıyoruz
            platformRb.linearVelocity = new Vector2(0, platformRb.linearVelocity.y);
            // Sonra yeni yöne doğru kuvvet uyguluyoruz
            platformRb.AddForce(new Vector2(yon * randomItmeGucu, 0f), ForceMode2D.Impulse);
        }
    }
}