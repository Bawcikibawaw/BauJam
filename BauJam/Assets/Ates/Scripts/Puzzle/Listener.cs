using UnityEngine;
using UnityEngine.UI;
using System;

public class Listener : MonoBehaviour
{
    [Header("Buton Referansı")]
    [Tooltip("Hiyerarşiden, bir sonraki seviyeyi yükleyecek butonu buraya sürükleyin.")]
    public Button nextLevelButton;

    private void Start()
    {
        // GameManager'ın kalıcı objesinin oluştuğundan emin olmak için kontrol
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance bulunamadı. Lütfen LVL1'in (Başlangıç Sahnesi) Build Settings'te ilk yüklendiğinden emin olun.");
            this.enabled = false;
            return;
        }

        if (nextLevelButton == null)
        {
            Debug.LogError("nextLevelButton referansı MenuButtonLinker'a atanmamış!");
            this.enabled = false;
            return;
        }

        // 1. Önceki dinleyicileri temizle (güvenlik için)
        nextLevelButton.onClick.RemoveAllListeners();

        // 2. Butonun OnClick olayına, GameManager'daki LoadNextTrackedLevel metodunu dinamik olarak ata.
        nextLevelButton.onClick.AddListener(GameManager.Instance.LoadNextTrackedLevel);

        Debug.Log("Menü Butonu, GameManager.LoadNextTrackedLevel'a başarıyla bağlandı.");
    }
}