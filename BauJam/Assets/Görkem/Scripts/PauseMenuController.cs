using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool isPaused = false;

    // Sadece ana duraklatma men�s� panelini kontrol edece�iz.
    public GameObject pauseMenuUI;

    void Update()
    {
        // "Esc" tu�una bas�ld���nda
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // E�er oyun zaten duraklat�lm��sa
            if (isPaused)
            {
                // Oyunu devam ettir
                Resume();
            }
            else
            {
                // Oyunu duraklat
                Pause();
            }
        }
    }

    // "Resume" butonu bu fonksiyonu �a��racak
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Zaman� normal ak���na d�nd�r
        isPaused = false;
    }

    // Esc'ye bas�nca bu fonksiyon �al��acak
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Zaman� tamamen durdur
        isPaused = true;
    }

    // "Ana Men�" butonu bu fonksiyonu �a��racak
    public void LoadMainMenu()
    {
        // Sahne de�i�tirmeden �nce zaman� normale d�nd�rmek �nemlidir.
        Time.timeScale = 1f;
        SceneManager.LoadScene("AnaMenu"); // Buraya kendi ana men� sahnenizin ad�n� yaz�n.
    }

    // "Quit" butonu bu fonksiyonu �a��racak
    public void QuitGame()
    {
        Debug.Log("Oyundan ��k�l�yor...");
        Application.Quit();
    }
} 