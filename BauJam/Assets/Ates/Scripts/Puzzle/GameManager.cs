using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 
using System.Collections; // Coroutine için gerekli

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Rastgele Hedefler")]
    [Tooltip("NPC'nin rastgele seçilerek gidebileceği tüm hedef noktalarının listesi.")]
    public List<PathTarget> availableTargets = new List<PathTarget>();

    [Header("Otomatik Tetikleme Ayarları")]
    [Tooltip("NPC'nin hareket etmesi için ne kadar beklenecek (Min/Max saniye).")]
    public Vector2 randomDelayRange = new Vector2(3f, 8f); // Örn: 3 ila 8 saniye arası

    // NPC'ye sadece hedef konumu gönderen event
    public event Action<Vector3> OnNPCWalkToLocation; 
    
    // NPC'nin şu an hareket edip etmediğini takip etmeliyiz
    public bool isNPCMoving = false;

    public int mana = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // 🚨 Oyun başladığında rastgele hareket döngüsünü başlat
        StartCoroutine(RandomMovementCycle());
    }
    
    public void TriggerNPCWalk(Vector3 targetPosition)
    {
        if (OnNPCWalkToLocation != null)
        {
            OnNPCWalkToLocation.Invoke(targetPosition);
            isNPCMoving = true; // Hareket başladı
            Debug.Log($"NPC yürüme olayı tetiklendi. Hedef: {targetPosition}"); 
        }
    }

    
    private IEnumerator RandomMovementCycle()
    {
        while (true) // Oyun çalıştığı sürece döngü devam eder
        {
            // 1. NPC'nin hareket etmesinin bitmesini bekle
            yield return new WaitUntil(() => isNPCMoving == false);

            // 2. Rastgele bekleme süresi kadar bekle (Örn: 3 ila 8 saniye)
            float waitTime = Random.Range(randomDelayRange.x, randomDelayRange.y);
            Debug.Log($"NPC durdu. Yeni hareket için {waitTime:F2} saniye bekleniyor...");
            yield return new WaitForSeconds(waitTime);

            // 3. Rastgele hedef seç ve hareketi tetikle
            SelectAndTriggerRandomTarget();
        }
    }

    // Bu fonksiyon artık Coroutine tarafından çağrılıyor
    public void SelectAndTriggerRandomTarget()
    {
        if (availableTargets.Count == 0)
        {
            Debug.LogWarning("Hedef listesi boş. Rastgele tetikleme yapılamıyor.");
            return;
        }

        int randomIndex = Random.Range(0, availableTargets.Count);
        PathTarget selectedTarget = availableTargets[randomIndex];

        if (selectedTarget != null && isNPCMoving == false)
        {
            TriggerNPCWalk(selectedTarget.transform.position);
        }
    }
    
    public void BuyCard(PainSO cardToBuy)
    {
        // 1. Gereksinim Kontrolü
        if (mana >= cardToBuy.manaRequirement)
        {
            // 2. Satın Alma Başarılı
            mana -= cardToBuy.manaRequirement;
            Debug.Log($"SATIN ALMA BAŞARILI: Kalan Mana: {mana}");
            
            // Satın alınan kartın etkisini burada uygula (Örn: Hasar verme, buff verme vb.)
        }
        else
        {
            // 3. Satın Alma Başarısız
            Debug.LogWarning($"SATIN ALMA BAŞARISIZ: Yeterli Mana yok. Gereken: {cardToBuy.manaRequirement}, Mevcut: {mana}");
        }
    }
}