using UnityEngine;

public class SurukleVeFirlatat : MonoBehaviour
{
    // F�rlatma g�c�n� ayarlamak i�in bir �arpan
    [SerializeField] private float firlatmaGucu = 1.2f;

    private Rigidbody2D rb;
    private bool surukleniyor = false;

    // Farenin son iki konumunu takip etmek i�in de�i�kenler
    private Vector3 farePozisyonu;
    private Vector3 sonFarePozisyonu;
    private Vector3 firlatmaVektoru;

    void Start()
    {
        // Rigidbody2D bile�enini al�yoruz
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        // T�klama ba�lad���nda s�r�kleme modunu aktif et
        surukleniyor = true;

        // Fizi�i devre d��� b�rak ki biz kontrol edelim
        rb.isKinematic = true;

        // Farenin ilk konumunu al
        sonFarePozisyonu = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Fare bas�l� tutulup s�r�klendi�i s�rece her frame �al���r
    void OnMouseDrag()
    {
        // Farenin mevcut konumunu d�nya koordinatlar�na �evir
        farePozisyonu = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        farePozisyonu.z = 0; // 2D'de Z eksenini s�f�rla

        // Nesnenin pozisyonunu farenin pozisyonuna e�itle
        transform.position = farePozisyonu;

        // F�rlatma vekt�r�n� hesapla (mevcut konum - bir �nceki konum)
        // Bu bize farenin son andaki hareket y�n�n� ve h�z�n� verir
        firlatmaVektoru = farePozisyonu - sonFarePozisyonu;

        // Bir sonraki frame'de kullanmak i�in mevcut konumu "son konum" olarak kaydet
        sonFarePozisyonu = farePozisyonu;
    }

    void OnMouseUp()
    {
        // Fare b�rak�ld���nda s�r�kleme modunu kapat
        surukleniyor = false;

        // Fizi�i tekrar aktif et
        rb.isKinematic = false;

        // F�rlatma i�lemini yap
        // Hesaplad���m�z vekt�r� (h�z�) Time.deltaTime'a b�lerek anl�k bir h�za d�n��t�r�yoruz
        // ve f�rlatma g�c�yle �arp�yoruz.
        rb.linearVelocity = firlatmaVektoru / Time.deltaTime * firlatmaGucu;
    }
}