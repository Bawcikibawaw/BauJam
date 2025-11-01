// BulmacaSalteri.cs
using UnityEngine;
using UnityEngine.UI;

public class BulmacaSalteri : MonoBehaviour
{
    public int benimIDm; // Bu �alterin kimli�i (0, 1, 2 veya 3)
    public SifreBeyniKontrol patronum; // Patronun kim oldu�unu buraya s�r�kleyece�iz

    // Button component'i t�kland���nda bu fonksiyonu �a��racak
    public void Tiklandim()
    {
        // Tek g�revi patrona haber vermek: "Hey, bana t�kland�! Benim ID'm bu."
        patronum.BirSaltereBasildi(benimIDm);
    }

    // Patron bu fonksiyonu �a��rarak resmini de�i�tirmesini emreder
    public void ResminiDegistir(Sprite yeniResim)
    {
        GetComponent<Image>().sprite = yeniResim;
    }
}