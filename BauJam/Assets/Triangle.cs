using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriangleController : MonoBehaviour
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

    void Start()
    {
        // Ba�lang��ta t�m UI elemanlar�n�n do�ru durumda oldu�undan emin olal�m
        if (qtePanel != null) qtePanel.SetActive(false);
        if (sonucText != null) sonucText.gameObject.SetActive(false);
        if (basarisizHerifTextObjesi != null) basarisizHerifTextObjesi.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGamePaused || qteCoroutine != null) return;
        if (collision.gameObject.CompareTag("Square"))
        {
            carpilanKare = collision.gameObject;
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

    public void OnButonaTiklandi()
    {
        if (qteCoroutine != null && tiklamaSayisi < maxTiklama)
        {
            tiklamaSayisi++;
            sayacText.text = tiklamaSayisi + " / " + maxTiklama;
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

            if (carpilanKare != null) Destroy(carpilanKare);
            StartCoroutine(GosterSonucMesaji(mesaj));
        }
        else
        {
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
    }
}