using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [Header("Tetikleme Ayarları")]
    public string playerTag = "Player";
    private bool hasBeenTriggered = false;
    
    public QTEController qteController;
    public GameObject puzzle;


    private void Update()
    {
        if (qteController.qteSuccsess)
        {
            puzzle.SetActive(true);
        }

        if (GameManager.Instance.isNPCMoving)
        {
            puzzle.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasBeenTriggered)
        {
            if (GameManager.Instance != null)
            {
                // GameManager'dan rastgele bir hedef seçmesini iste.
                GameManager.Instance.SelectAndTriggerRandomTarget();
                hasBeenTriggered = true; 
                
                // Opsiyonel: Collider'ı devre dışı bırak
                GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                Debug.LogError("GameManager sahneye eklenmemiş!");
            }
            
        }
    }
}