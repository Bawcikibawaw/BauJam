using System;
using UnityEngine;

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

    void OnEnable()
    {
        // ABONELİK: Event'e abone ol (Doğru yer)
        QTEController.OnQTEFinished += HandleQTEFinished;
    }

    void OnDisable()
    {
        // TEMİZLİK: Aboneliği kaldır
        QTEController.OnQTEFinished -= HandleQTEFinished;
    }

    private void Update()
    {
        if (!GameManager.Instance.isNPCMoving)
        {
            puzzle.SetActive(false);
        }
    }

    // NPC hareket ediyorsa puzzle'ı kapatma mantığını koruyoruz.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasBeenTriggered)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager sahneye eklenmemiş!");
                return;
            }

            if (qteController == null)
            {
                Debug.LogError("Trigger: QTEController atanmamış!");
                return;
            }

            // 1. NPC hareketini başlat
            GameManager.Instance.SelectAndTriggerRandomTarget();
            
            // 🚨 2. QTE'yi başlat! (Gecikmesiz açılış için)
            qteController.StartQTEProcess(); 

            hasBeenTriggered = true; 
            GetComponent<Collider2D>().enabled = false;
        }
    }

    // 🚨 EVENT HANDLER: QTE başarılı olduğu an bu metot çağrılır.
    private void HandleQTEFinished(bool success)
    {
        if (success)
        {
            if (puzzle != null)
            {
                puzzle.SetActive(true);
                Debug.Log("HandleQTEFinished: Puzzle anında aktif edildi!");
            }
        }
    }
}