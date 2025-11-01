using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 
using System.Collections;
using UnityEngine.SceneManagement; // Coroutine için gerekli

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
    [Tooltip("NPC'nin hareket etmesi için ne kadar beklenecek (Min/Max saniye).")]
    public Vector2 randomDelayRange = new Vector2(3f, 8f); // Örn: 3 ila 8 saniye arası

    // NPC'ye sadece hedef konumu gönderen event
    public event Action<Vector3> OnNPCWalkToLocation; 
    
    // NPC'nin şu an hareket edip etmediğini takip etmeliyiz
    public bool isNPCMoving = false;

    public int mana = 0;

    private void Awake()
    {
        // Önce Singleton kontrolü yapılır
        if (Instance != null && Instance != this)
        {
            // Eğer sahneden yeni yüklenen bir GameManager varsa, onu yok et.
            Destroy(gameObject);
        }
        else
        {
            // Eğer bu, tek ve ilk GameManager ise:
            Instance = this;
            
            // 🚨 BU SATIRI EKLEYİN: Objeyi sahneler arası taşır ve yok edilmesini engeller.
            DontDestroyOnLoad(gameObject); 
        }
    }

    private void Start()
    {
        // 🚨 Oyun başladığında rastgele hareket döngüsünü başlat
        Debug.Log("GAME MANAGER START BAŞLADI. RandomMovementCycle başlatılıyor..."); // LOG EKLE
        StartCoroutine(RandomMovementCycle());
    }
    
    public void TriggerNPCWalk(Vector3 targetPosition)
    {
        Debug.Log($"NPC Yürüme Olayı TETİKLENDİ. Abone sayısı: {OnNPCWalkToLocation?.GetInvocationList().Length ?? 0}");
        
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
        // YENİ KONTROL: Eğer tüm rastgele hedefler kullanıldıysa (ve final hareket başlatılmadıysa)
        if (usedTargets.Count >= availableTargets.Count)
        {
            Debug.Log("Tüm rastgele hedefler tamamlandı. Final hedefine geçiş tetikleniyor.");
        
            // Final hareketini başlat
            StartFinalMovement(); // <-- Önceki direkt sahne değiştirme yerine, final hareketini başlat
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

        if (remainingTargets.Count == 0) return; // (Bu satır aslında yukarıdaki kontrolle gereksizleşir ama kalsın)

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
    }    public void BuyCard(PainSO cardToBuy)
    {
        // 1. Gereksinim Kontrolü
        if (mana >= cardToBuy.manaRequirement)
        {
            // 2. Satın Alma Başarılı
            mana -= cardToBuy.manaRequirement;
            Debug.Log($"SATIN ALMA BAŞARILI: Kalan Mana: {mana}");
            Debug.Log("SİKKEEEEEEEEEMMMMMMMMMM");
            // Satın alınan kartın etkisini burada uygula (Örn: Hasar verme, buff verme vb.)
        }
        else
        {
            // 3. Satın Alma Başarısız
            Debug.Log("SİKEM");
        }
    }
    
    private void CheckForSceneChange()
    {
         int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
         SceneManager.LoadScene(nextSceneIndex);
    }
    
    private void StartFinalMovement()
    {
        // 1. Rastgele döngüyü durdur
        StopAllCoroutines(); 
        
        if (finalDestinationTarget == null)
        {
            Debug.LogError("Final hedefi atanmamış! Sahne geçişi direk tetikleniyor.");
            DoSceneChange();
            return;
        }

        if (isNPCMoving)
        {
            // 2. Final hedefine yürüme olayını tetikle
            TriggerNPCWalk(finalDestinationTarget.transform.position);
            finalDestinationReached = true; 

            // 3. NPC'nin son hedefine ulaşmasını bekleyen Coroutine'i başlat
            StartCoroutine(WaitForFinalMovementCompletion());
        }
    }
    
    private IEnumerator WaitForFinalMovementCompletion()
    {
        // isNPCMoving'in tekrar false olmasını bekle (yani NPC durdu)
        yield return new WaitUntil(() => isNPCMoving == false); 

        // Not: Eğer NPC bu noktada durduysa ve bu duruş final hedefindeyse, sahne değiştir.
        // Konum kontrolü opsiyoneldir, isNPCMoving=false yeterli olmalı.
        
        Debug.Log("NPC son hedefine ulaştı. Sahne Değiştiriliyor...");
        DoSceneChange();
    }
    
    private void DoSceneChange()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        SceneManager.LoadScene(nextSceneIndex);
    }
}