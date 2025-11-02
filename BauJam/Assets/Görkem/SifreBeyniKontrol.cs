using UnityEngine;
using System.Linq;
using System.Collections; // Coroutine kullanmak için bu satýr gerekli (YENÝ)

public class SifreBeyniKontrol : MonoBehaviour
{
    [Header("Görsel Ayarlarý")]
    public Sprite acikResim;
    public Sprite kapaliResim;

    [Header("Bulmaca Elemanlarý")]
    public BulmacaSalteri[] salterler;

    private int[] salterDurumlari = { 0, 0, 0, 0 };

    // Bulmacanýn çözülüp çözülmediðini kontrol eden bir bayrak (YENÝ)
    private bool bulmacaCozuldu = false;

    void Start()
    {
        GorselleriYenile();
    }

    public void BirSaltereBasildi(int hangiID)
    {
        // Eðer bulmaca zaten çözüldüyse, butonlarýn tekrar çalýþmasýný engelle (YENÝ)
        if (bulmacaCozuldu)
        {
            return; // Fonksiyondan çýk, hiçbir þey yapma
        }

        if (hangiID == 0) { DurumDegistir(0); DurumDegistir(1); }
        else if (hangiID == 1) { DurumDegistir(0); DurumDegistir(1); DurumDegistir(2); }
        else if (hangiID == 2) { DurumDegistir(1); DurumDegistir(2); DurumDegistir(3); }
        else if (hangiID == 3) { DurumDegistir(2); DurumDegistir(3); }

        GorselleriYenile();
        KazanmaKontrolu();
    }

    void DurumDegistir(int id)
    {
        if (id >= 0 && id < salterDurumlari.Length)
        {
            salterDurumlari[id] = 1 - salterDurumlari[id];
        }
    }

    void GorselleriYenile()
    {
        for (int i = 0; i < salterler.Length; i++)
        {
            if (salterDurumlari[i] == 1) { salterler[i].ResminiDegistir(acikResim); }
            else { salterler[i].ResminiDegistir(kapaliResim); }
        }
    }

    void KazanmaKontrolu()
    {
        if (salterDurumlari.All(durum => durum == 1))
        {
            // Paneli hemen kapatmak yerine, gecikmeyi baþlatan Coroutine'i çaðýr (YENÝ)
            bulmacaCozuldu = true; // Bayraðý indir, artýk butonlar çalýþmasýn
            StartCoroutine(KapanmaGecikmesi());
        }
    }

    // 2 saniye bekleyip sonra paneli kapatan fonksiyon (YENÝ)
    IEnumerator KapanmaGecikmesi()
    {
        Debug.Log("TEBRÝKLER! BULMACA ÇÖZÜLDÜ! Panel 2 saniye içinde kapanacak.");

        // 2 saniye bekle. Time.timeScale 0 olduðu için Realtime kullanmalýyýz.
        yield return new WaitForSecondsRealtime(2f);

        // Bekleme bittikten sonra bu kodlar çalýþýr
        Debug.Log("Panel kapatýlýyor ve oyun devam ediyor.");

        // Mini oyun penceresini kapat
        gameObject.transform.parent.gameObject.SetActive(false);

        // Ana oyunu devam ettir
        Time.timeScale = 1f;
    }
}