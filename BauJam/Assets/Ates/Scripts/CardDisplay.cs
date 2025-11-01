using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro kullanıyorsanız ekleyin

public class CardDisplay : MonoBehaviour
{
    // 1. Şablonun üzerindeki UI Bileşenleri
    [Header("UI Elementleri")]
    public TextMeshProUGUI levelText; // Veya sadece public Text levelText;
    public TextMeshProUGUI manaText;  // Veya sadece public Text manaText;
    public TextMeshProUGUI disclaimerText; // Veya sadece public Text disclaimerText;
    public Image cardImage;
    public Button purchaseButton; // Prefab'deki Buton bileşeni
    private PainSO currentCardData; // Tıklama için veriyi saklar


    public void DisplayCard(PainSO cardData)
    {
        
        currentCardData = cardData;
        
        if (cardData == null)
        {
            Debug.LogError("DisplayCard metoduna geçersiz (null) kart verisi gönderildi!");
            return;
        }

        // 2. SO Verilerini UI Bileşenlerine Yükleme
        
        // Seviye ve Mana Gereksinimleri
        levelText.text = cardData.painLevel.ToString();
        manaText.text = cardData.manaRequirement.ToString();

        // Açıklama Metni
        disclaimerText.text = cardData.disclaimer;
        
        if (cardData.card != null) // Eğer SO'daki Image yerine Sprite kullanıyorsanız
        {
              cardImage.sprite = cardData.card;
        }
        
        if (purchaseButton != null)
        {
            // 🚨 Butonun OnClick Event'ine kendi fonksiyonumuzu dinamik olarak ekle.
            // Önceki varsa temizle.
            purchaseButton.onClick.RemoveAllListeners(); 
            // Dinamik olarak 'AttemptPurchase' metodunu bu kart verisiyle bağla.
            purchaseButton.onClick.AddListener(AttemptPurchase); 
        }
    }
    
    public void AttemptPurchase()
    {
        if (currentCardData == null) return;

        // Satın alma/etkileşim mantığını burada başlatacağız.
        Debug.Log($"'{currentCardData.disclaimer}' kartı tıklandı.");

        // Bu veriyi bir satın alma yöneticisine (örneğin GameManager'a) gönder:
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BuyCard(currentCardData);
        }
    }
}