using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    // Bu de�i�ken, objenin hareket h�z�n� belirler.
    // Spawner script'i taraf�ndan bu de�er de�i�tirilecek.
    public float speed = 5f;

    void Update()
    {
        // objeyi her frame'de sa�a do�ru (Vector2.right) hareket ettirir.
        // Time.deltaTime ile �arp�larak hareketin bilgisayar h�z�ndan ba��ms�z olmas� sa�lan�r.
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}