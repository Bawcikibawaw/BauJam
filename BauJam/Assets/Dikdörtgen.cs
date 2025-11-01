using UnityEngine;
using UnityEngine.EventSystems; // Event sistemini kullanmak için bu kütüphane gerekli

public class BırakmaAlanı : MonoBehaviour, IDropHandler
{
    // Üzerine bir obje bırakıldığında çalışacak fonksiyon
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Üzerime bir obje bırakıldı!");

        // Eğer bırakılan bir obje varsa (eventData.pointerDrag null değilse)
        if (eventData.pointerDrag != null)
        {
            // Bırakılan objenin RectTransform'unu al ve pozisyonunu bu alanın pozisyonu yap
            RectTransform droppedObjectRectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
            droppedObjectRectTransform.position = GetComponent<RectTransform>().position;

            // İsteğe bağlı: Başarılı bir bırakma işleminden sonra sürüklemeyi devre dışı bırakabilirsin
            // eventData.pointerDrag.GetComponent<SürükleBırak>().enabled = false;

            Debug.Log("Obje başarıyla yerleştirildi.");
        }
    }
}