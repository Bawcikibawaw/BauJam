using UnityEngine;

// Bu script, sahnedeki 'Dikdortgen' objesine eklenmelidir.
public class CarpismaKontrol : MonoBehaviour
{
    [Header("Atanacak Objeler")]
    [Tooltip("�arp��ma sonras� olu�turulacak �alter nesnesinin Prefab'�")]
    [SerializeField] private GameObject salterPrefab;

    [Tooltip("��genin �arpmas� gereken hedef nokta")]
    [SerializeField] private Transform hedefNokta;

    [Header("Ayarlar")]
    [Tooltip("Hedef noktaya ne kadar yak�n bir �arp��man�n kabul edilece�i")]
    [SerializeField] private float toleransMesafesi = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ucgen"))
        {
            Vector2 carpismaNoktasi = collision.contacts[0].point;
            float mesafe = Vector2.Distance(carpismaNoktasi, hedefNokta.position);

            if (mesafe <= toleransMesafesi)
            {
                // Nesneyi do�rudan hedef noktan�n konumunda olu�tur
                Instantiate(salterPrefab, hedefNokta.position, Quaternion.identity);
                Debug.Log("�alter nesnesi olu�turuldu.");
            }
        }
    }
}