using UnityEngine;

// Bu script, sahnedeki 'Dikdortgen' objesine eklenmelidir.
public class CarpismaKontrol : MonoBehaviour
{
    [Header("Atanacak Objeler")]
    [Tooltip("Çarpýþma sonrasý oluþturulacak þalter nesnesinin Prefab'ý")]
    [SerializeField] private GameObject salterPrefab;

    [Tooltip("Üçgenin çarpmasý gereken hedef nokta")]
    [SerializeField] private Transform hedefNokta;

    [Header("Ayarlar")]
    [Tooltip("Hedef noktaya ne kadar yakýn bir çarpýþmanýn kabul edileceði")]
    [SerializeField] private float toleransMesafesi = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ucgen"))
        {
            Vector2 carpismaNoktasi = collision.contacts[0].point;
            float mesafe = Vector2.Distance(carpismaNoktasi, hedefNokta.position);

            if (mesafe <= toleransMesafesi)
            {
                // Nesneyi doðrudan hedef noktanýn konumunda oluþtur
                Instantiate(salterPrefab, hedefNokta.position, Quaternion.identity);
                Debug.Log("Þalter nesnesi oluþturuldu.");
            }
        }
    }
}