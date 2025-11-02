using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    // Bu script'in eklendi�i objenin collider'�na ba�ka bir collider �arpt��� anda bu fonksiyon tetiklenir.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Tag (etiket) kontrol� yapmadan, �arpan objeyi (collision.gameObject) do�rudan yok et.
        Destroy(collision.gameObject);
    }
}