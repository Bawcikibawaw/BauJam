using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEController : MonoBehaviour
{
    // QTE bittiğinde sonuç bilgisini ve HANGİ TRIGGER'dan geldiğini yayınlayan Event
    // Trigger.cs script'i buna abone olmalıdır.
    public static event Action<bool, Trigger> OnQTEFinished; // <-- KRİTİK GÜNCELLEME

    public static bool isGamePaused = false;

    [Header("UI Elemanları")]
    public GameObject qtePanel;
    public Slider timerSlider;
    public TextMeshProUGUI sayacText;
    public TextMeshProUGUI sonucText;
    public GameObject basarisizHerifTextObjesi;

    [Header("QTE Ayarları")]
    public float qteSuresi = 5f;

    private Trigger currentTrigger; // <-- Hangi Trigger'ın QTE'yi başlattığını tutar
    public Coroutine qteCoroutine;
    public int tiklamaSayisi;
    public GameObject qteObject; // QTE'yi içeren en dıştaki obje
    public bool qteSuccsess = false;

    void Start()
    {
        // Başlangıçta tüm görsel öğeleri kapatalım
        if (qtePanel != null) qtePanel.SetActive(false);
        if (sonucText != null) sonucText.gameObject.SetActive(false);
        if (basarisizHerifTextObjesi != null) basarisizHerifTextObjesi.SetActive(false);
        if (qteObject != null) qteObject.SetActive(false);
    }

    void Update()
    {
        QTEPress();
    }

    // Trigger'dan QTE'yi başlatmak için public metot
    // Parametre olarak HANGİ TRIGGER'ın başlattığını alır (Çözüm 1)
    public void StartQTEProcess(Trigger trigger) 
    {
        if (isGamePaused || qteCoroutine != null) return;
        
        // Hangi Trigger'ın başlattığını kaydet (Çözüm 2)
        currentTrigger = trigger; 

        if (qteObject != null)
        {
            qteObject.SetActive(true); 
        }
        else
        {
            Debug.LogError("QTEController: qteObject atanmamış!");
            return;
        }

        qteCoroutine = StartCoroutine(BaslatQTE());
    }

    public IEnumerator BaslatQTE()
    {
        tiklamaSayisi = 0; 
        sayacText.text = "0 / ";
        
        // Asıl paneli aç
        if (qtePanel != null) qtePanel.SetActive(true); 
        // Slider'ı başlangıç değerine ayarla
        if (timerSlider != null) timerSlider.maxValue = qteSuresi;
        
        float kalanZaman = qteSuresi;
        while (kalanZaman > 0)
        {
            kalanZaman -= Time.deltaTime;
            // Slider ve zamanı güncelle
            if (timerSlider != null) timerSlider.value = kalanZaman;
            yield return null;
        }
        DegerlendirSonucu();
    }

    public void QTEPress()
    {
        if (qteCoroutine != null && Input.GetKeyDown(KeyCode.E))
        {
            tiklamaSayisi++;
            if (GameManager.Instance != null)
            {
                 GameManager.Instance.mana++; // Mana'yı artır
            }
            sayacText.text = tiklamaSayisi + " / ";
        }
    }

    private void DegerlendirSonucu()
    {
        if (qtePanel != null) qtePanel.SetActive(false);
        
        // 🚨 Başarı Kriteri: En az 1 tıklama yapıldıysa başarılı say
        if (tiklamaSayisi >= 1)
        {
            qteSuccsess = true;
            Debug.Log("QTE BAŞARILI! Event Yayınlanıyor.");
            
            // Event'i yayınla ve HANGİ TRIGGER'dan geldiğini gönder
            if (OnQTEFinished != null && currentTrigger != null) 
            {
                OnQTEFinished.Invoke(true, currentTrigger); // <-- KRİTİK: Trigger objesini gönder
            } 

            StartCoroutine(GosterSonucMesaji());
        }
        else
        {
            qteSuccsess = false;
            // Başarısızlık durumunda da event yayınla (eğer dinleyen varsa)
            if (OnQTEFinished != null && currentTrigger != null) 
            {
                OnQTEFinished.Invoke(false, currentTrigger);
            } 
            
            StartCoroutine(GosterBasarisizMesaji());
        }
    }

    private IEnumerator GosterSonucMesaji()
    {
        if (sonucText != null) sonucText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f); 
        if (sonucText != null) sonucText.gameObject.SetActive(false);
        QTE_Sonlandir();
    }

    private IEnumerator GosterBasarisizMesaji()
    {
        isGamePaused = true;
        if (basarisizHerifTextObjesi != null) basarisizHerifTextObjesi.SetActive(true); 
        yield return new WaitForSeconds(2f);
        if (basarisizHerifTextObjesi != null) basarisizHerifTextObjesi.SetActive(false); 
        isGamePaused = false;
        QTE_Sonlandir(); 
    }

    private void QTE_Sonlandir()
    {
        currentTrigger = null; // Trigger referansını temizle
        qteCoroutine = null;
        if (qteObject != null) qteObject.SetActive(false);
    }
}