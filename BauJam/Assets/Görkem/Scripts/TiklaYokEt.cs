// AnaSalterKontrol.cs
using UnityEngine;

public class AnaSalterKontrol : MonoBehaviour
{
    // Bu de�i�ken, Unity edit�r�nde dolduraca��m�z bir kutucuk olu�turur.
    // Mini oyun penceremizi (Panel) bu kutucu�a s�r�kleyece�iz.
    public GameObject minigamePenceresi;

    // Bu obje fare ile t�kland���nda Unity bu fonksiyonu otomatik �al��t�r�r.
    void OnMouseDown()
    {
        Debug.Log("Ana �alter'e t�kland�!");

        // Pencereyi aktif hale getir, yani g�r�n�r yap.
        minigamePenceresi.SetActive(true);

        // Arkadaki oyunun durmas� i�in zaman� durdur.
        Time.timeScale = 0f;
    }
}