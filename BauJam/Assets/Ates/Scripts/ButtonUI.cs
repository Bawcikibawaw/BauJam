using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli kütüphane

public class SceneLoader : MonoBehaviour
{
    [Tooltip("Gideceği sahnenin Build Settings'deki indeks numarası veya adı.")]
    public string sceneToLoad = "Level_1"; 
    
    // YENİ METOT: Butonun OnClick() olayına atanacak
    public void LoadSceneByString()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Yüklenecek sahne adı/indeksi atanmamış!");
            return;
        }

        // Sahneyi ismine göre yükle
        SceneManager.LoadScene(sceneToLoad);
    }
    
    // (Opsiyonel) Sahneyi Build Index'ine göre yüklemek isterseniz:
    public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    // (Opsiyonel) Bir sonraki sahneyi yüklemek isterseniz:
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        // Build Settings'deki toplam sahne sayısını aşmadığından emin ol
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Bu, Build Settings'deki son sahnedir!");
        }
    }
}