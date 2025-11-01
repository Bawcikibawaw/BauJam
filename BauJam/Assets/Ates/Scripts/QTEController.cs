using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEController : MonoBehaviour
{
    public static bool isGamePaused = false;
    public static int mana = 0;

    [Header("UI Elemanlar� (Buraya S�r�kle)")]
    public GameObject qtePanel;
    public Slider timerSlider;
    public TextMeshProUGUI sayacText;
    public TextMeshProUGUI sonucText;
    public GameObject basarisizHerifTextObjesi; // "ba�ar�s�z herif!" yazan obje

    [Header("QTE Ayarlar�")]
    public float qteSuresi = 5f;
    public int maxTiklama = 10;

    private GameObject carpilanKare;
    private Coroutine qteCoroutine;
    private int tiklamaSayisi;
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
        sayacText.text = tiklamaSayisi + " / " + maxTiklama;
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
        if (qteCoroutine != null && tiklamaSayisi < maxTiklama && Input.GetKeyDown(KeyCode.E))
        {
            tiklamaSayisi++;
            sayacText.text = tiklamaSayisi + " / " + maxTiklama;
            
            Debug.Log("ANAN");
        }
    }

    private void DegerlendirSonucu()
    {
        qtePanel.SetActive(false);

        if (tiklamaSayisi >= 1)
        {
            string mesaj = "";
            if (tiklamaSayisi == maxTiklama) { mana += 10; mesaj = "+10 MANA!"; }
            else if (tiklamaSayisi >= 5) { mana += 5; mesaj = "+5 MANA"; }
            else { mana += 3; mesaj = "+3 MANA"; }
            
            qteSuccsess = true;
            StartCoroutine(GosterSonucMesaji(mesaj));
        }
        else
        {
            qteSuccsess = false;
            // Ba�ar�s�zl�k durumunda bu Coroutine'i ba�lat
            StartCoroutine(GosterBasarisizMesaji());
        }
    }

    private IEnumerator GosterSonucMesaji(string mesaj)
    {
        sonucText.text = mesaj;
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