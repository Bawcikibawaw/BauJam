using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BalancePuzzle : MonoBehaviour
{
    // Kazanma ve Denge Ayarları
    [Header("Kazanma ve Denge Ayarları")]
    [Tooltip("Kazanmak için nesnenin denge aralığında kalması gereken toplam süre (saniye).")]
    public float winTime = 5f; 
    
    [Tooltip("Nesnenin dengeye sayılması için kabul edilen Z açısı aralığı (Örn: 15f, yani -15 ile +15 derece arası).")]
    public float balanceTolerance = 15f; 
    
    [Tooltip("Kare bu açıyı geçtiği an oyuncu kaybeder (Örn: 90f).")]
    public float loseAngle = 90f; 

    // --- MANUEL KONTROL AYARLARI ---
    [Header("Kontrol ve Hız Ayarları")]
    [Tooltip("Dengeden uzaklaştıkça artan otomatik sapma kuvveti.")]
    public float rotationDriftForce = 40f; 
    
    [Tooltip("Oyuncunun A/D tuşları ile uygulayacağı düzeltme kuvveti (hıza etki eder).")]
    public float manualCorrectionForce = 300f;
    
    [Tooltip("Oyun başladığında uygulanacak ilk rastgele sapma kuvveti (derece/saniye).")]
    public float initialImpulse = 20f; 
    
    [Tooltip("Maksimum izin verilen açısal hız.")]
    public float maxAngularVelocity = 360f;

    // Durum Değişkenleri
    private RectTransform rectTransform;
    public float balanceTimer = 0f; 
    private bool gameFinished = false;
    private float currentAngularVelocity = 0f; // Açısal hız (derece/saniye)

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Başlangıç açısını sıfırla (Ortada başla)
        rectTransform.rotation = Quaternion.identity; 
        balanceTimer = 0f;
        
        // Rastgele bir yönde başlangıç hızı uygula (kendi kendine dönmeye başlar)
        float randomDirection = (Random.value > 0.5f) ? 1f : -1f;
        currentAngularVelocity = randomDirection * initialImpulse;
    }

    void Update()
    {
        if (gameFinished)
        {
            return;
        }

        // 1. KUVVETLERİ HESAPLA ve HIZI GÜNCELLE
        HandlePlayerInput();
        HandleAutomaticDrift();
        
        // Hızı limitle
        currentAngularVelocity = Mathf.Clamp(currentAngularVelocity, -maxAngularVelocity, maxAngularVelocity);

        // 2. ROTASYONU UYGULA (SADECE BURADA YAPILIR)
        rectTransform.Rotate(0, 0, currentAngularVelocity * Time.deltaTime); // <-- TEK ROTASYON KOMUTU

        // 3. DENGE VE KAZANMA/KAYBETME KONTROLÜ
        CheckBalanceAndWinCondition();
    }

    void HandlePlayerInput()
    {
        float correctionForce = 0f;
        
        // Oyuncunun uyguladığı kuvveti hesapla
        if (Input.GetKey(KeyCode.A))
        {
            correctionForce = manualCorrectionForce; // Sola doğru tork (Pozitif Z)
        }
        else if (Input.GetKey(KeyCode.D))
        {
            correctionForce = -manualCorrectionForce; // Sağa doğru tork (Negatif Z)
        }

        // Hıza kuvveti uygula
        currentAngularVelocity += correctionForce * Time.deltaTime;
    }

    void HandleAutomaticDrift()
    {
        float currentZ = GetCurrentAngle();
        
        // Merkezden uzaklaştıkça uygulanan kuvveti hesapla
        float driftFactor = currentZ / loseAngle; 
        float driftAmount = rotationDriftForce * driftFactor; // Geri sapma kuvveti

        // Mevcut hıza sapma kuvvetini ekle (Merkezden uzaklaşma yönünde)
        currentAngularVelocity += driftAmount * Time.deltaTime;
        
        // OPSİYONEL: Merkezden uzaklaşırken yavaşlama (Sürtünme) eklemek isterseniz:
        // currentAngularVelocity *= Mathf.Pow(0.99f, Time.deltaTime);
    }
    
    void CheckBalanceAndWinCondition()
    {
        float currentZ = GetCurrentAngle();

        // Kaybetme Kontrolü
        if (Mathf.Abs(currentZ) >= loseAngle)
        {
            LoseGame("❌ Kaybettin! Açı sınırı aşıldı.");
            return;
        }

        // Denge Kontrolü (Açı, kabul edilebilir aralıkta mı?)
        if (Mathf.Abs(currentZ) <= balanceTolerance)
        {
            balanceTimer += Time.deltaTime;
            
            if (balanceTimer >= winTime)
            {
                WinGame("🏆 Kazandın! Denge süresi tamamlandı.");
            }
        }
        else
        {
            // Denge aralığı dışındayız, sayacı sıfırla
            balanceTimer = 0f;
        }
    }
    
    // Z eksenindeki açıyı 0-360'tan -180/180 aralığına çevirir
    private float GetCurrentAngle()
    {
        float angle = rectTransform.localEulerAngles.z;
        if (angle > 180f) angle -= 360f; 
        return angle;
    }
    
    private void WinGame(string message)
    {
        gameFinished = true;
        
        // Rotasyonu sıfırla
        rectTransform.rotation = Quaternion.identity; 
        currentAngularVelocity = 0f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.MinigameSuccessTrigger(); // NPC'yi yola çıkar
        }
        Debug.Log(message);
    }
    
    private void LoseGame(string message)
    {
        gameFinished = true;
        // Rotasyonu durdur
        currentAngularVelocity = 0f;
        Debug.Log(message);
    }
}