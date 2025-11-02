using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için bu kütüphane gerekli!

public class MainMenu : MonoBehaviour
{
    // Bu fonksiyon, yeni bir sahne yüklemek için kullanýlacak.
    // Hangi sahneye gideceðini Unity Editor içinden belirteceðiz.
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Bu fonksiyon, oyundan çýkmak için kullanýlacak.
    public void QuitGame()
    {
        // Editörde çalýþýyorsanýz, bu kod oyunu kapatmaz.
        // Sadece derlenmiþ (build edilmiþ) oyunda çalýþýr.
        Debug.Log("Oyundan Çýkýldý!"); // Editörde test için konsola mesaj yazdýrýr.
        Application.Quit();
    }
}