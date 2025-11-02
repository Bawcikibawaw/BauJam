using UnityEngine;

public class Tersarabakodu : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // objeyi her frame'de saða doðru (Vector2.right) hareket ettirir.
        // Time.deltaTime ile çarpýlarak hareketin bilgisayar hýzýndan baðýmsýz olmasý saðlanýr.
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

}
