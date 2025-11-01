using UnityEngine;
using UnityEngine.UI;

public class MinigameImageMover : MonoBehaviour
{
    [Tooltip("Resmin saða doðru hareket hýzý.")]
    public float speed = 200f;

    [Tooltip("Her týklamada hýzýn ne kadar azalacaðý.")]
    public float slowDownAmount = 40f;

    private RectTransform rectTransform;
    private RectTransform parentPanelRect;
    private bool isMoving = true;

    void Start()
    {
        // Gerekli bileþenleri alýyoruz
        rectTransform = GetComponent<RectTransform>();
        // Bu script'in baðlý olduðu objenin ebeveyni olan Paneli alýyoruz
        parentPanelRect = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        // Eðer hareket etmiyorsa, Update fonksiyonundan çýk
        if (!isMoving)
        {
            return;
        }

        // Resmi saða doðru hareket ettir
        // anchoredPosition, ebeveyninin (Panel) içindeki konumunu ifade eder
        rectTransform.anchoredPosition += new Vector2(speed * Time.deltaTime, 0);

        // Panelin saðýna ulaþtý mý diye kontrol et
        float imageRightEdge = rectTransform.anchoredPosition.x + (rectTransform.rect.width / 2);
        float panelRightEdge = parentPanelRect.rect.width / 2;

        if (imageRightEdge >= panelRightEdge)
        {
            // Eðer panelin sonuna ulaþtýysa, durdur ve kaybettin say.
            isMoving = false;
            Debug.Log("Kaybettin! Resim panelin sonuna ulaþtý.");
        }
    }

    // Bu fonksiyon týklama ile tetiklenecek
    public void OnClicked()
    {
        // Sadece hareket ediyorsa týklamalar iþe yarasýn
        if (isMoving)
        {
            // Hýzý azalt
            speed -= slowDownAmount;

            // Eðer hýz 0 veya altýna düþerse, oyunu durdur ve kazandýn.
            if (speed <= 0)
            {
                speed = 0;
                isMoving = false;
                Debug.Log("Kazandýn! Resmi zamanýnda durdurdun.");
            }
        }
    }
}