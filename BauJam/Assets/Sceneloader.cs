using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için bu kütüphane her zaman gerekli!

public class SceneLoade3r : MonoBehaviour
{
    // Butonun OnClick event'inden çaðýracaðýmýz fonksiyon.
    // Hangi sahneye gidileceðini Unity Editor içinden belirteceðiz.
    public void LoadSpecificScene(string sceneName)
    {
        // Belirtilen isimdeki sahneyi yükler.
        SceneManager.LoadScene(sceneName);
    }
}