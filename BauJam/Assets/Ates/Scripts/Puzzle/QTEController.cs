using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEController : MonoBehaviour
{
    // QTE bittiğinde sonuç bilgisini yayınlayan Event
    public static event Action<bool> OnQTEFinished;

    public static bool isGamePaused = false;

    [Header("UI Elemanları")]
    public GameObject qtePanel;
    public Slider timerSlider;
    public TextMeshProUGUI sayacText;
    public TextMeshProUGUI sonucText;
    public GameObject basarisizHerifTextObjesi;

    [Header("QTE Ayarları")]
    public float qteSuresi = 5f;

    private GameObject carpilanKare;
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
    public void StartQTEProcess()
    {
        if (isGamePaused || qteCoroutine != null) return;
        
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

        float kalanZaman = qteSuresi;
        while (kalanZaman > 0)
        {
            kalanZaman -= Time.deltaTime;
            timerSlider.value = kalanZaman / qteSuresi;
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
                 GameManager.Instance.mana++;
            }
            sayacText.text = tiklamaSayisi + " / ";
        }
    }

    private void DegerlendirSonucu()
    {
        if (qtePanel != null) qtePanel.SetActive(false);

        if (tiklamaSayisi >= 1)
        {
            qteSuccsess = true;
            Debug.Log("QTE BAŞARILI! Event Yayınlanıyor.");
            
            // 🚨 Gecikmesiz açılış için Event hemen yayınlanır.
            if (OnQTEFinished != null) OnQTEFinished.Invoke(true); 

            StartCoroutine(GosterSonucMesaji());
        }
        else
        {
            qteSuccsess = false;
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
        carpilanKare = null;
        qteCoroutine = null;
        if (qteObject != null) qteObject.SetActive(false);
    }
}