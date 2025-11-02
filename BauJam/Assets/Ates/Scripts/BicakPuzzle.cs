using UnityEngine;
using UnityEngine.UI;

public class BicakPuzzle : MonoBehaviour
{
    [Tooltip("Her karede bıçağın VÜCUDA DOĞRU artırılacağı açısal hız (negatif yönde).")]
    public float angularSpeed = 15f; // Hızı hala pozitif tanımlıyoruz, ancak Update içinde negatif yönde kullanacağız.

    [Tooltip("Her tıklamada VÜCUTTAN DIŞARI doğru itileceği açı miktarı.")]
    public float pushAwayAngle = 10f; 

    [Tooltip("Kazanmak için bıçağın vücuda değmemesi gereken süre (saniye).")]
    public float winTime = 5f;

    [Header("Rotasyon Limitleri")]
    [Tooltip("Bıçağın vücuda değdiği ve kaybedilen Z açısı (Negatif Değer).")]
    public float maxAngle = -45f; // <-- YENİ MAX: Vücuda değme (Örn: -45 derece)

    [Tooltip("Vücuttan en uzak (güvenli) Z açısı (Pozitif Değer).")]
    public float minAngle = 45f; // <-- YENİ MIN: Vücuttan uzak (Örn: +45 derece)

    // Gerekli bileşenler
    private RectTransform rectTransform;
    private bool isMoving = true;
    private float timer = 0f;
    private bool gameFinished = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Başlangıç açısını güvenli bölge olan MIN (pozitif) açıya yakın ayarla
        SetCurrentAngle(minAngle); 
        timer = 0f;

        // Ayarların doğru olduğunu kontrol et (güvenlik)
        if (maxAngle >= minAngle)
        {
            Debug.LogError("Rotasyon Limitleri Yanlış: MaxAngle, MinAngle'dan küçük olmalıdır (- Vücut, + Dış). Lütfen Inspector'ı kontrol edin.");
            enabled = false;
        }
    }

    void Update()
    {
        if (!isMoving || gameFinished)
        {
            return;
        }

        // 1. Kazanma Süresini Kontrol Et
        timer += Time.deltaTime;
        if (timer >= winTime)
        {
            WinGame();
            return;
        }

        // 2. Açısal Hızı Uygula (VÜCUDA DOĞRU ÇEK)
        float currentAngle = GetCurrentAngle();
        
        // Açısal hızı NEGATİF YÖNDE uygula (Vücuda doğru = Azalma)
        float newAngle = currentAngle - angularSpeed * Time.deltaTime;
        
        // 3. Kaybetme (Açı Limiti) Kontrolü
        if (newAngle <= maxAngle) // Eğer açı, MAX (negatif) limitin altına inerse
        {
            SetCurrentAngle(maxAngle); // Kaybetme açısına sabitle
            LoseGame("❌ Kaybettin! Bıçak vücuda değdi.");
            return;
        }
        
        // Açıyı MIN (pozitif) limitin üzerine çıkmayacak şekilde limitle
        newAngle = Mathf.Min(newAngle, minAngle); 

        // 4. Yeni Açıyı Uygula
        SetCurrentAngle(newAngle);
    }

    // Bu fonksiyon tıklama ile tetiklenecek
    public void OnClicked()
    {
        if (isMoving && !gameFinished)
        {
            // Tıklandığında, açıyı VÜCUTTAN DIŞARI doğru it (Artış)
            float currentAngle = GetCurrentAngle();

            // Açıyı artır
            float targetAngle = currentAngle + pushAwayAngle;
            
            // Açının maksimum limitin (minAngle) üzerine çıkmasını engelle
            targetAngle = Mathf.Min(targetAngle, minAngle);
            
            SetCurrentAngle(targetAngle);
            
            Debug.Log($"Tıklandı. Yeni Açı (Vücuttan Uzaklaştı): {targetAngle:F1} derece.");
        }
    }
    
    // Z eksenindeki açıyı 0-360'tan -180/180 aralığına çevirir
    private float GetCurrentAngle()
    {
        float angle = rectTransform.localEulerAngles.z;
        // Eğer açı 180'den büyükse (yani negatif bölgede), düzelt
        if (angle > 180) angle -= 360; 
        return angle;
    }

    // Z eksenindeki açıyı ayarlayan yardımcı metot
    private void SetCurrentAngle(float angle)
    {
        Vector3 rotation = rectTransform.localEulerAngles;
        rotation.z = angle;
        rectTransform.localEulerAngles = rotation;
    }
    
    // Kazanma metodu
    private void WinGame()
    {
        isMoving = false;
        gameFinished = true;
        SetCurrentAngle(minAngle); // Bıçağı güvenli konuma çek

        if (GameManager.Instance != null)
        {
            GameManager.Instance.MinigameSuccessTrigger();
        }
        Debug.Log("🏆 Kazandın! Süre doldu.");
    }
    
    // Kaybetme metodu
    private void LoseGame(string message)
    {
        isMoving = false;
        gameFinished = true;
        Debug.Log(message);
    }
}