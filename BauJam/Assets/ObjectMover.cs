using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    // Bu deðiþken, objenin hareket hýzýný belirler.
    // Spawner script'i tarafýndan bu deðer deðiþtirilecek.
    public float speed = 5f;

    void Update()
    {
        // objeyi her frame'de saða doðru (Vector2.right) hareket ettirir.
        // Time.deltaTime ile çarpýlarak hareketin bilgisayar hýzýndan baðýmsýz olmasý saðlanýr.
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}