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
    
    // PathTarget tipinde Hash Set
    private HashSet<PathTarget> usedTargets = new HashSet<PathTarget>(); 
    
    public PathTarget finalDestinationTarget; 
    
    private bool finalDestinationReached = false; 

    [Header("Otomatik Tetikleme Ayarları")]
    [Tooltip("NPC bir hedefe ulaştıktan sonra diğerini tetiklemeden önceki bekleme süresi (SABİT DÖNGÜ BEKLEMESİ - Örn: 5s).")]
    public float timeBetweenMovements = 5f; 
    
    [Tooltip("NPC bir RASTGELE hedefe ulaştığında Minigame'in açık kalma süresi (20 saniye).")]
    public float timeAfterRandomTargetReached = 20f; // MİNİGAME SÜRESİ

    [Tooltip("NPC'nin hareket etmesi için ne kadar beklenecek (Min/Max saniye).")]
    public Vector2 randomDelayRange = new Vector2(3f, 8f); // Artık kullanılmıyor

    public event Action<PathTarget> OnNPCWalkToLocation; 
    public event Action<bool> OnMinigameStatusChange; 
    public event Action OnCardCountUpdated; // Kart sistemi için
    
    [Header("Oyun Durumu")]
    public int cardsPurchasedCount = 0; // Kart sistemi için
    public bool isNPCMoving = false;
    public int mana = 0;
    
    private bool firstRun = true; 
    private bool minigameIsPending = false; 
    
    // YENİ EKLENTİ: Seviye Geçişi İçin
    [Header("Seviye Yönetimi")]
    public int nextLevelToLoad = 1; // Başlangıç seviyesi
    public string mainMenuSceneName = "MainMenu"; // Ana Menü sahne adı

    private void Awake()
    {
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
        StartCoroutine(RandomMovementCycle());
    }
    
    public void TriggerNPCWalk(PathTarget target)
    {
        if (OnNPCWalkToLocation != null)
        {
            OnNPCWalkToLocation.Invoke(target); 
            isNPCMoving = true; 
            Debug.Log($"NPC yürüme olayı BAŞARILI İLE GÖNDERİLDİ. Hedef: {target.transform.position}"); 
        }
    }
    
    
    private IEnumerator RandomMovementCycle()
    {
        yield return new WaitForSeconds(1f);

        while (true) 
        {
            // 1. NPC'nin hareket etmesinin bitmesini bekle
            yield return new WaitUntil(() => !isNPCMoving); 
            
            if (finalDestinationReached) 
            {
                yield break; 
            }
            
            // 🚨 İLK ÇALIŞTIRMA KONTROLÜ
            if (firstRun)
            {
                firstRun = false; 
                SelectAndTriggerRandomTarget();
                continue; 
            }

            // --- Minigame/Wait Mantığı Başlar ---
            
            // Eğer tüm hedefler tamamlandıysa, Final'a geç (Bu, Minigame oynandıktan sonra olur)
            if (usedTargets.Count >= availableTargets.Count)
            {
                // Artık StartFinalMovement() yerine seviye tamamlandı komutu verilir
                CompleteCurrentLevel(); 
                yield break; 
            }
            
            // --- Normal Minigame Beklemesi ---
            
            float waitTime = timeAfterRandomTargetReached; 
            
            minigameIsPending = true; 
            
            OnMinigameStatusChange?.Invoke(true); 
            Debug.Log("Minigame AÇILDI. 20 saniye süre başladı.");
            
            Debug.Log($"NPC durdu. Yeni hareket için {waitTime:F2} saniye bekleniyor.");
            
            yield return new WaitForSeconds(waitTime); 
            
            if (minigameIsPending)
            {
                 OnMinigameStatusChange?.Invoke(false);
                 Debug.Log("Minigame KAPANDI. Süre doldu.");
                 minigameIsPending = false;
            }

            // 3. Rastgele hedef seç ve hareketi tetikle
            SelectAndTriggerRandomTarget();
        }
    }

    // 🚨 GERİ YÜKLENEN METOT: Rastgele hedef seçimi (önceki mantık korundu)
    public void SelectAndTriggerRandomTarget()
    {
        // Hedefler bittiyse, bir şey yapma 
        if (usedTargets.Count >= availableTargets.Count)
        {
            return; 
        }

        List<PathTarget> remainingTargets = new List<PathTarget>();
        foreach(var target in availableTargets)
        {
            if (!usedTargets.Contains(target))
            {
                remainingTargets.Add(target);
            }
        }

        if (remainingTargets.Count == 0) return; 

        int randomIndex = Random.Range(0, remainingTargets.Count);
        PathTarget selectedTarget = remainingTargets[randomIndex];

        if (selectedTarget != null && isNPCMoving == false)
        {
            TriggerNPCWalk(selectedTarget);
        
            usedTargets.Add(selectedTarget);
            Debug.Log($"Hedef kullanıldı: {selectedTarget.name}. Kalan Hedef Sayısı: {remainingTargets.Count - 1}");
        }
    }
    
    // --- KART SİSTEMİ METOTLARI (KORUNDU) ---
    
    public bool CanPurchaseFinalCard(int totalCardsInShop)
    {
        return cardsPurchasedCount >= (totalCardsInShop - 1); 
    }

    public void BuyCard(PainSO cardToBuy)
    {
        if (mana >= cardToBuy.manaRequirement)
        {
            mana -= cardToBuy.manaRequirement;
            cardsPurchasedCount++; 
            
            Debug.Log($"SATIN ALMA BAŞARILI: Kalan Mana: {mana}. Toplam Kart: {cardsPurchasedCount}");

            OnCardCountUpdated?.Invoke(); 
        }
        else
        {
            Debug.Log("Yeterli Mana yok.");
        }
    }
    
    // --- FİNAL / SEVİYE GEÇİŞ METOTLARI ---

    /// <summary>
    /// Bir seviye (LVL1, LVL2 vb.) bittiğinde çağrılır.
    /// Bir sonraki seviye indeksini kaydeder ve Menü'ye döner.
    /// </summary>
    public void CompleteCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // 1. Bir sonraki seviyenin indeksini kaydet
        nextLevelToLoad = currentSceneIndex + 1;

        Debug.Log($"Seviye Tamamlandı. Bir sonraki yüklenecek seviye indexi: {nextLevelToLoad}. Menü'ye dönülüyor.");

        // 2. Menü Sahnesini yükle
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    /// <summary>
    /// Ana Menü'deki 'Devam Et' butonuna atanacak metot.
    /// nextLevelToLoad değişkeninde kayıtlı olan seviyeyi yükler.
    /// </summary>
    public void LoadNextTrackedLevel()
    {
        if (nextLevelToLoad < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Menü butonu tıklandı. Yükleniyor: Index {nextLevelToLoad}");
            SceneManager.LoadScene(nextLevelToLoad);
        }
        else
        {
            Debug.LogWarning("Tüm seviyeler tamamlandı! Oyun sonu akışına geçiliyor.");
        }
    }
    
    // NOT: NPC'nin son hedefine ulaştığında Minigame sonrası çağrılmalıdır.
    private void StartFinalMovement()
    {
        Debug.LogWarning("NPC Final Destination'a yönlendirildi. Artık CompleteCurrentLevel() çağrılmalı.");
        
        // Final Destination yerine Level'ı bitirme komutu verilir.
        CompleteCurrentLevel();
    }
    
    public void MinigameSuccessTrigger()
    {
        StopAllCoroutines(); 

        if (minigameIsPending)
        {
            OnMinigameStatusChange?.Invoke(false);
            Debug.Log("Minigame Başarıyla Tamamlandı. Bekleme atlandı.");
            minigameIsPending = false;
        }
    
        if (usedTargets.Count >= availableTargets.Count)
        {
            // Eğer tüm hedefler bittiyse, seviyeyi tamamla ve Menü'ye dön
            CompleteCurrentLevel();
            return; 
        }

        SelectAndTriggerRandomTarget();
        StartCoroutine(RandomMovementCycle());
    }
}