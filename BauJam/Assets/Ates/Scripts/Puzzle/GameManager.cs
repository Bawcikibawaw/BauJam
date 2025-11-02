using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 
using System.Collections;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Rastgele Hedefler")]
    [Tooltip("NPC'nin rastgele seçilerek gidebileceği tüm hedef noktalarının listesi.")]
    public List<PathTarget> availableTargets = new List<PathTarget>();
    
    private HashSet<PathTarget> usedTargets = new HashSet<PathTarget>();
    
    public PathTarget finalDestinationTarget; 
    
    private bool finalDestinationReached = false; // Final hedefine ulaşıldı mı?

    [Header("Otomatik Tetikleme Ayarları")]
    [Tooltip("NPC bir hedefe ulaştıktan sonra diğerini tetiklemeden önceki bekleme süresi (saniye).")]
    public float timeBetweenMovements = 3f; // <-- YENİ DELAY DEĞİŞKENİ
    
    [Tooltip("NPC'nin hareket etmesi için ne kadar beklenecek (Min/Max saniye).")]
    public Vector2 randomDelayRange = new Vector2(3f, 8f); // Rastgele İlk Bekleme (Artık kullanılmıyor, ama ayar için bırakıldı)

    // NPC'ye sadece hedef konumu gönderen event
    public event Action<Vector3> OnNPCWalkToLocation; 
    
    // NPC'nin şu an hareket edip etmediğini takip etmeliyiz
    public bool isNPCMoving = false;

    public int mana = 0;

    private void Awake()
    {
        // Singleton kontrolü
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    private void Start()
    {
        Debug.Log("GAME MANAGER START BAŞLADI. RandomMovementCycle başlatılıyor..."); 
        // Oyun başladığında rastgele hareket döngüsünü başlat
        StartCoroutine(RandomMovementCycle());
    }
    
    public void TriggerNPCWalk(Vector3 targetPosition)
    {
        // LOG EKLE: Olayın tetiklenip tetiklenmediğini kontrol eder
        Debug.Log($"NPC Yürüme Olayı TETİKLENDİ. Abone sayısı: {OnNPCWalkToLocation?.GetInvocationList().Length ?? 0}"); 
        
        if (OnNPCWalkToLocation != null)
        {
            OnNPCWalkToLocation.Invoke(targetPosition);
            isNPCMoving = true; // Hareket başladı
            Debug.Log($"NPC yürüme olayı BAŞARILI İLE GÖNDERİLDİ. Hedef: {targetPosition}"); 
        }
    }
    
    
    private IEnumerator RandomMovementCycle()
    {
        // Başlangıçta bir bekleme (opsiyonel)
        yield return new WaitForSeconds(1f);

        while (true) // Oyun çalıştığı sürece döngü devam eder
        {
            // 1. NPC'nin hareket etmesinin bitmesini bekle
            yield return new WaitUntil(() => !isNPCMoving); // isNPCMoving == false olana kadar bekle

            // 2. DELAY: Belirlenen süre kadar bekle
            Debug.Log($"NPC durdu. Yeni hareket için {timeBetweenMovements:F2} saniye bekleniyor...");
            yield return new WaitForSeconds(timeBetweenMovements); // <-- DELAY BURADA!

            // 3. Rastgele hedef seç ve hareketi tetikle
            SelectAndTriggerRandomTarget();
        }
    }

    // Bu fonksiyon artık Coroutine tarafından çağrılıyor
    public void SelectAndTriggerRandomTarget()
    {
        // YENİ KONTROL: Eğer tüm rastgele hedefler kullanıldıysa
        if (usedTargets.Count >= availableTargets.Count)
        {
            Debug.Log("Tüm rastgele hedefler tamamlandı. Final hedefine geçiş tetikleniyor.");
            StartFinalMovement(); 
            return; 
        }

        // 1. Kullanılmamış hedeflerin listesini hazırla
        List<PathTarget> remainingTargets = new List<PathTarget>();
        foreach(var target in availableTargets)
        {
            if (!usedTargets.Contains(target))
            {
                remainingTargets.Add(target);
            }
        }

        if (remainingTargets.Count == 0) return; 

        // 2. Kalan hedefler arasından rastgele birini seç
        int randomIndex = Random.Range(0, remainingTargets.Count);
        PathTarget selectedTarget = remainingTargets[randomIndex];

        if (selectedTarget != null && isNPCMoving == false)
        {
            TriggerNPCWalk(selectedTarget.transform.position);
        
            // Hareketi başlattıktan sonra hedefi kullanılanlar listesine ekle
            usedTargets.Add(selectedTarget);
            Debug.Log($"Hedef kullanıldı: {selectedTarget.name}. Kalan Hedef Sayısı: {remainingTargets.Count - 1}");
        }
    }
    
    public void BuyCard(PainSO cardToBuy)
    {
        // Önceki iş mantığı...
        if (mana >= cardToBuy.manaRequirement)
        {
            mana -= cardToBuy.manaRequirement;
            Debug.Log($"SATIN ALMA BAŞARILI: Kalan Mana: {mana}");
        }
        else
        {
            Debug.Log("Yeterli Mana yok.");
        }
    }
    
    private void StartFinalMovement()
    {
        StopAllCoroutines(); // Rastgele döngüyü durdur
        
        if (finalDestinationTarget == null)
        {
            Debug.LogError("Final hedefi atanmamış! Sahne geçişi direk tetikleniyor.");
            DoSceneChange();
            return;
        }

        // Final hedefine yürüme olayını tetikle (isNPCMoving zaten false olmalı)
        TriggerNPCWalk(finalDestinationTarget.transform.position);
        finalDestinationReached = true; 

        // NPC'nin son hedefine ulaşmasını bekleyen Coroutine'i başlat
        StartCoroutine(WaitForFinalMovementCompletion());
    }
    
    private IEnumerator WaitForFinalMovementCompletion()
    {
        // isNPCMoving'in tekrar false olmasını bekle (yani NPC durdu)
        yield return new WaitUntil(() => isNPCMoving == false); 

        Debug.Log("NPC son hedefine ulaştı. Sahne Değiştiriliyor...");
        DoSceneChange();
    }
    
    private void DoSceneChange()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}