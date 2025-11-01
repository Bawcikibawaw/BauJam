using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool isPaused = false;

    // Sadece ana duraklatma menüsü panelini kontrol edeceðiz.
    public GameObject pauseMenuUI;

    void Update()
    {
        // "Esc" tuþuna basýldýðýnda
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Eðer oyun zaten duraklatýlmýþsa
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

    // "Resume" butonu bu fonksiyonu çaðýracak
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Zamaný normal akýþýna döndür
        isPaused = false;
    }

    // Esc'ye basýnca bu fonksiyon çalýþacak
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Zamaný tamamen durdur
        isPaused = true;
    }

    // "Ana Menü" butonu bu fonksiyonu çaðýracak
    public void LoadMainMenu()
    {
        // Sahne deðiþtirmeden önce zamaný normale döndürmek önemlidir.
        Time.timeScale = 1f;
        SceneManager.LoadScene("AnaMenu"); // Buraya kendi ana menü sahnenizin adýný yazýn.
    }

    // "Quit" butonu bu fonksiyonu çaðýracak
    public void QuitGame()
    {
        Debug.Log("Oyundan çýkýlýyor...");
        Application.Quit();
    }
} 