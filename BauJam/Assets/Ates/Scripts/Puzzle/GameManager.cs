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
    public List<PathTarget> availableTargets = new List<PathTarget>();
    
    private HashSet<PathTarget> usedTargets = new HashSet<PathTarget>(); 
    
    public PathTarget finalDestinationTarget; 
    
    private bool finalDestinationReached = false; 

    [Header("Otomatik Tetikleme Ayarları")]
    public float timeBetweenMovements = 5f; 
    public float timeAfterRandomTargetReached = 20f; // MİNİGAME SÜRESİ
    public Vector2 randomDelayRange = new Vector2(3f, 8f); 

    public event Action<PathTarget> OnNPCWalkToLocation; 
    public event Action<bool> OnMinigameStatusChange; 

    public bool isNPCMoving = false;
    public int mana = 0;
    
    private bool firstRun = true; 
    private bool minigameIsPending = false; 

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
            
            // --- Normal Minigame Beklemesi (Son hedeften sonra da çalışmalı) ---
            
            // Eğer tüm hedefler tamamlandıysa, Minigame zaten oynanmış/oynanıyor olmalı.
            // Bu kontrolü burada yapmıyoruz, aşağıda SelectAndTriggerRandomTarget'tan sonra yapacağız.

            float waitTime = timeAfterRandomTargetReached; 
            
            minigameIsPending = true; 
            
            // PUZZLE'I AÇ KOMUTU
            OnMinigameStatusChange?.Invoke(true); 
            Debug.Log("Minigame AÇILDI. 20 saniye süre başladı.");
            
            Debug.Log($"NPC durdu. Yeni hareket için {waitTime:F2} saniye bekleniyor.");
            
            // Bekle
            yield return new WaitForSeconds(waitTime); 
            
            // Eğer süre dolduysa (ve SuccessTrigger tarafından kesilmediyse)
            if (minigameIsPending)
            {
                 OnMinigameStatusChange?.Invoke(false);
                 Debug.Log("Minigame KAPANDI. Süre doldu.");
                 minigameIsPending = false;
            }

            // 3. Kontrol ve sonraki hareketi tetikle
            
            // Eğer tüm hedefler bittiyse (son Minigame oynandı), Final'a git
            if (usedTargets.Count >= availableTargets.Count)
            {
                StartFinalMovement();
                yield break; 
            }
            
            // Aksi halde bir sonraki rastgele hedefi seç
            SelectAndTriggerRandomTarget();
        }
    }

    // Bu metot sadece rastgele hedefler bitmediği sürece yeni hedef seçer.
    public void SelectAndTriggerRandomTarget()
    {
        // Hedefler bittiyse, bir şey yapma (Final RandomMovementCycle içinde tetiklenecek)
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
    
    public void BuyCard(PainSO cardToBuy)
    {
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
        if (finalDestinationTarget == null)
        {
            Debug.LogError("Final hedefi atanmamış! Sahne geçişi direk tetikleniyor.");
            DoSceneChange();
            return;
        }
        
        Debug.Log("Tüm rastgele hedefler tamamlandı. Final hedefine yönlendiriliyor.");
        
        TriggerNPCWalk(finalDestinationTarget);
        finalDestinationReached = true; 

        StartCoroutine(WaitForFinalMovementCompletion());
    }
    
    private IEnumerator WaitForFinalMovementCompletion()
    {
        yield return new WaitUntil(() => isNPCMoving == false); 
        
        Debug.Log("NPC son hedefine ulaştı. Kısa bir süre bekleniyor...");
        yield return new WaitForSeconds(1f); 

        Debug.Log("Sahne Değiştiriliyor...");
        DoSceneChange();
    }
    
    // Minigame kazanıldığında bekleme süresini atlamak için çağrılır
    public void MinigameSuccessTrigger()
    {
        StopAllCoroutines(); 

        // Eğer Minigame beklemedeyse (süre dolmadan başarılı oldu), kapatma komutu gönderilir.
        if (minigameIsPending)
        {
            OnMinigameStatusChange?.Invoke(false);
            Debug.Log("Minigame Başarıyla Tamamlandı. Bekleme atlandı.");
            minigameIsPending = false;
        }
    
        // HEDEFLER BİTTİYSE, FİNAL HAREKETİNİ BAŞLAT
        if (usedTargets.Count >= availableTargets.Count)
        {
            StartFinalMovement();
            return; 
        }

        // Bir sonraki rastgele hedefi seç
        SelectAndTriggerRandomTarget();
        
        // RandomMovementCycle'ı yeniden başlat
        StartCoroutine(RandomMovementCycle());
    }
    
    private void DoSceneChange()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}