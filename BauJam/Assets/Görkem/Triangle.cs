using UnityEngine;

public class TriangleController : MonoBehaviour
{
    // Bu fonksiyon, bu script'in eklendiði obje (Üçgen)
    // baþka bir fiziksel objeyle çarpýþtýðýnda Unity tarafýndan otomatik olarak çalýþtýrýlýr.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Çarptýðýmýz objenin etiketinin (Tag) "Square" olup olmadýðýný kontrol ediyoruz.
        if (collision.gameObject.CompareTag("Square"))
        {
            // Eðer etiket "Square" ise, çarptýðýmýz o objeyi (yani kareyi) yok et.
            Destroy(collision.gameObject);
        }
    }
}