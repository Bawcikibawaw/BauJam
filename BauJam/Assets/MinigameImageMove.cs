using UnityEngine;
using UnityEngine.UI;

public class MinigameImageMover : MonoBehaviour
{
    [Tooltip("Resmin sa�a do�ru hareket h�z�.")]
    public float speed = 200f;

    [Tooltip("Her t�klamada h�z�n ne kadar azalaca��.")]
    public float slowDownAmount = 40f;

    private RectTransform rectTransform;
    private RectTransform parentPanelRect;
    private bool isMoving = true;

    void Start()
    {
        // Gerekli bile�enleri al�yoruz
        rectTransform = GetComponent<RectTransform>();
        // Bu script'in ba�l� oldu�u objenin ebeveyni olan Paneli al�yoruz
        parentPanelRect = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        // E�er hareket etmiyorsa, Update fonksiyonundan ��k
        if (!isMoving)
        {
            return;
        }

        // Resmi sa�a do�ru hareket ettir
        // anchoredPosition, ebeveyninin (Panel) i�indeki konumunu ifade eder
        rectTransform.anchoredPosition += new Vector2(speed * Time.deltaTime, 0);

        // Panelin sa��na ula�t� m� diye kontrol et
        float imageRightEdge = rectTransform.anchoredPosition.x + (rectTransform.rect.width / 2);
        float panelRightEdge = parentPanelRect.rect.width / 2;

        if (imageRightEdge >= panelRightEdge)
        {
            // E�er panelin sonuna ula�t�ysa, durdur ve kaybettin say.
            isMoving = false;
            Debug.Log("Kaybettin! Resim panelin sonuna ula�t�.");
        }
    }

    // Bu fonksiyon t�klama ile tetiklenecek
    public void OnClicked()
    {
        // Sadece hareket ediyorsa t�klamalar i�e yaras�n
        if (isMoving)
        {
            // H�z� azalt
            speed -= slowDownAmount;

            // E�er h�z 0 veya alt�na d��erse, oyunu durdur ve kazand�n.
            if (speed <= 0)
            {
                speed = 0;
                isMoving = false;
                Debug.Log("Kazand�n! Resmi zaman�nda durdurdun.");
            }
        }
    }
}