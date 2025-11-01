using UnityEngine;
using UnityEngine.EventSystems; // Event sistemini kullanmak için bu kütüphane gerekli

public class SürükleBırak : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Sürükleme başladığında çalışacak fonksiyon
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Sürükleme başladı.");
        originalPosition = rectTransform.anchoredPosition; // Başlangıç pozisyonunu kaydet
        canvasGroup.alpha = 0.6f; // Sürüklerken biraz şeffaf yap (görsel efekt)
        canvasGroup.blocksRaycasts = false; // Sürüklenen objenin altındaki objeleri algılamasını sağlar
    }

    // Sürükleme devam ederken her frame çalışacak fonksiyon
    public void OnDrag(PointerEventData eventData)
    {
        // Kareyi mouse pozisyonuna getir
        // eventData.delta, mouse'un son kareden bu yana ne kadar hareket ettiğini verir
        // Canvas'ın scale faktörüne bölerek farklı ekran çözünürlüklerinde doğru çalışmasını sağlarız
        rectTransform.anchoredPosition += eventData.delta / transform.root.localScale.x;
    }

    // Sürükleme bittiğinde (mouse tuşu bırakıldığında) çalışacak fonksiyon
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Sürükleme bitti.");
        canvasGroup.alpha = 1f; // Şeffaflığı geri al
        canvasGroup.blocksRaycasts = true; // Raycast engellemesini geri aç

        // Eğer obje bir BırakmaAlanı'na bırakılmadıysa eski pozisyonuna dönsün
        // Bu kontrol BırakmaAlanı script'i tarafından yönetilecek. Eğer o script pozisyonu
        // değiştirmezse, obje bırakıldığı yerde kalır. İstersen eski yerine dönmesini de sağlayabiliriz.
    }
}