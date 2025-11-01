using UnityEngine;

public class TriangleController : MonoBehaviour
{
    // Bu fonksiyon, bu script'in eklendi�i obje (��gen)
    // ba�ka bir fiziksel objeyle �arp��t���nda Unity taraf�ndan otomatik olarak �al��t�r�l�r.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �arpt���m�z objenin etiketinin (Tag) "Square" olup olmad���n� kontrol ediyoruz.
        if (collision.gameObject.CompareTag("Square"))
        {
            // E�er etiket "Square" ise, �arpt���m�z o objeyi (yani kareyi) yok et.
            Destroy(collision.gameObject);
        }
    }
}