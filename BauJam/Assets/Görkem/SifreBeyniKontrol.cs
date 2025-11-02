using UnityEngine;
using System.Linq;
using System.Collections; // Coroutine kullanmak i�in bu sat�r gerekli (YEN�)

public class SifreBeyniKontrol : MonoBehaviour
{
    [Header("G�rsel Ayarlar�")]
    public Sprite acikResim;
    public Sprite kapaliResim;

    [Header("Bulmaca Elemanlar�")]
    public BulmacaSalteri[] salterler;

    private int[] salterDurumlari = { 0, 0, 0, 0 };

    // Bulmacan�n ��z�l�p ��z�lmedi�ini kontrol eden bir bayrak (YEN�)
    private bool bulmacaCozuldu = false;

    void Start()
    {
        GorselleriYenile();
    }

    public void BirSaltereBasildi(int hangiID)
    {
        // E�er bulmaca zaten ��z�ld�yse, butonlar�n tekrar �al��mas�n� engelle (YEN�)
        if (bulmacaCozuldu)
        {
            return; // Fonksiyondan ��k, hi�bir �ey yapma
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
            // Paneli hemen kapatmak yerine, gecikmeyi ba�latan Coroutine'i �a��r (YEN�)
            bulmacaCozuldu = true; // Bayra�� indir, art�k butonlar �al��mas�n
            gameObject.SetActive(false);
            GameManager.Instance.MinigameSuccessTrigger();
        }
    }
}