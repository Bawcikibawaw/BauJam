using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    // Bu script'in eklendiði objenin collider'ýna baþka bir collider çarptýðý anda bu fonksiyon tetiklenir.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Tag (etiket) kontrolü yapmadan, çarpan objeyi (collision.gameObject) doðrudan yok et.
        Destroy(collision.gameObject);
    }
}