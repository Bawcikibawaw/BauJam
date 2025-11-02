using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    // Inspector'da atayacağımız hedef GameObject.
    // Bu nesne tıklandığında etkinleştirilecek.
    public GameObject targetObject;

    // Fare sol tuşuna basıldığında (veya mobil cihazlarda dokunulduğunda)
    // bu nesnenin Collider'ı üzerinde tetiklenir.
    void OnMouseDown()
    {
        // Hedef nesnenin atanıp atanmadığını kontrol et
        if (targetObject != null)
        {
            // Tıklama ile nesnenin aktif/pasif durumunu değiştir (toggle)
            bool newState = !targetObject.activeSelf;
            targetObject.SetActive(newState);

            Debug.Log(targetObject.name + " durumu " + (newState ? "Aktif" : "Pasif") + " olarak değiştirildi.");
            
            // Eğer sadece tek seferlik aktif olmasını istiyorsanız
            // aşağıdaki satırları kullanabilirsiniz:
            // targetObject.SetActive(true);
            // this.enabled = false; // Betiği devre dışı bırak
        }
    }

    // Ek Not: OnMouseDown'ın çalışması için bu nesnenin
    // Ignore Raycast katmanında olmadığından emin olun.
}