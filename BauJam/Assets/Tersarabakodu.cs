using UnityEngine;

public class Tersarabakodu : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // objeyi her frame'de sa�a do�ru (Vector2.right) hareket ettirir.
        // Time.deltaTime ile �arp�larak hareketin bilgisayar h�z�ndan ba��ms�z olmas� sa�lan�r.
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

}
