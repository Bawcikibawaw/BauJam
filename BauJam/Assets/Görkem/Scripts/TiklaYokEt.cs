// AnaSalterKontrol.cs
using UnityEngine;

public class AnaSalterKontrol : MonoBehaviour
{
    // Bu deðiþken, Unity editöründe dolduracaðýmýz bir kutucuk oluþturur.
    // Mini oyun penceremizi (Panel) bu kutucuða sürükleyeceðiz.
    public GameObject minigamePenceresi;

    // Bu obje fare ile týklandýðýnda Unity bu fonksiyonu otomatik çalýþtýrýr.
    void OnMouseDown()
    {
        Debug.Log("Ana Þalter'e týklandý!");

        // Pencereyi aktif hale getir, yani görünür yap.
        minigamePenceresi.SetActive(true);

        // Arkadaki oyunun durmasý için zamaný durdur.
        Time.timeScale = 0f;
    }
}