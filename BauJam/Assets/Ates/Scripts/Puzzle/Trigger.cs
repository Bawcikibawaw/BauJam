using System;
using UnityEngine;
using System.Collections.Generic;

public class Trigger : MonoBehaviour
{
    [Header("Tetikleme Ayarları")]
    public string playerTag = "Player";
    private bool hasBeenTriggered = false;
    
    [Header("QTE ve Puzzle")]
    [Tooltip("Inspector'da sahnedeki QTEController objesini atayın.")]
    public QTEController qteController; 
    [Tooltip("QTE başarılı olduğunda açılacak Panel/Puzzle objesi.")]
    public GameObject puzzle; 

    // Referanslar
    private GameManager gmInstance; 

    void Start() 
    {
        gmInstance = GameManager.Instance;
        // Başlangıçta Puzzle'ı kapat
        if (puzzle != null) puzzle.SetActive(false); 
    }

    void OnEnable()
    {
        // DİNAMİK ABONELİK: Sadece QTE Event'ine abone ol
        QTEController.OnQTEFinished += HandleQTEFinished;
    }

    void OnDisable()
    {
        QTEController.OnQTEFinished -= HandleQTEFinished;
        
        // Kapanma event'inden aboneliği kaldır (güvenlik)
        if (gmInstance != null)
        {
            gmInstance.OnMinigameStatusChange -= HandleMinigameStatus; 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasBeenTriggered)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("Trigger: GameManager sahneye eklenmemiş!");
                return;
            }

            if (qteController == null)
            {
                Debug.LogError("Trigger: QTEController atanmamış!");
                return;
            }

            // 1. NPC rastgele hareketini başlat
            GameManager.Instance.SelectAndTriggerRandomTarget();
            
            // 2. QTE'yi başlat ve KENDİ Trigger referansını gönder!
            qteController.StartQTEProcess(this); 

            hasBeenTriggered = true; 
            GetComponent<Collider2D>().enabled = false;
        }
    }

    // 🚨 EVENT HANDLER: QTE başarılı olduğu an bu metot çağrılır.
    private void HandleQTEFinished(bool success, Trigger triggeredBy) 
    {
        // Filtre: Event'i tetikleyen objee bu değilse çık (Sadece bu Trigger'a ait sonuçsa işlem yap)
        if (triggeredBy != this) return; 

        if (success)
        {
            // YALNIZCA BAŞARILI OLDUĞUNDA KAPANMA EVENT'İNE ABONE OL
            if (gmInstance != null)
            {
                 // Kapanma Event'ine abone ol
                 gmInstance.OnMinigameStatusChange += HandleMinigameStatus; 
                 
                 // Puzzle'ı hemen aç (Kapanma komutu 20 sn sonra GameManager'dan gelecek)
                 if (puzzle != null)
                 {
                     puzzle.SetActive(true); 
                     Debug.Log($"Trigger: QTE Başarılı, {gameObject.name}'e ait Puzzle AÇIK.");
                 }
            }
        }
    }

    // 🚨 YENİ METOT: GameManager'dan gelen Aç/Kapat komutunu işler
    private void HandleMinigameStatus(bool status)
    {
        if (puzzle != null)
        {
            // Event true ise aç, false ise kapat
            puzzle.SetActive(status);
            Debug.Log($"Trigger: Minigame Kapanma/Açılma Komutu Geldi: {status}");
            
            // Eğer kapanma komutu geldiyse (status=false), aboneliği kaldır
            if (status == false && gmInstance != null)
            {
                gmInstance.OnMinigameStatusChange -= HandleMinigameStatus;
                Debug.Log($"Trigger: {gameObject.name} kapanma eventinden ayrıldı.");
            }
        }
    }
}