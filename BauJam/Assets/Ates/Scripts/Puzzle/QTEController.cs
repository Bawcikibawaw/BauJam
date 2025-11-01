using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEController : MonoBehaviour
{
    public static bool isGamePaused = false;

    [Header("UI Elemanlar� (Buraya S�r�kle)")]
    public GameObject qtePanel;
    public Slider timerSlider;
    public TextMeshProUGUI sayacText;
    public TextMeshProUGUI sonucText;
    public GameObject basarisizHerifTextObjesi; // "ba�ar�s�z herif!" yazan obje

    [Header("QTE Ayarlar�")]
    public float qteSuresi = 5f;


    private GameObject carpilanKare;
    private Coroutine qteCoroutine;
    public int tiklamaSayisi;
    public GameObject qteObject;
    public bool qteSuccsess = false;

    void Start()
    {
        // Ba�lang��ta t�m UI elemanlar�n�n do�ru durumda oldu�undan emin olal�m
        if (qtePanel != null) qtePanel.SetActive(false);
        if (sonucText != null) sonucText.gameObject.SetActive(false);
        if (basarisizHerifTextObjesi != null) basarisizHerifTextObjesi.SetActive(false);
    }

    void Update()
    {
        QTEPress();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Oyun durdurulmuşsa veya QTE zaten aktifse (çalışan bir Coroutine varsa) çık.
        if (isGamePaused || qteCoroutine != null) return;
    
        // Çarpışan objenin (other) etiketi "Square" ise devam et.
        if (other.gameObject.CompareTag("Square"))
        {
            // Tetikleyiciye giren objeyi kaydet.
            carpilanKare = other.gameObject;
            
            qteObject.SetActive(true);
        
            // QTE Coroutine'ini başlat ve referansını kaydet.
            qteCoroutine = StartCoroutine(BaslatQTE());
        }
    }

    private IEnumerator BaslatQTE()
    {
        tiklamaSayisi = 0; 
        qtePanel.SetActive(true);

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
            GameManager.Instance.mana++;
            sayacText.text = tiklamaSayisi + " / ";
            
            Debug.Log("ANAN");
        }
    }

    private void DegerlendirSonucu()
    {
        qtePanel.SetActive(false);

        if (tiklamaSayisi >= 1)
        {
            qteSuccsess = true;
            Debug.Log("HALAN");
            StartCoroutine(GosterSonucMesaji());
        }
        else
        {
            qteSuccsess = false;
            // Ba�ar�s�zl�k durumunda bu Coroutine'i ba�lat
            StartCoroutine(GosterBasarisizMesaji());
        }
    }

    private IEnumerator GosterSonucMesaji()
    {
        sonucText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        sonucText.gameObject.SetActive(false);
        QTE_Sonlandir();
    }

    private IEnumerator GosterBasarisizMesaji()
    {
        Debug.Log("Ba�ar�s�zl�k fonksiyonu �al��t�. Yaz�n�n �imdi g�r�nmesi laz�m.");
        isGamePaused = true;
        basarisizHerifTextObjesi.SetActive(true); // G�R�N�R YAP
        yield return new WaitForSeconds(2f);
        basarisizHerifTextObjesi.SetActive(false); // G�ZLE
        QTE_Sonlandir();
        isGamePaused = false;
    }

    private void QTE_Sonlandir()
    {
        carpilanKare = null;
        qteCoroutine = null;
        qteObject.SetActive(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNPCMoving = false;
        }
    }
}