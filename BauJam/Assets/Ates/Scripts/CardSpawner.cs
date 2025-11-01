using UnityEngine;
using System.Collections.Generic; // Listeler için gerekli

public class CardSpawner : MonoBehaviour
{
    [Header("Veri Gereksinimleri")]
    [Tooltip("Oluşturulacak TÜM kartların SO listesi.")]
    public List<PainSO> availablePainSOs; // Tek SO yerine SO listesi
    
    [Header("UI Gereksinimleri")]
    [Tooltip("Oluşturulacak Kart Prefab'i (CardDisplay script'i içeren).")]
    public GameObject cardPrefab;

    [Tooltip("Kartların oluşturulacağı Canvas/Parent Transform'u (Genellikle bir Layout Grubu).")]
    public Transform spawnParent; 
    
    [Header("Oluşturulan Kartlar")]
    private List<GameObject> currentCardInstances = new List<GameObject>();

    void Start()
    {
        // Oyun başladığında tüm kartları listeden oluştur.
        SpawnAllCards();
    }

    public void SpawnAllCards()
    {
        if (cardPrefab == null || spawnParent == null)
        {
            Debug.LogError("Kart oluşturma gereksinimleri eksik! Prefab veya Parent atanmamış.");
            return;
        }

        if (availablePainSOs.Count == 0)
        {
             Debug.LogWarning("PainSO listesi boş. Hiç kart oluşturulmadı.");
             return;
        }

        // Önceki kartları temizle (Eğer metod tekrar çağrılabilirse)
        foreach (var card in currentCardInstances)
        {
            Destroy(card);
        }
        currentCardInstances.Clear();


        // Listedeki her bir SO için döngü başlat
        foreach (PainSO cardData in availablePainSOs)
        {
            if (cardData != null)
            {
                // 1. Prefab'i oluştur
                GameObject newCard = Instantiate(cardPrefab, spawnParent);
                currentCardInstances.Add(newCard);
                
                // 2. CardDisplay script'ini al
                CardDisplay display = newCard.GetComponent<CardDisplay>();

                if (display != null)
                {
                    // 3. SO verilerini Prefab'e yükle
                    display.DisplayCard(cardData);
                }
                else
                {
                    Debug.LogError("CardPrefab üzerinde CardDisplay script'i bulunamadı!");
                }
            }
        }
        Debug.Log($"Toplam {currentCardInstances.Count} adet kart oluşturuldu.");
    }
}